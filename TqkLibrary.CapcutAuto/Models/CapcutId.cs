using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.Models
{
    public abstract class CapcutId
    {
        [JsonProperty("id")]
        public Guid Id { get; } = Guid.NewGuid();
    }
}
