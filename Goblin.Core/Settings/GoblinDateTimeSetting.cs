namespace Goblin.Core.Settings
{
    public static class GoblinDateTimeSetting
    {
        /// <summary>
        ///     Config use datetime with TimeZone. Default is "UTC", See more: https://msdn.microsoft.com/en-us/library/gg154758.aspx
        /// </summary>
        public const string TimeZone = "SE Asia Standard Time"; // "UTC"

        public const string DateFormat = "dd/MM/yyyy";

        public const string TimeFormat = "h:mm:ss tt";

        public const string DateTimeFormat = DateFormat + " " + TimeFormat;
    }
}