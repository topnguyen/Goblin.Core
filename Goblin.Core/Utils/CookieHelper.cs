using System.Linq;
using System.Text.Json;
using Goblin.Core.DateTimeUtils;
using Microsoft.AspNetCore.Http;

namespace Goblin.Core.Utils
{
public static class CookieHelper
    {
        public static void Set<T>(HttpContext httpContext, string key, T data, CookieOptions options = null)
        {
            var dataJson = JsonSerializer.Serialize(data);

            httpContext.Response.Cookies.Append(key, dataJson, options ?? new CookieOptions
            {
                Expires = null,
                HttpOnly = true,
                Secure = false
            });
        }

        public static T Get<T>(HttpContext httpContext, string key)
        {
            if (!httpContext.Request.Cookies.TryGetValue(key, out var dataJson))
            {
                return default;
            }

            try
            {
                var data = JsonSerializer.Deserialize<T>(dataJson);

                return data;
            }
            catch
            {
                return default;
            }
        }

        public static void Expire(HttpContext httpContext, string key)
        {
            httpContext.Response.Cookies.Append(key, string.Empty, new CookieOptions
            {
                Expires = GoblinDateTimeHelper.SystemTimeNow.AddYears(-1),
                HttpOnly = true,
                Secure = false
            });
        }

        // Share

        public static void SetShare<T>(HttpContext httpContext, string key, T data, CookieOptions options = null)
        {
            options ??= new CookieOptions
            {
                Expires = null,
                HttpOnly = true,
                Secure = false
            };

            var dataJson = JsonSerializer.Serialize(data);

            if (!httpContext.Request.Host.Value.Contains('.'))
            {
                httpContext.Response.Cookies.Append(key, dataJson, options);

                return;
            }

            var parts = httpContext.Request.Host.Value.Split('.').ToList();

            var dotName = parts[parts.Count - 1];

            var domainName = parts[parts.Count - 2];

            options.Domain = $".{domainName}.{dotName}";

            var listCountryDomain = new[] {"edu", "org", "info", "gov", "name", "health", "biz", "pro"};

            if (listCountryDomain.Contains(domainName))
            {
                options.Domain = $".{parts[parts.Count - 3]}{options.Domain}";
            }

            httpContext.Response.Cookies.Append(key, dataJson, options);
        }

        public static T GetShare<T>(HttpContext httpContext, string key)
        {
            if (!httpContext.Request.Cookies.TryGetValue(key, out var dataJson))
            {
                return default;
            }

            try
            {
                var data = JsonSerializer.Deserialize<T>(dataJson);

                return data;
            }
            catch
            {
                return default;
            }
        }

        public static void ExpireShare(HttpContext httpContext, string key)
        {
            var options = new CookieOptions
            {
                Expires = GoblinDateTimeHelper.SystemTimeNow.AddYears(-1),
                HttpOnly = true,
                Secure = false
            };

            if (!httpContext.Request.Host.Value.Contains('.'))
            {
                httpContext.Response.Cookies.Append(key, string.Empty, new CookieOptions
                {
                    Expires = GoblinDateTimeHelper.SystemTimeNow.AddYears(-1),
                    HttpOnly = true,
                    Secure = false
                });

                return;
            }

            var parts = httpContext.Request.Host.Value.Split('.').ToList();

            var dotName = parts[^1];

            var domainName = parts[^2];

            options.Domain = $".{domainName}.{dotName}";

            var listCountryDomain = new[] {"edu", "org", "info", "gov", "name", "health", "biz", "pro"};

            if (listCountryDomain.Contains(domainName))
            {
                options.Domain = $".{parts[^3]}{options.Domain}";
            }

            httpContext.Response.Cookies.Append(key, string.Empty, options);
        }
    }
}