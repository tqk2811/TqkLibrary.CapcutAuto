using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks.Segments
{
    public sealed class CapcutSegmentVideoVideo : CapcutSegmentVideoBase
    {
        [JsonIgnore]
        public override CapcutMaterialVideoBase MaterialVideoBase => MaterialVideo;

        [JsonIgnore]
        public required CapcutMaterialVideoVideo MaterialVideo { get; set; }
        protected override CapcutId GetMaterial()
        {
            return MaterialVideo;
        }
    }
}
