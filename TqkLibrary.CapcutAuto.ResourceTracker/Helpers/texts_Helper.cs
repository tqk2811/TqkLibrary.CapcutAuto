using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.CapcutAuto.ResourceTracker.Helpers
{
    internal class texts_Helper : BaseHelper
    {
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

                                string fileName = $"{resource_id}.json";
                                string jsonFilePath = Path.Combine(FontsDir, fileName);
                                if (!File.Exists(jsonFilePath))
                                {
                                    string fixPath = "%userprofile%" + path.Replace('\\', '/').Substring(userprofile.Length);
                                    font["path"] = fixPath;

                                    Console.WriteLine($"Write font: {title} ({fileName})");
                                    string json = JsonConvert.SerializeObject(font, Formatting.Indented);
                                    await File.WriteAllTextAsync(jsonFilePath, json);
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
                                if(!File.Exists(extraInfoFilePath))
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

                        //int count = Directory.GetFiles(TextsDir).Length;
                        //string fileName = $"{count:000}.json";
                        //string jsonFilePath = Path.Combine(TextsDir, fileName);
                        //Console.WriteLine($"Write text: {fileName}");
                        //string json = JsonConvert.SerializeObject(text, Formatting.Indented);
                        //await File.WriteAllTextAsync(jsonFilePath, json);
                    }
                }
            }
        }
    }
}
