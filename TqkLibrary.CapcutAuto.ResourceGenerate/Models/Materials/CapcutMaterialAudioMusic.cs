using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public sealed class CapcutMaterialAudioMusic : CapcutMaterialAudio
    {
        [JsonConstructor]
        private CapcutMaterialAudioMusic(JObject jObject) : base(MaterialType.music, jObject)
        {

        }
        public static CapcutMaterialAudioMusic Create(string json)
        {
            return JsonConvert.DeserializeObject<CapcutMaterialAudioMusic>(json, Singleton.JsonSerializerSettings)!;
        }
    }
}
