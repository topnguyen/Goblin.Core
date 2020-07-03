using System.Collections.Generic;
using System.Linq;

namespace Goblin.Core.Utils
{
    public static class StringExtensions
    {
        public delegate bool TryParseHandler<T>(string value, out T result);
        
        public static T? TryParse<T>(string value, TryParseHandler<T> handler) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (handler(value, out var result))
            {
                return result;
            }
            
            return null;
        }
        
        public static List<T> ToList<T>(this string value, TryParseHandler<T> handler, char separateChar = ',') where T : struct
        {
            var listParsed = new List<T>();

            if (!string.IsNullOrWhiteSpace(value))
            {
                listParsed = value
                    .Split(separateChar)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => TryParse<T>(x.Trim(), handler))
                    .Where(x => x != null)
                    .Select(x => x.Value)
                    .Distinct()
                    .ToList();
            }

            return listParsed;
        }
    }
}