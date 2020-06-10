using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Goblin.Core.DateTimeUtils;
using Goblin.Core.Settings;

namespace Goblin.Core.Web.JsonConverters
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            var value = reader.GetString()?.ToSystemDateTime();

            return value?.DateTime ?? reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime dateTimeValue, JsonSerializerOptions options)
        {
            writer.WriteStringValue(dateTimeValue.ToString(GoblinDateTimeSetting.DateTimeFormat));
        }
    }
}