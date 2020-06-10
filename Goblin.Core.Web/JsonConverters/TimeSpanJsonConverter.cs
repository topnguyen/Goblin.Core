using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Goblin.Core.DateTimeUtils;

namespace Goblin.Core.Web.JsonConverters
{
    public class TimeSpanJsonConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            var value = reader.GetString()?.ToSystemTimeSpan();

            if (value != null)
            {
                return value.Value;
            }

            throw new FormatException("The Data not valid TimeSpan Type");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan timeSPanValue, JsonSerializerOptions options)
        {
            writer.WriteStringValue(timeSPanValue.ToString());
        }
    }
}