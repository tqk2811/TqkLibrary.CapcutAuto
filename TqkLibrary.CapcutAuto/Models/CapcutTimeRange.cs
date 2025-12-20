using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.Models
{
    public class CapcutTimeRange
    {
        [JsonProperty("duration")]
        public TimeSpan Duration { get; set; }

        [JsonProperty("start")]
        public TimeSpan Start { get; set; }
    }
}
