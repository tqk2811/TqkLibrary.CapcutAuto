using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters
{
    public class CapcutDateTimeConverter : JsonConverter<DateTime>
    {
        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            long unixTime = ((DateTimeOffset)value).ToUnixTimeSeconds();
            writer.WriteValue(unixTime);
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return DateTime.MinValue;
            long unixTime = Convert.ToInt64(reader.Value);
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
        }
    }
}
