using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.Models.Materials;

namespace TqkLibrary.CapcutAuto.Models.Tracks.Segments
{
    public sealed class CapcutSegmentVideo : CapcutSegment
    {
        [JsonIgnore]
        public CapcutMaterialSpeed MaterialSpeed { get; set; } = new();

        /// <summary>
        /// Transition to next video
        /// </summary>
        [JsonIgnore]
        public CapcutMaterialTransition? MaterialTransition { get; set; }

        [JsonIgnore]
        public required CapcutMaterialVideo MaterialVideo { get; set; }


        protected override IEnumerable<CapcutId> GetExtraMaterialRefs()
        {
            yield return MaterialSpeed;
            if (MaterialTransition is not null)
                yield return MaterialTransition;
        }
        protected override CapcutId GetMaterial()
        {
            return MaterialVideo;
        }
    }
}
