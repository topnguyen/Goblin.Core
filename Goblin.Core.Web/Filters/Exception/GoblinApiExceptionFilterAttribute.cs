using System;
using System.Linq;
using System.Text.Json;
using Elect.Core.EnvUtils;
using Elect.Core.XmlUtils;
using Elect.DI.Attributes;
using Elect.Web.Models;
using Goblin.Core.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.PlatformAbstractions;
using OpenTracing;
using ContentType = Elect.Web.Models.ContentType;

namespace Goblin.Core.Web.Filters.Exception
{
    [ScopedDependency]
    public class GoblinApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ITracer _jaegerTracer;

        public GoblinApiExceptionFilterAttribute(ITracer jaegerTracer)
        {
            _jaegerTracer = jaegerTracer;
        }

        public override void OnException(ExceptionContext context)
        {
            // Ignore cancel action
            if (context.Exception is OperationCanceledException || context.Exception is ObjectDisposedException)
            {
                context.ExceptionHandled = true;

                return;
            }

            var errorModel = GetErrorModel(context);

            // Log to Jaeger
            
            LogToJaeger(context, errorModel, _jaegerTracer);

            // Response Result

            if(!EnvHelper.IsDevelopment())
            {
                // Additional Data for Dev only
                errorModel.AdditionalData = null;
            }
            
            if (context.HttpContext.Request.Headers[HeaderKey.Accept] == ContentType.Xml ||
                context.HttpContext.Request.Headers[HeaderKey.ContentType] == ContentType.Xml)
                context.Result = new ContentResult
                {
                    ContentType = ContentType.Xml,
                    Content = XmlHelper.ToXmlString(errorModel),
                    StatusCode = context.HttpContext.Response.StatusCode
                };
            else
            {
                context.Result = new ContentResult
                {
                    ContentType = ContentType.Json,
                    Content = JsonSerializer.Serialize(errorModel),
                    StatusCode = errorModel.StatusCode
                };
            }

            context.ExceptionHandled = true;

            // Keep base Exception

            base.OnException(context);
        }

        private static GoblinErrorModel GetErrorModel(ExceptionContext exceptionContext)
        {
            GoblinErrorModel errorModel;

            switch (exceptionContext.Exception)
            {
                case GoblinException exception:
                {
                    errorModel = new GoblinErrorModel(exception);

                    break;
                }

                case UnauthorizedAccessException _:
                {
                    errorModel = new GoblinErrorModel(nameof(GoblinErrorCode.UnAuthorized),
                        GoblinErrorCode.UnAuthorized, StatusCodes.Status401Unauthorized);

                    break;
                }

                default:
                {
                    var message = EnvHelper.IsDevelopment()
                        ? exceptionContext.Exception.Message
                        : GoblinErrorCode.Unknown;

                    errorModel = new GoblinErrorModel(nameof(GoblinErrorCode.Unknown), message,
                        StatusCodes.Status500InternalServerError);

                    break;
                }
            }

            errorModel.AdditionalData.Add("exception", new
            {
                message = exceptionContext.Exception.Message,
                source = exceptionContext.Exception.Source,
                stackTrade = exceptionContext.Exception.StackTrace,
                innerException = new
                {
                    message = exceptionContext.Exception.InnerException?.Message,
                    source = exceptionContext.Exception.InnerException?.Source,
                    stackTrade = exceptionContext.Exception.InnerException?.StackTrace,
                }
            });

            exceptionContext.HttpContext.Response.StatusCode = errorModel.StatusCode;

            return errorModel;
        }

        private static void LogToJaeger(ExceptionContext exceptionContext, GoblinErrorModel errorModel, ITracer jaegerTracer)
        {
            var operationName = "💩😭💩 EXCEPTION 🔥🐛🔥";

            var builder = jaegerTracer.BuildSpan(operationName);

            using var scope = builder.StartActive(true);

            var span = scope.Span;

            if (errorModel != null)
            {
                jaegerTracer.ActiveSpan.SetTag("goblin.app_name", PlatformServices.Default.Application.ApplicationName);
                jaegerTracer.ActiveSpan.SetTag("goblin.log_type", "exception");
                jaegerTracer.ActiveSpan.SetTag("goblin.exception.id", errorModel.Id);
                jaegerTracer.ActiveSpan.SetTag("goblin.exception.code", errorModel.Code);
                jaegerTracer.ActiveSpan.SetTag("goblin.exception.message", errorModel.Message);
                jaegerTracer.ActiveSpan.SetTag("goblin.exception.status_code", errorModel.StatusCode);

                if (errorModel.AdditionalData?.Any() == true)
                {
                    span.Log(errorModel.AdditionalData);
                }
            }
            else
            {
                span.Log(exceptionContext.Exception.Message);
            }
        }
    }
}