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
            return isSubclass;
            //if (!isSubclass) return false;
            //var constructor = objectType.GetConstructor(
            //    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            //    null,
            //    new[] { typeof(JObject) },
            //    null
            //    );
            //return constructor != null;
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

            BaseCapcut instance;
            if (constructor is null)
            {
                constructor = objectType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    null,
                    new Type[0] { },
                    null
                )!;
                instance = (BaseCapcut)constructor.Invoke(new object[] {  });
            }
            else
            {
                instance = (BaseCapcut)constructor.Invoke(new object[] { temp });
            }


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

            var tempSettings = new JsonSerializerSettings
            {
                Formatting = serializer.Formatting,
                DateFormatHandling = serializer.DateFormatHandling,
                DateTimeZoneHandling = serializer.DateTimeZoneHandling,
                NullValueHandling = serializer.NullValueHandling,
                ContractResolver = serializer.ContractResolver
            };
            foreach (var conv in serializer.Converters)
            {
                if (conv is not BaseCapcutConverter)
                    tempSettings.Converters.Add(conv);
            }
            JsonSerializer internalSerializer = JsonSerializer.Create(tempSettings);

            JObject classData = JObject.FromObject(value, internalSerializer);
            result.Merge(classData, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Replace,
                MergeNullValueHandling = MergeNullValueHandling.Merge,
            });
            result.WriteTo(writer);
        }
    }
}
