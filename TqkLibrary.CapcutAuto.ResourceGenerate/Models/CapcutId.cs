using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models
{
    public abstract class CapcutId : BaseCapcut
    {
        protected CapcutId()
        {

        }
        protected CapcutId(JObject jObject) : base(jObject)
        {
        }



        [JsonProperty("id")]
        [JsonConverter(typeof(SerializeOnlyConverter))]
        public Guid Id { get; internal set; } = Guid.NewGuid();
    }
}
