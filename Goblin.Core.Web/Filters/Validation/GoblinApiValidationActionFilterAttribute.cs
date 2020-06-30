using System.Collections.Generic;
using System.Linq;
using System.Net;
using Elect.Core.XmlUtils;
using Elect.DI.Attributes;
using Elect.Web.Models;
using Goblin.Core.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OpenTracing;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Goblin.Core.Web.Filters.Validation
{
    [ScopedDependency]
    public class GoblinApiValidationActionFilterAttribute : ActionFilterAttribute
    {
        private readonly ITracer _jaegerTracer;

        public GoblinApiValidationActionFilterAttribute(ITracer jaegerTracer)
        {
            _jaegerTracer = jaegerTracer;
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid) return;

            var keyValueInvalidDictionary = GetModelStateInvalidInfo(context);

            var errorModel =
                new GoblinErrorModel(nameof(GoblinErrorCode.BadRequest), GoblinErrorCode.BadRequest,
                    StatusCodes.Status400BadRequest)
                {
                    AdditionalData = keyValueInvalidDictionary
                };
            
            // Log to Jaeger
            
            LogToJaeger(errorModel, _jaegerTracer);

            // Response Result

            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            if (context.HttpContext.Request.Headers[HttpRequestHeader.Accept.ToString()] == ContentType.Xml)
                context.Result = new ContentResult
                {
                    ContentType = ContentType.Xml,
                    StatusCode = context.HttpContext.Response.StatusCode,
                    Content = XmlHelper.ToXmlString(errorModel)
                };
            else
            {
                context.Result = new ContentResult
                {
                    ContentType = ContentType.Json,
                    StatusCode = context.HttpContext.Response.StatusCode,
                    Content = JsonSerializer.Serialize(errorModel)
                };
            }
        }

        private static Dictionary<string, object> GetModelStateInvalidInfo(ActionContext context)
        {
            var keyValueInvalidDictionary = new Dictionary<string, object>();

            foreach (var keyValueState in context.ModelState)
            {
                var error = string.Join(", ", keyValueState.Value.Errors.Select(x => x.ErrorMessage));

                keyValueInvalidDictionary.Add(keyValueState.Key, error);
            }

            return keyValueInvalidDictionary;
        }
        
        private static void LogToJaeger(GoblinErrorModel errorModel, ITracer jaegerTracer)
        {
            var operationName = "👀 INVALID DATA 🤣";

            var builder = jaegerTracer.BuildSpan(operationName);

            using var scope = builder.StartActive(true);

            var span = scope.Span;

            if (errorModel != null)
            {
                jaegerTracer.ActiveSpan.SetTag("Exception Id", errorModel.Id);
                jaegerTracer.ActiveSpan.SetTag("Exception Code", errorModel.Code);
                jaegerTracer.ActiveSpan.SetTag("Exception Message", errorModel.Message);
                jaegerTracer.ActiveSpan.SetTag("Exception Status Code", errorModel.StatusCode);

                if (errorModel.AdditionalData?.Any() == true)
                {
                    span.Log(errorModel.AdditionalData);
                }
            }
        }
    }
}