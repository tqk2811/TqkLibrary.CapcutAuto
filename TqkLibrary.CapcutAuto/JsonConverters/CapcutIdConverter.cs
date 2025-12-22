using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using TqkLibrary.CapcutAuto.Models;

namespace TqkLibrary.CapcutAuto.JsonConverters
{
    public class CapcutIdConverter<TCapcutId> : JsonConverter<TCapcutId>
        where TCapcutId : CapcutId
    {
        public override TCapcutId? ReadJson(
            JsonReader reader,
            Type objectType,
            TCapcutId? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
            )
        {
            JObject temp = JObject.Load(reader);

            var constructor = typeof(TCapcutId).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null,
                new[] { typeof(JObject) },
                null
                ); 

            TCapcutId instance;
            if (constructor != null)
            {
                instance = (TCapcutId)constructor.Invoke(new object[] { temp });
            }
            else
            {
                instance = (TCapcutId)Activator.CreateInstance(typeof(TCapcutId), true)!;
            }
            serializer.Populate(temp.CreateReader(), instance);
            return instance;
        }


        public override void WriteJson(
            JsonWriter writer,
            TCapcutId? value,
            JsonSerializer serializer
            )
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            var instance = (CapcutId)value!;
            JObject result = instance.GetRawJObject() != null
                ? (JObject)instance.GetRawJObject()!.DeepClone()
                : new JObject();

            var contract = serializer.ContractResolver.ResolveContract(typeof(TCapcutId));
            var oldConverter = contract.Converter;
            contract.Converter = null;
            try
            {
                JObject classData = JObject.FromObject(value, serializer);
                result.Merge(classData, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Replace,
                    MergeNullValueHandling = MergeNullValueHandling.Merge,
                });
                result.WriteTo(writer);
            }
            finally
            {
                contract.Converter = oldConverter;
            }
        }
    }
}
