using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return clone;
        }
    }
}
