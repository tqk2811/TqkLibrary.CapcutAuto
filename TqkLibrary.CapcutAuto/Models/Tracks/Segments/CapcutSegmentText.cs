using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.Models.Materials;

namespace TqkLibrary.CapcutAuto.Models.Tracks.Segments
{
    public sealed class CapcutSegmentText : CapcutSegment
    {
        [JsonIgnore]
        public required CapcutMaterialText MaterialText { get; set; }
        protected override CapcutId GetMaterial()
        {
            return MaterialText;
        }




        [JsonIgnore]
        public CapcutMaterialAnimation MaterialAnimation { get; set; } = new();

        [JsonIgnore]
        public CapcutMaterialEffect? MaterialEffect { get; set; }

        protected override IEnumerable<CapcutId> GetExtraMaterialRefs()
        {
            CapcutId?[] capcutIds = new CapcutId?[]
            {
                 MaterialAnimation,//animation
                 MaterialEffect,//effect
            };
            foreach (CapcutId capcutId in capcutIds.Where(x => x is not null)!)
            {
                yield return capcutId;
            }
        }
    }
}
