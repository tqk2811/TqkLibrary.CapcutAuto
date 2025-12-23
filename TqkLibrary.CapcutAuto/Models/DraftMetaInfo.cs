using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.CapcutAuto.Enums;
using TqkLibrary.CapcutAuto.JsonConverters;

namespace TqkLibrary.CapcutAuto.Models
{
    public class DraftMetaInfo : BaseCapcut
    {
        public DraftMetaInfo() : base(JObject.Parse(Extensions.GetEmbeddedResource("draft_meta_info.json")))
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
        public List<DraftMaterial> DraftMaterials { get; } = new();

        public class DraftMaterial
        {
            [JsonProperty("type")]
            public int Type { get; set; }//0 là file input

            [JsonProperty("value")]
            public List<DraftMaterialValue> Value { get; set; } = new();
        }
        public class DraftMaterialValue
        {
            [JsonProperty("ai_group_type")]
            public string AiGroupType { get; set; } = string.Empty;

            [JsonProperty("create_time")]
            public DateTime CreateTime { get; set; } = DateTime.Now;

            [JsonProperty("duration")]
            public TimeSpan Duration { get; set; } = TimeSpan.Zero;

            [JsonProperty("extra_info")]
            public required string ExtraInfo { get; set; }//file name

            [JsonProperty("file_Path")]
            public required string FilePath { get; set; }

            [JsonProperty("height")]
            public int Height { get; set; } = 0;

            [JsonProperty("id")]
            public Guid Id { get; set; } = Guid.NewGuid();

            [JsonProperty("import_time")]
            public DateTime ImportTime { get; set; } = DateTime.Now;

            [JsonProperty("import_time_ms")]
            public long ImportTimeMs => ImportTime.Microsecond;

            [JsonProperty("item_source")]
            public int ItemSource { get; set; } = 1;

            [JsonProperty("md5")]
            public string Md5 { get; set; } = string.Empty;

            [JsonProperty("metetype")]
            public MeteType Metetype { get; set; }

            [JsonProperty("roughcut_time_range")]
            public RoughcutTimeRange RoughcutTimeRange { get; set; } = new();

            [JsonProperty("sub_time_range")]
            public SubTimeRange SubTimeRange { get; set; } = new();

            [JsonProperty("type")]
            public int Type { get; set; } = 0;

            [JsonProperty("width")]
            public int Width { get; set; } = 0;
        }
        public class RoughcutTimeRange
        {
            [JsonProperty("duration")]
            public int Duration { get; set; }

            [JsonProperty("start")]
            public int Start { get; set; }
        }

        public class SubTimeRange
        {
            [JsonProperty("duration")]
            public int Duration { get; set; } = -1;

            [JsonProperty("start")]
            public int Start { get; set; } = -1;
        }
    }
}
