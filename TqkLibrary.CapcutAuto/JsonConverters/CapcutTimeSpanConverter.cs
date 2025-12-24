using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.JsonConverters
{
    public class CapcutTimeSpanConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan) || objectType == typeof(TimeSpan?);
        }
        public override void WriteJson(
            JsonWriter writer,
            object? value,
            JsonSerializer serializer
            )
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            TimeSpan ts = (TimeSpan)value;            
            writer.WriteValue((long)ts.TotalMicroseconds);
        }
        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
            )
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (long.TryParse(reader.Value?.ToString(), out long microValue))
            {
                return TimeSpan.FromMicroseconds(microValue);
            }
            return null;
        }
    }
}
