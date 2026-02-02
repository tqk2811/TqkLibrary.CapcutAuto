using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks.Segments
{
    public sealed class CapcutSegmentVideoPhoto : CapcutSegmentVideoBase
    {
        [JsonIgnore]
        public override CapcutMaterialVideoBase MaterialVideoBase => MaterialPhoto;

        [JsonIgnore]
        public required CapcutMaterialVideoPhoto MaterialPhoto { get; set; }
        protected override CapcutId GetMaterial()
        {
            return MaterialPhoto;
        }
    }
}
