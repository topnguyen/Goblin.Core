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

        public GoblinErrorModel ErrorModel => ToExceptionModel();
        
        private readonly GoblinErrorModel _errorModel;
        
        public GoblinException(string code, string message = "", int statusCode = StatusCodes.Status400BadRequest):base(message)
        {
            Code = code;
            StatusCode = statusCode;
        }

        public GoblinException(GoblinErrorModel errorModel)
        {
            _errorModel = errorModel;
        }

        private GoblinErrorModel ToExceptionModel()
        {
            if (_errorModel != null)
            {
                return _errorModel;
            }
            
            return new GoblinErrorModel(this);
        }
    }
}