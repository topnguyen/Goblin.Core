using System.Globalization;
using Newtonsoft.Json;

namespace Goblin.Core.Settings
{
    public static class GoblinJsonSetting
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Elect.Core.Constants.Formatting.JsonSerializerSettings.Formatting,
            NullValueHandling = Elect.Core.Constants.Formatting.JsonSerializerSettings.NullValueHandling,
            MissingMemberHandling = Elect.Core.Constants.Formatting.JsonSerializerSettings.MissingMemberHandling,
            DateFormatHandling = Elect.Core.Constants.Formatting.JsonSerializerSettings.DateFormatHandling,
            ReferenceLoopHandling = Elect.Core.Constants.Formatting.JsonSerializerSettings.ReferenceLoopHandling,
            ContractResolver = Elect.Core.Constants.Formatting.JsonSerializerSettings.ContractResolver,
            DateFormatString = GoblinDateTimeSetting.DateTimeFormat,
            Culture = CultureInfo.InvariantCulture
        };
    }
}