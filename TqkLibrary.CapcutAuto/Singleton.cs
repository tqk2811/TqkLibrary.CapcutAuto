using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TqkLibrary.CapcutAuto.JsonConverters;

namespace TqkLibrary.CapcutAuto
{
    public static class Singleton
    {
        static internal readonly JsonSerializerSettings JsonSerializerSettings;
        static Singleton()
        {
            JsonSerializerSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                Converters =
                [
                    new CapcutTimeSpanConverter(),
                    new StringEnumConverter(),
                    new SpecialGuidConverter(),
                    new BaseCapcutConverter(),
                    new CapcutDateTimeConverter(),
                ]
            };
        }
    }
}
