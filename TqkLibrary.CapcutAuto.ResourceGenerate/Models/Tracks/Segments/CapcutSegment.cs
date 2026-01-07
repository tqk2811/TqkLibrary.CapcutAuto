using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks.Segments
{
    public abstract class CapcutSegment : CapcutId
    {
        [JsonProperty("extra_material_refs")]
        public IEnumerable<Guid> ExtraMaterialRefs
        {
            get { return GetExtraMaterialRefs().Select(x => x.Id).ToArray(); }
        }

        [JsonProperty("material_id")]
        public Guid MaterialId
        {
            get { return GetMaterial().Id; }
        }

        [JsonIgnore]
        public CapcutMaterialSpeed MaterialSpeed { get; } = new();

        [JsonProperty("speed")]
        public double Speed
        {
            get
            {
                return SourceTimerange.Duration / TargetTimerange.Duration;
            }
        }

        [JsonProperty("render_index")]
        public long RenderIndex { get; set; } = 0;

        [JsonProperty("source_timerange")]
        public required CapcutTimeRange SourceTimerange { get; set; }

        [JsonProperty("target_timerange")]
        public required CapcutTimeRange TargetTimerange { get; set; }

        [JsonProperty("volume")]
        public required double Volume { get; set; }

        [JsonProperty("track_render_index")]
        public int TrackRenderIndex { get; internal set; }

        protected abstract IEnumerable<CapcutId> GetExtraMaterialRefs();
        protected abstract CapcutId GetMaterial();
    }
}
