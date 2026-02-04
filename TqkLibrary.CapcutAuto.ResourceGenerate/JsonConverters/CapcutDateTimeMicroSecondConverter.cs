using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters
{
    public class CapcutDateTimeMicroSecondConverter : JsonConverter<DateTime>
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            long ticks = value.ToUniversalTime().Ticks - _epoch.Ticks; 
            long microseconds = ticks / 10;
            writer.WriteValue(microseconds);
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return DateTime.MinValue;
            long microseconds = Convert.ToInt64(reader.Value); 
            return _epoch.AddTicks(microseconds * 10).ToLocalTime();
        }
    }
}
