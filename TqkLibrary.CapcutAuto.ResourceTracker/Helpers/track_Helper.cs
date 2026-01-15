using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.CapcutAuto.ResourceTracker.Helpers
{
    internal class track_Helper : BaseHelper
    {
        readonly Dictionary<string, IReadOnlyList<string>> _videoExtraMaterialRefIds = new();
        readonly Dictionary<string, IReadOnlyList<string>> _textExtraMaterialRefIds = new();
        readonly Dictionary<string, IReadOnlyList<string>> _stickerExtraMaterialRefIds = new();


        public IReadOnlyDictionary<string, IReadOnlyList<string>> VideoExtraMaterialRefIds => _videoExtraMaterialRefIds;
        public IReadOnlyDictionary<string, IReadOnlyList<string>> TextExtraMaterialRefIds => _textExtraMaterialRefIds;
        public IReadOnlyDictionary<string, IReadOnlyList<string>> StickerExtraMaterialRefIds => _stickerExtraMaterialRefIds;

        protected override Task _ParseAsync(JObject data)
        {
            var tracks = data["tracks"];
            if (tracks?.Type == JTokenType.Array)
            {
                foreach (var track in tracks)
                {
                    string? type = track.Value<string>("type");
                    if (string.IsNullOrWhiteSpace(type))
                        continue;

                    Dictionary<string, IReadOnlyList<string>>? extraMaterialRefIds = type switch
                    {
                        "video" => _videoExtraMaterialRefIds,
                        "text" => _textExtraMaterialRefIds,
                        "sticker" => _stickerExtraMaterialRefIds,
                        _ => null
                    };
                    if (extraMaterialRefIds is null)
                        continue;

                    var segments = track["segments"];
                    if (segments?.Type == JTokenType.Array)
                    {
                        foreach (var segment in segments)
                        {
                            string? material_id = segment.Value<string>("material_id");
                            if (string.IsNullOrWhiteSpace(material_id))
                                continue;

                            List<string> extraMaterialRefs = new();

                            var extra_material_refs = segment["extra_material_refs"];
                            if (extra_material_refs?.Type == JTokenType.Array)
                            {
                                foreach (var extra_material_ref in extra_material_refs)
                                {
                                    extraMaterialRefs.Add(extra_material_ref.ToString());
                                }
                            }

                            extraMaterialRefIds[material_id] = extraMaterialRefs;
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
