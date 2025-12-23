using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TqkLibrary.CapcutAuto.Models
{
    public abstract class CapcutId : BaseCapcut
    {
        protected CapcutId()
        {

        }
        protected CapcutId(JObject jObject) : base(jObject)
        {
        }



        [JsonProperty("id", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public Guid Id { get; internal set; } = Guid.NewGuid();


    }
}
