using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks.Segments
{
    public sealed class CapcutSegmentSticker : CapcutSegment
    {
        [JsonProperty("clip")]
        public SegmentClip Clip { get; init; } = new();


        [JsonIgnore]
        public required CapcutMaterialSticker MaterialSticker { get; init; }
        protected override CapcutId GetMaterial()
        {
            return MaterialSticker;
        }

        [JsonIgnore]
        public CapcutMaterialAnimationSticker MaterialAnimation { get; set; } = new();




        protected override IEnumerable<CapcutId> GetExtraMaterialRefs()
        {
            CapcutId?[] capcutIds = new CapcutId?[]
            {
                MaterialAnimation,
            };
            foreach (CapcutId capcutId in capcutIds.Where(x => x is not null)!)
            {
                yield return capcutId;
            }
        }

    }
}
