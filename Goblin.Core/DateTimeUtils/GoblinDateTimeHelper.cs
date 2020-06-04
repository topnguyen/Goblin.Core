using System;
using Goblin.Core.Settings;

namespace Goblin.Core.DateTimeUtils
{
    public class GoblinDateTimeHelper
    {
        public static TimeZoneInfo TimeZoneInfo => Elect.Core.DateTimeUtils.DateTimeHelper.GetTimeZoneInfo(GoblinDateTimeSetting.TimeZone);
        
        public static DateTimeOffset SystemTimeNow => DateTimeOffset.UtcNow.UtcToSystemTime();
    }
}