using Newtonsoft.Json;
using System.Collections;
using TqkLibrary.CapcutAuto.Enums;
using TqkLibrary.CapcutAuto.Models.Tracks.Segments;

namespace TqkLibrary.CapcutAuto.Models.Tracks
{
    public abstract class CapcutTrack : CapcutId, IEnumerable
    {
        protected readonly List<CapcutSegment> _segments = new();



        [JsonProperty("attribute")]
        public int Attribute { get; set; }

        [JsonProperty("flag")]
        public int Flag { get; set; }

        [JsonProperty("is_default_name")]
        public bool IsDefaultName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("segments")]
        public IReadOnlyList<CapcutSegment> Segments => _segments;

        [JsonProperty("type")]
        public TrackType Type { get; protected set; }

        public IEnumerator GetEnumerator()
        {
            return _segments.GetEnumerator();
        }
    }
}
