using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks.Segments
{
    public sealed class CapcutSegmentText : CapcutSegment
    {
        [JsonIgnore]
        public required CapcutMaterialText MaterialText { get; set; }
        protected override CapcutId GetMaterial()
        {
            return MaterialText;
        }

        [JsonProperty("clip")]
        public SegmentClip Clip { get; init; } = new();


        [JsonIgnore]
        public CapcutMaterialEffectBloom? MaterialEffectBloom { get; set; }

        [JsonIgnore]
        public CapcutMaterialAnimationText MaterialAnimation { get; set; } = new();

        [JsonIgnore]
        public CapcutMaterialEffectTextEffect? MaterialEffectTextEffect { get; set; }

        [JsonIgnore]
        public CapcutMaterialEffectTextShape? MaterialEffectTextShape { get; set; }

        [JsonIgnore]
        public IEnumerable<CapcutMaterialEffect> CapcutMaterialEffects
        {
            get
            {
                if (MaterialEffectBloom is not null)
                    yield return MaterialEffectBloom;
                if (MaterialEffectTextEffect is not null)
                    yield return MaterialEffectTextEffect;
                if (MaterialEffectTextShape is not null)
                    yield return MaterialEffectTextShape;
            }
        }


        protected override IEnumerable<CapcutId> GetExtraMaterialRefs()
        {
            CapcutId?[] capcutIds = new CapcutId?[]
            {
                 MaterialAnimation,//animation
                 MaterialEffectTextEffect,//effect
                 MaterialEffectTextShape,//effect
                 MaterialEffectBloom,
            };
            foreach (CapcutId capcutId in capcutIds.Where(x => x is not null)!)
            {
                yield return capcutId;
            }
        }
    }
}
