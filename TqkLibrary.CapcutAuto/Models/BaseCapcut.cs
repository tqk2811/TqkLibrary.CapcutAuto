using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TqkLibrary.CapcutAuto.Models
{
    public abstract class BaseCapcut
    {
        JObject? _jObject = null;
        protected BaseCapcut()
        {

        }
        protected BaseCapcut(JObject jObject)
        {
            this._jObject = jObject;
        }
        public JObject? GetRawJObject()
        {
            return _jObject;
        }
        internal void SetRawJObject(JObject? jObject)
        {
            this._jObject = jObject;
        }
        public string GetJsonString()
        {
            return JsonConvert.SerializeObject(this, Singleton.JsonSerializerSettings);
        }
    }
}
