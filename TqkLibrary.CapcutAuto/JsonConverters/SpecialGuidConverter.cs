using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.JsonConverters
{
    public class SpecialGuidConverter : JsonConverter<Guid>
    {
        public override void WriteJson(
            JsonWriter writer, 
            Guid value, 
            JsonSerializer serializer
            )
        {
            string guidString = value.ToString("D");
            string[] parts = guidString.Split('-');
            for (int i = 0; i < parts.Length; i++)
            {
                if (i == 2)
                    parts[i] = parts[i].ToLower();
                else
                    parts[i] = parts[i].ToUpper();
            }
            writer.WriteValue(string.Join("-", parts));
        }

        public override Guid ReadJson(
            JsonReader reader, 
            Type objectType, 
            Guid existingValue, 
            bool hasExistingValue, 
            JsonSerializer serializer
            )
        {
            return Guid.Parse(reader.Value!.ToString()!);
        }
    }
}
