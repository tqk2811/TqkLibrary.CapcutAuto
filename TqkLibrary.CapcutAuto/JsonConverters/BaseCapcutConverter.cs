using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using TqkLibrary.CapcutAuto.Models;

namespace TqkLibrary.CapcutAuto.JsonConverters
{
    public class BaseCapcutConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            bool isSubclass = objectType.IsSubclassOf(typeof(BaseCapcut));
            if (!isSubclass) return false;
            var constructor = objectType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[] { typeof(JObject) },
                null
                );
            return constructor != null;
        }

        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
            )
        {
            JObject temp = JObject.Load(reader);

            var constructor = objectType.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null,
                new[] { typeof(JObject) },
                null
                )!;

            BaseCapcut instance = (BaseCapcut)constructor.Invoke(new object[] { temp });
            serializer.Populate(temp.CreateReader(), instance);
            return instance;
        }


        public override void WriteJson(
            JsonWriter writer,
            object? value,
            JsonSerializer serializer
            )
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            var instance = (BaseCapcut)value!;
            JObject result = instance.GetRawJObject() != null
                ? (JObject)instance.GetRawJObject()!.DeepClone()
                : new JObject();

            int index = serializer.Converters.IndexOf(this);
            serializer.Converters.RemoveAt(index);
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
                serializer.Converters.Insert(index, this);
            }
        }
    }
}
