using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.CapcutAuto.ResourceTracker.Helpers
{
    internal class material_animations_Helper : BaseHelper
    {
        readonly track_Helper _track_Helper;
        public material_animations_Helper(track_Helper track_Helper)
        {
            this._track_Helper = track_Helper;
        }
        protected override async Task _ParseAsync(JObject data)
        {
            var materials = data["materials"];
            if (materials is not null)
            {
                var material_animations = materials["material_animations"];
                if (material_animations?.Type == JTokenType.Array)
                {
                    foreach (var material_animation in material_animations)
                    {
                        string? id = material_animation.Value<string>("id");
                        var animations = material_animation["animations"];
                        if (string.IsNullOrWhiteSpace(id)
                            || animations?.Type != JTokenType.Array
                            )
                            continue;

                        string dirName;
                        if (_track_Helper.VideoExtraMaterialRefIds.Values.SelectMany(x => x).Contains(id, StringComparer.OrdinalIgnoreCase))
                        {
                            dirName = "Videos";
                        }
                        else if (_track_Helper.TextExtraMaterialRefIds.Values.SelectMany(x => x).Contains(id, StringComparer.OrdinalIgnoreCase))
                        {
                            dirName = "Texts";
                        }
                        else if (_track_Helper.StickerExtraMaterialRefIds.Values.SelectMany(x => x).Contains(id, StringComparer.OrdinalIgnoreCase))
                        {
                            dirName = "Stickers";
                        }
                        else
                        {
                            Console.WriteLine($"material_animation: {id} not link to any segments");
                            continue;
                        }

                        foreach (var animation in animations)
                        {
                            string? name = animation.Value<string>("name");
                            string? path = animation.Value<string>("path");
                            string? resource_id = animation.Value<string>("resource_id");
                            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(resource_id) || !Directory.Exists(path))
                                continue;

                            string? type = animation.Value<string>("type");
                            string? childDirName = type switch
                            {
                                "in" => "Ins",
                                "out" => "Outs",
                                "loop" => "Loops",
                                _ => null
                            };
                            if (string.IsNullOrWhiteSpace(childDirName))
                            {
                                Console.WriteLine($"Not support animation type: {type}");
                                continue;
                            }

                            string jsonFileName = $"{resource_id}.json";
                            string jsonFilePath = Path.Combine(AnimationsDir, dirName, childDirName, jsonFileName);
                            if (!File.Exists(jsonFilePath))
                            {
                                string fixPath = "%userprofile%" + path.Replace('\\', '/').Substring(userprofile.Length);
                                animation["path"] = fixPath;

                                Console.WriteLine($"Write animation {dirName} {type}: {name} ({jsonFileName})");
                                string json = JsonConvert.SerializeObject(animation, Formatting.Indented);
                                await File.WriteAllTextAsync(jsonFilePath, json);
                            }
                            string zipFileName = $"{resource_id}.zip";
                            string zipFilePath = Path.Combine(AnimationsDir, dirName, childDirName, zipFileName);
                            if (!File.Exists(zipFilePath))
                            {
                                ZipFile.CreateFromDirectory(path, zipFilePath);
                            }
                        }
                    }
                }
            }
        }
    }
}
