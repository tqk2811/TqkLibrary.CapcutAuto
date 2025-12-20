using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.Models.Tracks.Segments
{
    public abstract class CapcutSegment : CapcutId
    {
        [JsonProperty("extra_material_refs")]
        public IEnumerable<Guid> ExtraMaterialRefs => GetExtraMaterialRefs().Select(x => x.Id).ToArray();

        [JsonProperty("material_id")]
        public Guid MaterialId => GetMaterial().Id;

        [JsonProperty("speed")]
        public double Speed { get; set; } = 1.0;

        [JsonProperty("render_index")]
        public long RenderIndex { get; set; } = 0;

        [JsonProperty("source_timerange")]
        public CapcutTimeRange? SourceTimerange { get; set; }

        [JsonProperty("target_timerange")]
        public required CapcutTimeRange TargetTimerange { get; set; }

        [JsonProperty("volume")]
        public required double Volume { get; set; }

        protected abstract IEnumerable<CapcutId> GetExtraMaterialRefs();
        protected abstract CapcutId GetMaterial();
    }
}
