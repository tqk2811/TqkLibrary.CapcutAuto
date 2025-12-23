using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.JsonConverters;

namespace TqkLibrary.CapcutAuto.Models
{
    public class RootMetaInfo
    {
        [JsonProperty("all_draft_store")]
        public required List<DraftStore> AllDraftStore { get; init; }

        [JsonProperty("draft_ids")]
        public required int DraftIds { get; init; }

        [JsonProperty("root_path")]
        [JsonConverter(typeof(CapcutPathConverter))]
        public required string RootPath { get; init; }

        public class DraftStore
        {
            [JsonProperty("cloud_draft_cover")]
            public bool CloudDraftCover { get; set; } = false;

            [JsonProperty("cloud_draft_sync")]
            public bool CloudDraftSync { get; set; } = false;

            [JsonProperty("draft_cloud_last_action_download")]
            public bool DraftCloudLastActionDownload { get; set; } = false;

            [JsonProperty("draft_cloud_purchase_info")]
            public string DraftCloudPurchaseInfo { get; set; } = string.Empty;

            [JsonProperty("draft_cloud_template_id")]
            public string DraftCloudTemplateId { get; set; } = string.Empty;

            [JsonProperty("draft_cloud_tutorial_info")]
            public string DraftCloudTutorialInfo { get; set; } = string.Empty;

            [JsonProperty("draft_cloud_videocut_purchase_info")]
            public string DraftCloudVideocutPurchaseInfo { get; set; } = string.Empty;

            [JsonProperty("draft_cover")]
            [JsonConverter(typeof(CapcutPathConverter))]
            public string DraftCover { get; set; } = string.Empty;

            [JsonProperty("draft_fold_path")]
            [JsonConverter(typeof(CapcutPathConverter))]
            public required string DraftFoldPath { get; set; }

            [JsonProperty("draft_id")]
            public Guid DraftId { get; set; } = Guid.NewGuid();

            [JsonProperty("draft_is_ai_shorts")]
            public bool DraftIsAiShorts { get; set; } = false;

            [JsonProperty("draft_is_cloud_temp_draft")]
            public bool DraftIsCloudTempDraft { get; set; } = false;

            [JsonProperty("draft_is_invisible")]
            public bool DraftIsInvisible { get; set; } = false;

            [JsonProperty("draft_is_web_article_video")]
            public bool DraftIsWebArticleVideo { get; set; } = false;

            [JsonProperty("draft_json_file")]
            [JsonConverter(typeof(CapcutPathConverter))]
            public required string DraftJsonFile { get; set; }

            [JsonProperty("draft_name")]
            public required string DraftName { get; set; }

            [JsonProperty("draft_new_version")]
            public string DraftNewVersion { get; set; } = string.Empty;

            [JsonProperty("draft_root_path")]
            [JsonConverter(typeof(CapcutPathConverter))]
            public required string DraftRootPath { get; set; }

            [JsonProperty("draft_timeline_materials_size")]
            public int DraftTimelineMaterialsSize { get; set; } = 0;

            [JsonProperty("draft_type")]
            public string DraftType { get; set; } = string.Empty;

            [JsonProperty("draft_web_article_video_enter_from")]
            public string DraftWebArticleVideoEnterFrom { get; set; } = string.Empty;

            [JsonProperty("streaming_edit_draft_ready")]
            public bool StreamingEditDraftReady { get; set; } = true;

            [JsonProperty("tm_draft_cloud_completed")]
            public string TmDraftCloudCompleted { get; set; } = string.Empty;

            [JsonProperty("tm_draft_cloud_entry_id")]
            public int TmDraftCloudEntryId { get; set; } = -1;

            [JsonProperty("tm_draft_cloud_modified")]
            public int TmDraftCloudModified { get; set; } = 0;

            [JsonProperty("tm_draft_cloud_parent_entry_id")]
            public int TmDraftCloudParentEntryId { get; set; } = -1;

            [JsonProperty("tm_draft_cloud_space_id")]
            public int TmDraftCloudSpaceId { get; set; } = -1;

            [JsonProperty("tm_draft_cloud_user_id")]
            public int TmDraftCloudUserId { get; set; } = -1;

            [JsonProperty("tm_draft_create")]
            public DateTime TmDraftCreate { get; set; } = DateTime.Now;

            [JsonProperty("tm_draft_modified")]
            public DateTime TmDraftModified { get; set; } = DateTime.Now;

            [JsonProperty("tm_draft_removed")]
            public long TmDraftRemoved { get; set; } = 0;

            [JsonProperty("tm_duration")]
            public TimeSpan TmDuration { get; set; } = TimeSpan.Zero;
        }
    }


}
