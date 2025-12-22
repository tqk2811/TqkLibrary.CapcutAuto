using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TqkLibrary.CapcutAuto.Models
{
    public abstract class CapcutId
    {
        readonly JObject? _jObject;
        protected CapcutId()
        {

        }
        protected CapcutId(JObject jObject)
        {
            this._jObject = jObject;
        }



        [JsonProperty("id", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public Guid Id { get; internal set; } = Guid.NewGuid();


        public JObject? GetRawJObject()
        {
            return _jObject;
        }
        public string GetJsonString()
        {
            return JsonConvert.SerializeObject(this, Singleton.JsonSerializerSettings);
        }
    }
}
