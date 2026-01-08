using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters
{
    public class SpecialGuidConverter : JsonConverter<Guid>
    {
        public override void WriteJson(
            JsonWriter writer, 
            Guid value, 
            JsonSerializer serializer
            )
        {
            writer.WriteValue(GuidToString(value));
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


        public static string GuidToString(Guid guid)
        {
            string guidString = guid.ToString("D");
            string[] parts = guidString.Split('-');
            for (int i = 0; i < parts.Length; i++)
            {
                if (i == 2)
                    parts[i] = parts[i].ToLower();
                else
                    parts[i] = parts[i].ToUpper();
            }
            return string.Join("-", parts);
        }
    }
}
