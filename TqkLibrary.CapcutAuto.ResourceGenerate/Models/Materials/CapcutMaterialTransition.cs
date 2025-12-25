using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public sealed class CapcutMaterialTransition : CapcutMaterial
    {
        [JsonConstructor]
        private CapcutMaterialTransition()
        {
            this.Type = MaterialType.transition;
        }

        [JsonProperty("duration")]
        public required TimeSpan Duration { get; set; }





        [JsonProperty("category_id")]
        public required string CategoryId { get; init; }

        [JsonProperty("category_name")]
        public required string CategoryName { get; init; }

        [JsonProperty("effect_id")]
        public required string EffectId { get; init; }

        [JsonProperty("is_ai_transition")]
        public required bool IsAiTransition { get; init; }

        [JsonProperty("is_overlap")]
        public required bool IsOverlap { get; init; }

        [JsonProperty("name")]
        public required string Name { get; init; }

        [JsonProperty("path")]
        [JsonConverter(typeof(CapcutPathConverter))]
        public required string Path { get; init; }

        [JsonProperty("platform")]
        public required string Platform { get; init; }

        [JsonProperty("request_id")]
        public required string RequestId { get; init; }

        [JsonProperty("resource_id")]
        public required string ResourceId { get; init; }

        [JsonProperty("source_platform")]
        public required int SourcePlatform { get; init; }

        [JsonProperty("task_id")]
        public required string TaskId { get; init; }

        [JsonProperty("third_resource_id")]
        public required string ThirdResourceId { get; init; }

        [JsonProperty("video_path")]
        public required string VideoPath { get; init; }


        public static CapcutMaterialTransition Parse(string json_text)
        {
            return JsonConvert.DeserializeObject<CapcutMaterialTransition>(json_text, Singleton.JsonSerializerSettings)!;
        }
    }
}
