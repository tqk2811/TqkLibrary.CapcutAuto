using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters
{
    public class SerializeOnlyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Guid);
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is Guid guid)
            {
                writer.WriteValue(SpecialGuidConverter.GuidToString(guid));
            }
            else
            {
                writer.WriteValue(value?.ToString());
            }
        }
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return existingValue;
        }
        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}
