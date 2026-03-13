using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public partial class CapcutMaterialText
    {
        public class _ContentHelper
        {
            public void UpdateFrom(CapcutMaterialText capcutMaterialText)
            {
                var style = Styles?.FirstOrDefault();
                if (style is null)
                    return;

                if (style?.Fill?.Content?.Solid is not null)
                {
                    style.Fill.Content.Solid!.Color = new()
                    {
                        capcutMaterialText.TextColor.R/255.0,
                        capcutMaterialText.TextColor.G/255.0,
                        capcutMaterialText.TextColor.B/255.0,
                    };
                }
            }


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
                public _EffectStyle? EffectStyle { get; set; }

                [JsonProperty("strokes")]
                public List<_Fill>? Strokes { get; set; }

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
                [JsonProperty("alpha")]
                public double? Alpha { get; set; }

                [JsonProperty("content")]
                public required _Content Content { get; set; }
            }
            public class _Content
            {
                [JsonProperty("texture")]
                public required _Texture Texture { get; set; }

                [JsonProperty("render_type")]
                public required string RenderType { get; set; }

                [JsonProperty("solid")]
                public _Solid? Solid { get; set; }
            }
            public class _Texture
            {
                [JsonProperty("range")]
                public required int Range { get; set; }

                [JsonProperty("path")]
                public required string Path { get; set; }
            }
            public class _Solid
            {
                [JsonProperty("alpha")]
                public double? Alpha { get; set; }

                [JsonProperty("color")]
                public required List<double> Color { get; set; }
            }
        }
    }
}
