using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.CapcutAuto.Models;
using TqkLibrary.CapcutAuto.Models.Materials;

namespace TqkLibrary.CapcutAuto
{
    public static class Extensions
    {
        public static T Clone<T>(this T t) where T : CapcutMaterial
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

        public static string GetEmbeddedResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"{assembly.GetName().Name}.EmbeddedResources.{name}";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
            {
                using StreamReader streamReader = new StreamReader(stream);
                return streamReader.ReadToEnd();
            }
        }
    }
}
