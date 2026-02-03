using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models
{
    public class DraftMetaInfo : BaseCapcut
    {
        public DraftMetaInfo() : base(JObject.Parse(Extensions.GetEmbeddedResourceString("draft_meta_info.json")))
        {

        }

        [JsonProperty("draft_id")]
        public Guid DraftId { get; } = Guid.NewGuid();

        [JsonProperty("draft_fold_path")]
        [JsonConverter(typeof(CapcutPathConverter))]
        public required string DraftFolderPath { get; set; }

        [JsonProperty("draft_cover")]
        public required string DraftCover { get; set; }

        [JsonProperty("draft_name")]
        public required string DraftName { get; set; }

        [JsonProperty("draft_root_path")]
        [JsonConverter(typeof(CapcutPathConverter))]
        public required string DraftRootPath { get; set; }

        [JsonProperty("draft_materials")]
        public List<DraftMaterial> DraftMaterials { get; } = new()
        {
            new DraftMaterial()
            {
            }
        };

        public class DraftMaterial
        {
            [JsonProperty("type")]
            public int Type { get; }//0 là file input

            [JsonProperty("value")]
            public List<DraftMaterialValue> Value { get; set; } = new();
        }
        public abstract class DraftMaterialValue
        {
            [JsonProperty("ai_group_type")]
            public string AiGroupType { get; } = string.Empty;

            [JsonProperty("create_time")]
            public DateTime CreateTime { get; } = DateTime.Now;

            [JsonProperty("duration")]
            public required TimeSpan Duration { get; init; }

            [JsonProperty("extra_info")]
            public required string ExtraInfo { get; init; }//file name

            [JsonProperty("file_Path")]
            [JsonConverter(typeof(CapcutPathConverter))]
            public required string FilePath { get; init; }

            [JsonProperty("id")]
            public Guid Id { get; } = Guid.NewGuid();

            [JsonProperty("import_time")]
            public DateTime ImportTime { get; } = DateTime.Now;

            [JsonProperty("import_time_ms")]
            public long ImportTimeMs => (((DateTimeOffset)ImportTime).UtcTicks - DateTimeOffset.UnixEpoch.Ticks) / 10;

            [JsonProperty("item_source")]
            public int ItemSource { get; } = 1;

            [JsonProperty("md5")]
            public string Md5 { get; } = string.Empty;

            [JsonProperty("metetype")]
            public MeteType Metetype { get; protected set; }

            [JsonProperty("roughcut_time_range")]
            public required TimeRange RoughcutTimeRange { get; init; }

            [JsonProperty("sub_time_range")]
            public TimeRange SubTimeRange { get; } = new()
            {
                Duration = TimeSpan.FromMicroseconds(-1),
                Start = TimeSpan.FromMicroseconds(-1),
            };

            [JsonProperty("type")]
            public int Type { get; } = 0;

            [JsonProperty("width")]
            protected int _Width = 0;

            [JsonProperty("height")]
            protected int _Height = 0;
        }
        public class DraftMaterialValueVideo : DraftMaterialValue
        {
            public DraftMaterialValueVideo()
            {
                Metetype = MeteType.video;
            }

            [JsonIgnore]
            public required int Width
            {
                get => _Width;
                init => _Width = value;
            }

            [JsonIgnore]
            public required int Height
            {
                get => _Height;
                init => _Height = value;
            }

            [JsonIgnore]
            internal bool HasAudio { get; init; }

            public CapcutMaterialVideoVideo CreateMaterial() => CapcutMaterialVideoVideo.Create(this);
        }
        public class DraftMaterialValuePhoto : DraftMaterialValue
        {
            public DraftMaterialValuePhoto()
            {
                Metetype = MeteType.photo;
            }

            [JsonIgnore]
            public required int Width
            {
                get => _Width;
                init => _Width = value;
            }

            [JsonIgnore]
            public required int Height
            {
                get => _Height;
                init => _Height = value;
            }

            public CapcutMaterialVideoPhoto CreateMaterial() => CapcutMaterialVideoPhoto.Create(this);
        }
        public class DraftMaterialValueAudio : DraftMaterialValue
        {
            public DraftMaterialValueAudio()
            {
                Metetype = MeteType.music;
            }
            public CapcutMaterialAudioExtractMusic CreateMaterial() => CapcutMaterialAudioExtractMusic.Create(this);
        }
        public class TimeRange
        {
            [JsonProperty("duration")]
            public required TimeSpan Duration { get; init; }

            [JsonProperty("start")]
            public required TimeSpan Start { get; init; }
        }
    }
}
