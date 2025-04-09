namespace FFEC
{
    internal static class JsonStorage
    {
        public static string GetControlText(string sector, string name)
        {
            return controlsData[sector][name].Value<string>();
        }

        public static Dictionary<string, Dictionary<string, string>> Controls => controlsData.ToObject<Dictionary<string, Dictionary<string, string>>>();
        private static readonly JObject controlsData = JsonStreamer.ReadControls();


        public static JObject Configurations { get => configurationsData; set => JsonStreamer.WriteConfigurations(value); }
        private static readonly JObject configurationsData = JsonStreamer.ReadConfigurations();


        public static JObject Config { get => configData; set => JsonStreamer.WriteConfigurations(value); }
        private static readonly JObject configData = JsonStreamer.ReadConfig();


        public static string GetTranslate(string text)
        {
            return translateData[text] is not null ? translateData[text].Value<string>() : text;
        }

        private static readonly JObject translateData = JsonStreamer.ReadTranslate();

        public static JObject Egg { get; } = JsonStreamer.ReadEgg();
    }
}
