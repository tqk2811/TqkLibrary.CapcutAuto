using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.Models.Materials;

namespace TqkLibrary.CapcutAuto.Models.Tracks.Segments
{
    public sealed class CapcutSegmentText : CapcutSegment
    {
        [JsonIgnore]
        public CapcutMaterialAnimation MaterialAnimation { get; set; } = new();

        [JsonIgnore]
        public CapcutMaterialEffect? MaterialEffect { get; set; }

        [JsonIgnore]
        public CapcutMaterialText MaterialText { get; set; } = new();

        protected override IEnumerable<CapcutId> GetExtraMaterialRefs()
        {
            yield return MaterialAnimation;
            if (MaterialEffect != null) yield return MaterialEffect;
        }
        protected override CapcutId GetMaterial()
        {
            return MaterialText;
        }
    }
}
