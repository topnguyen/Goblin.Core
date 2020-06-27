using Elect.Core.ObjUtils;
using Microsoft.AspNetCore.Http;

namespace Goblin.Core.Web.Utils
{
    public static class HttpContextExtensions
    {
        public static T GetKey<T>(this HttpContext httpContext, string keyName)
        {
            httpContext.Request.Headers.TryGetValue(keyName, out var keyInHeader);

            string keyStrData = keyInHeader;

            if (string.IsNullOrWhiteSpace(keyStrData))
            {
                keyStrData = httpContext.Request.Cookies[keyName];
            }

            if (string.IsNullOrWhiteSpace(keyInHeader))
            {
                keyStrData = httpContext.Request.Query[keyName];
            }

            ObjHelper.TryConvertTo(keyStrData, default(T), out var keyData);

            return keyData;
        }
    }
}