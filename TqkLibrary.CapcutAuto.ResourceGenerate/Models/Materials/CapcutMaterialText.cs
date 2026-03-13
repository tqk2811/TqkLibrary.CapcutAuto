using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public partial class CapcutMaterialText : CapcutMaterial
    {
        [JsonConstructor]
        private CapcutMaterialText(JObject jObject) : base(jObject)
        {
            Type = MaterialType.text;
        }

        [JsonIgnore]
        public _ContentHelper ContentHelper { get; set; } = new _ContentHelper();

        [JsonProperty("font_source_platform")]
        public int FontSourcePlatform { get; set; } = 0;

        [JsonProperty("font_resource_id")]
        public string FontResourceId { get; set; } = string.Empty;


        //[JsonProperty("add_type")]
        //public int AddType { get; set; } = 0;


        [JsonProperty("alignment")]
        int _Alignment { get; set; } = 1;
        [JsonIgnore]
        public MaterialTextAlignment Alignment
        {
            get => (MaterialTextAlignment)_Alignment;
            set => _Alignment = (int)value;
        }


        [JsonProperty("check_flag")]
        int _check_flag { get; set; }
        [JsonIgnore]
        public TextCheckFlag CheckFlag
        {
            get { return (TextCheckFlag)_check_flag; }
            set
            {
                if (value.HasFlag(TextCheckFlag.Curve) && value.HasFlag(TextCheckFlag.Background))
                    throw new InvalidOperationException($"Can't enable {nameof(TextCheckFlag.Curve)} and {TextCheckFlag.Background} sametime");

                _check_flag = (int)value;

                TextCurve.Enable = value.HasFlag(TextCheckFlag.Curve);
                HasShadow = value.HasFlag(TextCheckFlag.Shadow);
                ContentHelper.UpdateFrom(this);
            }
        }

        #region Blend

        /// <summary>
        /// <see cref="TextCheckFlag.Blend"/>
        /// </summary>
        [JsonProperty("global_alpha")]
        public double GlobalAlpha { get; set; } = 1.0;

        #endregion


        #region Background

        [JsonProperty("background_style")]
        public int BackgroundStyle { get; set; }//1

        [JsonProperty("background_alpha")]
        public double BackgroundAlpha { get; set; } = 1.0;

        [JsonProperty("background_color")]
        string _background_color;//rrggbb
        [JsonIgnore]
        public Color BackgroundColor
        {
            get { return ColorTranslator.FromHtml(_background_color); }
            set { _background_color = ColorTranslator.ToHtml(value); }
        }

        [JsonProperty("background_height")]
        public double BackgroundHeight { get; set; }//14% = 0.14

        [JsonProperty("background_width")]
        public double BackgroundWidth { get; set; }//14% = 0.14

        [JsonProperty("background_round_radius")]
        public double BackgroundRoundRadius { get; set; }//0-100% ex: 0.01

        #endregion


        #region Curve 

        [JsonProperty("text_curve")]
        public TextCurve TextCurve { get; set; } = new TextCurve();

        #endregion

        #region Shadow
        /*
"has_shadow": false,
"shadow_alpha": 0.8999999761581421,
"shadow_angle": -45.0,
"shadow_color": "#000000",
"shadow_distance": 5.0,
"shadow_point": {
    "x": 0.6363961030678928,
    "y": -0.6363961030678928
},
"shadow_smoothing": 0.45000001788139343,
         */
        bool _HasShadow = false;
        [JsonProperty("has_shadow")]
        public bool HasShadow
        {
            get { return _HasShadow; }
            set
            {
                _HasShadow = value;
                //ContentHelper.Styles.First().sh
            }
        }

        [JsonProperty("shadow_alpha")]
        public double shadow_alpha { get; set; }

        [JsonProperty("shadow_angle")]
        public double shadow_angle { get; set; }

        [JsonProperty("shadow_color")]
        string _shadow_color;//html
        [JsonIgnore]
        public Color ShadowColor
        {
            get { return ColorTranslator.FromHtml(_shadow_color); }
            set { _shadow_color = ColorTranslator.ToHtml(value); }
        }

        [JsonProperty("shadow_distance")]
        public double ShadowDistance { get; set; }
        #endregion


        [JsonProperty("text_color")]
        string _text_color;//rrggbb
        [JsonIgnore]
        public Color TextColor
        {
            get { return ColorTranslator.FromHtml(_text_color); }
            set
            {
                _text_color = ColorTranslator.ToHtml(value);
                ContentHelper.UpdateFrom(this);
            }
        }

        [JsonProperty("border_color")]
        string _border_color;
        [JsonIgnore]
        public Color BorderColor
        {
            get { return ColorTranslator.FromHtml(_border_color); }
            set
            {
                _border_color = ColorTranslator.ToHtml(value);
                ContentHelper.UpdateFrom(this);
            }
        }

        [JsonProperty("use_effect_default_color")]
        public bool UseEffectDefaultColor { get; set; }

        [JsonProperty("content")]
        private string _Content
        {
            get => JsonConvert.SerializeObject(ContentHelper, Singleton.JsonSerializerSettings);
            set => ContentHelper = JsonConvert.DeserializeObject<_ContentHelper>(value, Singleton.JsonSerializerSettings)!;
        }

        [JsonProperty("font_path")]
        [JsonConverter(typeof(CapcutPathConverter))]
        public required string FontPath { get; set; }

        [JsonProperty("fonts")]
        public List<FontResource> Fonts { get; } = new();

        public void SetText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            this.ContentHelper.Text = text;
            this.ContentHelper.Styles.First().Range = new() { 0, text.Length };
        }
        public void SetFontSize(int size)
        {
            if (size < 1) throw new InvalidOperationException("Font size should larger than 1");
            this.ContentHelper.Styles.First().Size = size;
        }
        public void SetFont(FontResource fontResource)
        {
            if (fontResource is null) return;
            FontPath = fontResource.Path;
            FontSourcePlatform = 1;
            FontResourceId = fontResource.ResourceId;
            Fonts.Clear();
            Fonts.Add(fontResource);
            foreach (var item in ContentHelper.Styles)
            {
                if (item.Font is not null)
                {
                    item.Font.Path = fontResource.Path;
                    item.Font.Id = fontResource.ResourceId;
                }
            }
        }

        public static CapcutMaterialText Parse(string json)
        {
            return JsonConvert.DeserializeObject<CapcutMaterialText>(json, Singleton.JsonSerializerSettings)!;
        }

        public static CapcutMaterialText CreateDefault()
        {
            string json = Extensions.GetEmbeddedResourceString("Materials.text.json");
            return Parse(json);
        }
    }
}
