using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elect.Core.DictionaryUtils;
using Elect.Core.ObjUtils;
using Elect.Web.Middlewares.HttpContextMiddleware;

namespace Goblin.Core.Web.Utils
{
    /// <summary>
    ///     Set and Get singleton value per HTTP Request
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>This method require to use Elect Http Context Middleware</remarks>
    public static class SingletonPerHttpRequest<T> where T : class, new()
    {
        private static string Key => typeof(T).GetTypeInfo().FullName;

        public static T Current
        {
            get => Get();
            set => Set(value);
        }

        private static T Get()
        {
            if (HttpContext.Current?.Items != null)
            {
                return HttpContext.Current.Items.TryGetValue(Key, out var value) ? value?.ConvertTo<T>() : null;
            }

            return null;
        }

        private static void Set(T value)
        {
            if (HttpContext.Current.Items?.Any() != true)
            {
                HttpContext.Current.Items = new Dictionary<object, object>();
            }

            if (value == null)
            {
                if (HttpContext.Current.Items?.ContainsKey(Key) == true) HttpContext.Current.Items.Remove(Key);

                return;
            }

            HttpContext.Current?.Items.AddOrUpdate(Key, value);
        }
    }
}