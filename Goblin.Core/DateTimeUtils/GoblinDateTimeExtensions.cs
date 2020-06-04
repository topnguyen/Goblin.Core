using System;
using System.Globalization;
using Elect.Core.DateTimeUtils;
using Goblin.Core.Settings;

namespace Goblin.Core.DateTimeUtils
{
    public static class GoblinDateTimeExtensions
    {
        public static DateTimeOffset UtcToSystemTime(this DateTimeOffset dateTimeOffsetUtc)
        {
            var dateTime = dateTimeOffsetUtc.UtcDateTime.UtcToSystemTime();

            var dateTimeOffset = dateTime.WithTimeZone(GoblinDateTimeHelper.TimeZoneInfo);

            return dateTimeOffset;
        }
        
        public static DateTime UtcToSystemTime(this DateTime dateTimeUtc)
        {
            var dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, GoblinDateTimeHelper.TimeZoneInfo);

            return dateTime;
        }
        
        public static DateTimeOffset? ToSystemDateTime(this string dateTimeString)
        {
            DateTimeOffset result;

            if (DateTime.TryParseExact(dateTimeString, GoblinDateTimeSetting.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
            {
                result = dateTime;
            }
            else if (DateTime.TryParseExact(dateTimeString, GoblinDateTimeSetting.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                result = date;
            }
            else
            {
                return null;
            }

            result = result.DateTime.WithTimeZone(GoblinDateTimeSetting.TimeZone);

            return result;
        }
        
        public static TimeSpan? ToSystemTimeSpan(this string timeSpanString)
        {
            TimeSpan result;

            if (DateTime.TryParseExact(timeSpanString, GoblinDateTimeSetting.TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
            {
                result = dateTime.TimeOfDay;
            }
            else if (TimeSpan.TryParse(timeSpanString, CultureInfo.InvariantCulture, out var timeSpan))
            {
                result = timeSpan;
            }
            else
            {
                return null;
            }

            return result;
        }

        public static string ToSystemString(this TimeSpan timeSpan)
        {
            var result = DateTime.Today.Add(timeSpan).ToString(GoblinDateTimeSetting.TimeFormat);

            return result;
        }
    }
}