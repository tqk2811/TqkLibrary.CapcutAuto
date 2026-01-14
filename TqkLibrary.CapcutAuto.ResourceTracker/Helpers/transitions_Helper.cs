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
    internal class transitions_Helper : BaseHelper
    {
        public override Task ParseAsync(JObject data)
        {
            return Task.Run(() => _ParseAsync(data));
        }
        async Task _ParseAsync(JObject data)
        {
            var materials = data["materials"];
            if (materials is not null)
            {
                var transitions = materials["transitions"];
                if (transitions?.Type == JTokenType.Array)
                {
                    foreach (var transition in transitions)
                    {
                        string? name = transition.Value<string>("name");
                        string? path = transition.Value<string>("path");
                        string? resource_id = transition.Value<string>("resource_id");
                        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(resource_id) || !Directory.Exists(path))
                            continue;

                        string fileName = $"{resource_id}.json";
                        string jsonFilePath = Path.Combine(TransitionsDir, fileName);
                        if (!File.Exists(jsonFilePath))
                        {
                            string fixPath = "%userprofile%" + path.Replace('\\', '/').Substring(userprofile.Length);
                            transition["path"] = fixPath;

                            Console.WriteLine($"Write transition: {name} ({fileName})");
                            string json = JsonConvert.SerializeObject(transition, Formatting.Indented);
                            await File.WriteAllTextAsync(jsonFilePath, json);
                        }
                        string zipFileName = $"{resource_id}.zip";
                        string zipFilePath = Path.Combine(TransitionsDir, zipFileName);
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
