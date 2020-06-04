using System;
using Elect.Core.EnvUtils;
using Elect.Logger.Logging;
using Elect.Logger.Models.Logging;
using Goblin.Core.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Goblin.Core.Web.Filters.Exception
{
    public static class GoblinExceptionContextHelper
    {
        public static GoblinErrorModel GetErrorModel(ExceptionContext context, IElectLog electLog)
        {
            GoblinErrorModel errorModel;

            switch (context.Exception)
            {
                case GoblinException exception:
                {
                    errorModel = new GoblinErrorModel(exception);
                    
                    break;
                }

                case UnauthorizedAccessException _:
                {
                    errorModel = new GoblinErrorModel(nameof(GoblinErrorCode.UnAuthorized), GoblinErrorCode.UnAuthorized, StatusCodes.Status401Unauthorized);
                  
                    break;
                }

                default:
                {
                    var message = EnvHelper.IsDevelopment() ? context.Exception.Message : GoblinErrorCode.Unknown;
                    
                    errorModel = new GoblinErrorModel(nameof(GoblinErrorCode.Unknown), message, StatusCodes.Status500InternalServerError);
                    
                    break;
                }
            }

            electLog.Capture(context.Exception, LogType.Error, context.HttpContext);

            if (EnvHelper.IsDevelopment())
            {
                errorModel.AdditionalData.Add("exception", new
                {
                    message = context.Exception.Message,
                    source = context.Exception.Source,
                    stackTrade = context.Exception.StackTrace,
                    innerException = new
                    {
                        message = context.Exception.InnerException?.Message,
                        source = context.Exception.InnerException?.Source,
                        stackTrade = context.Exception.InnerException?.StackTrace,
                    }
                });
            }

            context.HttpContext.Response.StatusCode = errorModel.StatusCode;

            return errorModel;
        }
    }
}