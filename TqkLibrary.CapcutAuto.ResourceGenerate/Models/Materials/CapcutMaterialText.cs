using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public class CapcutMaterialText : CapcutMaterial
    {
        [JsonConstructor]
        private CapcutMaterialText(JObject jObject) : base(jObject)
        {
            Type = MaterialType.text;
        }

        [JsonIgnore]
        public _ContentHelper ContentHelper { get; set; } = new _ContentHelper();



        [JsonProperty("add_type")]
        public int AddType { get; set; } = 0;

        [JsonProperty("alignment")]
        public int Alignment { get; set; } = 1;

        [JsonProperty("content")]
        private string _Content
        {
            get => JsonConvert.SerializeObject(ContentHelper, Singleton.JsonSerializerSettings);
            set => ContentHelper = JsonConvert.DeserializeObject<_ContentHelper>(value, Singleton.JsonSerializerSettings)!;
        }

        [JsonProperty("font_path")]
        [JsonConverter(typeof(CapcutPathConverter))]
        public required string FontPath { get; set; }



        public static CapcutMaterialText Parse(string json)
        {
            return JsonConvert.DeserializeObject<CapcutMaterialText>(json, Singleton.JsonSerializerSettings)!;
        }

        public static CapcutMaterialText CreateDefault()
        {
            string json = Extensions.GetEmbeddedResourceString("Materials.text.json");
            return Parse(json);
        }

        public class _ContentHelper
        {
            [JsonProperty("text")]
            public string Text { get; set; } = string.Empty;

            [JsonProperty("styles")]
            public List<_Style> Styles { get; set; } = new();

            public class _Style
            {
                [JsonProperty("fill")]
                public required _Fill Fill { get; set; }

                [JsonProperty("font")]
                public required _Font Font { get; set; }

                [JsonProperty("size")]
                public required double Size { get; set; }

                [JsonProperty("effectStyle")]
                public required _EffectStyle EffectStyle { get; set; }

                [JsonProperty("range")]
                public required List<int> Range { get; set; }
            }
            public class _Font
            {
                [JsonProperty("path")]
                [JsonConverter(typeof(CapcutPathConverter))]
                public required string Path { get; set; }

                [JsonProperty("id")]
                public required string Id { get; set; }
            }
            public class _EffectStyle
            {
                [JsonProperty("path")]
                [JsonConverter(typeof(CapcutPathConverter))]
                public required string Path { get; set; }

                [JsonProperty("id")]
                public required string Id { get; set; }
            }
            public class _Fill
            {
                [JsonProperty("content")]
                public required _Content Content { get; set; }
            }
            public class _Content
            {
                [JsonProperty("texture")]
                public required _Texture Texture { get; set; }

                [JsonProperty("render_type")]
                public required string RenderType { get; set; }
            }
            public class _Texture
            {
                [JsonProperty("range")]
                public required int Range { get; set; }

                [JsonProperty("path")]
                public required string Path { get; set; }
            }
        }
    }
}
