using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.Enums;
using TqkLibrary.CapcutAuto.JsonConverters;

namespace TqkLibrary.CapcutAuto.Models.Resources
{
    public class CapcutAnimation : BaseCapcut
    {
        [JsonConstructor]
        private CapcutAnimation()
        {

        }


        [JsonProperty("category_id")]
        public required string CategoryId { get; init; }

        [JsonProperty("category_name")]
        public required string CategoryName { get; init; }

        [JsonProperty("id")]
        public required string Id { get; init; }

        [JsonProperty("material_type")]
        public required string MaterialType { get; init; }

        [JsonProperty("name")]
        public required string Name { get; init; }

        [JsonProperty("panel")]
        public string? Panel { get; init; }

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

        [JsonProperty("third_resource_id")]
        public required string ThirdResourceId { get; init; }




        [JsonProperty("type")]
        public required AnimationType Type { get; init; }



        [JsonProperty("start")]
        public TimeSpan Start { get; set; } = TimeSpan.Zero;

        [JsonProperty("duration")]
        public required TimeSpan Duration { get; set; }


        public static CapcutAnimation Parse(string json_text)
        {
            return JsonConvert.DeserializeObject<CapcutAnimation>(json_text, Singleton.JsonSerializerSettings)!;
        }
    }
}
