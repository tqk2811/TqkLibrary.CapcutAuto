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
    internal class effects_Helper : BaseHelper
    {
        protected override async Task _ParseAsync(JObject data)
        {
            var materials = data["materials"];
            if (materials is not null)
            {
                var effects = materials["effects"];
                if (effects?.Type == JTokenType.Array)
                {
                    foreach (var effect in effects)
                    {
                        string? type = effect.Value<string>("type");
                        string? path = effect.Value<string>("path");
                        string? name = effect.Value<string>("name");
                        string? resource_id = effect.Value<string>("resource_id");
                        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(resource_id) || !Directory.Exists(path))
                            continue;

                        string? dir = type switch
                        {
                            "text_effect" => TextEffectsDir,
                            "text_shape" => TextShapesDir,
                            _ => null,
                        };
                        if (string.IsNullOrWhiteSpace(dir))
                        {
                            Console.WriteLine($"Not support materials effects type: {type}");
                            continue;
                        }

                        string fileName = $"{resource_id}.json";
                        string jsonFilePath = Path.Combine(dir, fileName);
                        if (!File.Exists(jsonFilePath))
                        {
                            string fixPath = "%userprofile%" + path.Replace('\\', '/').Substring(userprofile.Length);
                            effect["path"] = fixPath;

                            Console.WriteLine($"Write effect {type}: {name} ({fileName})");
                            string json = JsonConvert.SerializeObject(effect, Formatting.Indented);
                            await File.WriteAllTextAsync(jsonFilePath, json);
                        }
                        string zipFileName = $"{resource_id}.zip";
                        string zipFilePath = Path.Combine(dir, zipFileName);
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
