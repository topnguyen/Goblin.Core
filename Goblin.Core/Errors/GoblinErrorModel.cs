using System;
using System.Collections.Generic;

namespace Goblin.Core.Errors
{
    public class GoblinErrorModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        ///     Unique error code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Message/Description of error
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Http response code
        /// </summary>
        public int StatusCode { get; set; }

        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();

        public GoblinErrorModel()
        {
        }

        public GoblinErrorModel(string code) : this()
        {
            Code = code;
        }

        public GoblinErrorModel(string code, string message) : this(code)
        {
            Message = message;
        }

        public GoblinErrorModel(string code, string message, int statusCode) : this(code, message)
        {
            StatusCode = statusCode;
        }

        public GoblinErrorModel(GoblinException exception) : this(exception.Code, exception.Message, exception.StatusCode)
        {
            Id = exception.Id;
            
            AdditionalData = exception.AdditionalData;
        }
    }
}