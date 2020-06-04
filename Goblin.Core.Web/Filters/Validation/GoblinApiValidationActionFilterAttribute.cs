using System.Collections.Generic;
using System.Linq;
using System.Net;
using Elect.Core.XmlUtils;
using Elect.Web.Models;
using Goblin.Core.Errors;
using Goblin.Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Goblin.Core.Web.Filters.Validation
{
    public class GoblinApiValidationActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid) return;

            // Log Error
            var keyValueInvalidDictionary = GetModelStateInvalidInfo(context);

            // Response Result
            var apiErrorViewModel =
                new GoblinErrorModel(nameof(GoblinErrorCode.BadRequest), GoblinErrorCode.BadRequest,
                    StatusCodes.Status400BadRequest)
                {
                    AdditionalData = keyValueInvalidDictionary
                };

            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            if (context.HttpContext.Request.Headers[HttpRequestHeader.Accept.ToString()] == ContentType.Xml)
                context.Result = new ContentResult
                {
                    ContentType = ContentType.Xml,
                    StatusCode = context.HttpContext.Response.StatusCode,
                    Content = XmlHelper.ToXmlString(apiErrorViewModel)
                };
            else
            {
                context.Result = new ContentResult
                {
                    ContentType = ContentType.Json,
                    StatusCode = context.HttpContext.Response.StatusCode,
                    Content = JsonConvert.SerializeObject(apiErrorViewModel, GoblinJsonSetting.JsonSerializerSettings)
                };
            }
        }

        public static Dictionary<string, object> GetModelStateInvalidInfo(ActionExecutingContext context)
        {
            var keyValueInvalidDictionary = new Dictionary<string, object>();

            foreach (var keyValueState in context.ModelState)
            {
                var error = string.Join(", ", keyValueState.Value.Errors.Select(x => x.ErrorMessage));

                keyValueInvalidDictionary.Add(keyValueState.Key, error);
            }

            return keyValueInvalidDictionary;
        }
    }
}