using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Goblin.Core.Errors
{
    public class GoblinException : Exception
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public string Code { get; }

        public int StatusCode { get; set; }

        [JsonExtensionData] 
        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();

        public GoblinException(string code, string message = "", int statusCode = StatusCodes.Status400BadRequest):base(message)
        {
            Code = code;
            StatusCode = statusCode;
        }

        public GoblinErrorModel ToExceptionModel()
        {
            return new GoblinErrorModel(this);
        }
    }
}