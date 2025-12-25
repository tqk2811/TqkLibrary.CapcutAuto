using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models;

namespace TqkLibrary.CapcutAuto.ResourceGenerate
{
    public static class Extensions
    {
        public static T SetProp<T>(this T obj, Action<T> action)
        {
            action.Invoke(obj);
            return obj;
        }
        public static T Clone<T>(this T t) where T : BaseCapcut
        {
            string text_json = JsonConvert.SerializeObject(t, Singleton.JsonSerializerSettings);
            T clone = JsonConvert.DeserializeObject<T>(text_json, Singleton.JsonSerializerSettings)!;
            clone.SetRawJObject(t.GetRawJObject());
            return clone;
        }
        public static T CloneWithRandomId<T>(this T t) where T : CapcutId
        {
            string text_json = JsonConvert.SerializeObject(t, Singleton.JsonSerializerSettings);
            T clone = JsonConvert.DeserializeObject<T>(text_json, Singleton.JsonSerializerSettings)!;
            clone.Id = Guid.NewGuid();
            clone.SetRawJObject(t.GetRawJObject());
            return clone;
        }
        public static string GetCapcutJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Singleton.JsonSerializerSettings);
        }

        public static string GetEmbeddedResourceString(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"{assembly.GetName().Name}.EmbeddedResources.{name}";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
            {
                using StreamReader streamReader = new StreamReader(stream);
                return streamReader.ReadToEnd();
            }
        }
        public static Stream GetEmbeddedResourceStream(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"{assembly.GetName().Name}.EmbeddedResources.{name}";
            return assembly.GetManifestResourceStream(resourceName)!;
        }
    }
}
