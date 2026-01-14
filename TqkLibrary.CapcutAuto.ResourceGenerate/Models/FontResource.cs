using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models
{
    public class FontResource : BaseCapcut
    {
        [JsonConstructor]
        private FontResource(JObject jObject) : base(jObject)
        {

        }

        [JsonProperty("path")]
        [JsonConverter(typeof(CapcutPathConverter))]
        public required string Path { get; set; }


        public static FontResource Parse(string json)
        {
            return JsonConvert.DeserializeObject<FontResource>(json, Singleton.JsonSerializerSettings)!;
        }
    }
}
