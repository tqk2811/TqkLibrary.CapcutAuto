using FFMpegCore;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public sealed class CapcutMaterialVideoVideo : CapcutMaterialVideoBase
    {
        private CapcutMaterialVideoVideo(JObject jObject) : base(jObject)
        {
            this.Type = MaterialType.video;
        }

        internal static CapcutMaterialVideoVideo Create(DraftMetaInfo.DraftMaterialValueVideo draftMaterialValueVideo)
        {
            string json = Extensions.GetEmbeddedResourceString("Materials.Video.json");
            JObject jObject = JObject.Parse(json);
            return new CapcutMaterialVideoVideo(jObject)
            {
                Duration = draftMaterialValueVideo.Duration,
                HasAudio = draftMaterialValueVideo.HasAudio,
                Height = draftMaterialValueVideo.Height,
                Width = draftMaterialValueVideo.Width,
                MaterialName = draftMaterialValueVideo.ExtraInfo,
                Path = draftMaterialValueVideo.FilePath,
                LocalMaterialId = draftMaterialValueVideo.Id,
            };
        }
    }
}
