using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.Models
{
    public abstract class CapcutId
    {
        [JsonProperty("id", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public Guid Id { get; internal set; } = Guid.NewGuid();
    }
}
