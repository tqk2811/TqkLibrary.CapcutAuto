using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.JsonConverters
{
    public class CapcutPathConverter : JsonConverter<string>
    {
        public override string? ReadJson(
            JsonReader reader,
            Type objectType,
            string? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
            )
        {
            string? rawPath = reader.Value?.ToString();

            if (string.IsNullOrEmpty(rawPath))
                return null;
            string expandedPath = Environment.ExpandEnvironmentVariables(rawPath);
            return expandedPath.Replace('\\', '/');
        }

        public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
