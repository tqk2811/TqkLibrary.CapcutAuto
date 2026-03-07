using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceTracker.Helpers
{
    internal class texts_Helper : BaseHelper
    {
        private track_Helper _track_Helper;
        private effects_Helper _effects_Helper;

        public texts_Helper(track_Helper track_Helper, effects_Helper effects_Helper)
        {
            this._track_Helper = track_Helper;
            this._effects_Helper = effects_Helper;
        }

        protected override async Task _ParseAsync(JObject data)
        {
            var materials = data["materials"];
            if (materials is not null)
            {
                var texts = materials["texts"];
                if (texts is not null && (texts?.Type) == JTokenType.Array)
                {
                    foreach (var text in texts)
                    {
                        var fonts = text["fonts"];
                        if (fonts is not null && (fonts?.Type) == JTokenType.Array)
                        {
                            foreach (var font in fonts)
                            {
                                string? title = font.Value<string>("title");
                                string? resource_id = font.Value<string>("resource_id");
                                string? path = font.Value<string>("path");
                                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(resource_id) || !File.Exists(path))
                                    continue;

                                string fontFileName = $"{resource_id}.json";
                                string fontJsonFilePath = Path.Combine(FontsDir, fontFileName);
                                if (!File.Exists(fontJsonFilePath))
                                {
                                    string fixPath = "%userprofile%" + path.Replace('\\', '/').Substring(userprofile.Length);
                                    font["path"] = fixPath;

                                    Console.WriteLine($"Write font: {title} ({fontFileName})");
                                    string fontJson = JsonConvert.SerializeObject(font, Formatting.Indented);
                                    await File.WriteAllTextAsync(fontJsonFilePath, fontJson);
                                }

                                string zipFileName = $"{resource_id}.zip";
                                string zipFilePath = Path.Combine(FontsDir, zipFileName);
                                if (!File.Exists(zipFilePath))
                                {
                                    using FileStream zipToOpen = new FileStream(zipFilePath, FileMode.Create);
                                    using ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create);
                                    archive.CreateEntryFromFile(path, Path.GetFileName(path));
                                }

                                string extraInfoFileName = $"{resource_id}.ExtraInfoJson";
                                string extraInfoFilePath = Path.Combine(FontsDir, extraInfoFileName);
                                if (!File.Exists(extraInfoFilePath))
                                {
                                    JObject extraInfoData = new JObject();
                                    using (var typeface = SKTypeface.FromFile(path))
                                    {
                                        using (var skFont = new SKFont(typeface, 16))
                                        {
                                            JArray measuresArray = new JArray();
                                            foreach (var fontSize in new int[] { 16, 24, 36 })
                                            {
                                                JObject measureItem = new JObject();
                                                measureItem["FontSize"] = fontSize;
                                                skFont.Size = fontSize;
                                                measureItem["Spacing"] = skFont.Spacing;
                                                measuresArray.Add(measureItem);
                                            }
                                            extraInfoData["Measures"] = measuresArray;
                                        }
                                    }
                                    string extraInfoJson = JsonConvert.SerializeObject(extraInfoData, Formatting.Indented);
                                    await File.WriteAllTextAsync(extraInfoFilePath, extraInfoJson);
                                }
                            }
                        }

                        string id = text.Value<string>("id")!;

                        int count = Directory.GetFiles(TextsDir, "*.json").Length;

                        string textFileName = $"{count:000}.json";
                        string textJsonFilePath = Path.Combine(TextsDir, textFileName);
                        Console.WriteLine($"Write text: {textFileName}");
                        string textJson = JsonConvert.SerializeObject(text, Formatting.Indented);
                        await File.WriteAllTextAsync(textJsonFilePath, textJson);

                        string? bloomId = null;
                        if (_track_Helper.TextExtraMaterialRefIds.ContainsKey(id))
                        {
                            foreach (var pair_id_resourceId in _effects_Helper.Effect_Blooms)
                            {
                                if (_track_Helper.TextExtraMaterialRefIds[id].Contains(pair_id_resourceId.Key))
                                {
                                    bloomId = pair_id_resourceId.Value;
                                    break;
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(bloomId))
                        {
                            string LinkedResourcesJsonFileName = $"{count:000}.LinkedResourcesJson";
                            string textLinkedResourcesJsonFilePath = Path.Combine(TextsDir, LinkedResourcesJsonFileName);
                            if (!File.Exists(textLinkedResourcesJsonFilePath))
                            {
                                Console.WriteLine($"Write text linked resources: {LinkedResourcesJsonFileName}");
                                LinkedResourceItem linkedResourceItem = new LinkedResourceItem()
                                {
                                    Id = bloomId,
                                    Type = MaterialType.bloom
                                };

                                string textLinkedResourcesJson = JsonConvert.SerializeObject(new List<LinkedResourceItem>() { linkedResourceItem }, Formatting.Indented);
                                await File.WriteAllTextAsync(textLinkedResourcesJsonFilePath, textLinkedResourcesJson);
                            }
                        }
                    }
                }
            }
        }


        class LinkedResourceItem
        {
            public required MaterialType Type { get; set; }
            public required string Id { get; set; }
        }
    }
}