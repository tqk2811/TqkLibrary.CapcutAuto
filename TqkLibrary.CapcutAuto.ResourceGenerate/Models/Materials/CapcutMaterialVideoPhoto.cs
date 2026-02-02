using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public sealed class CapcutMaterialVideoPhoto : CapcutMaterialVideoBase
    {
        private CapcutMaterialVideoPhoto(JObject jObject) : base(jObject)
        {
            this.Type = MaterialType.photo;
        }

        internal static CapcutMaterialVideoPhoto Create(DraftMetaInfo.DraftMaterialValuePhoto draftMaterialValuePhoto)
        {
            string json = Extensions.GetEmbeddedResourceString("Materials.Photo.json");
            JObject jObject = JObject.Parse(json);
            return new CapcutMaterialVideoPhoto(jObject)
            {
                Duration = draftMaterialValuePhoto.Duration,
                HasAudio = false,
                Height = draftMaterialValuePhoto.Height,
                Width = draftMaterialValuePhoto.Width,
                MaterialName = draftMaterialValuePhoto.ExtraInfo,
                Path = draftMaterialValuePhoto.FilePath,
                LocalMaterialId = draftMaterialValuePhoto.Id,
            };
        }
    }
}
