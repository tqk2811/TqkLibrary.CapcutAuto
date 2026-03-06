using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TqkLibrary.CapcutAuto.ResourceTracker.Helpers
{
    internal class stickers_Hepler : BaseHelper
    {
        protected override async Task _ParseAsync(JObject data)
        {
            var materials = data["materials"];
            if (materials is not null)
            {
                var stickers = materials["stickers"];
                if (stickers?.Type == JTokenType.Array)
                {
                    foreach (var sticker in stickers)
                    {
                        string? name = sticker.Value<string>("name");
                        string? path = sticker.Value<string>("path");
                        string? resource_id = sticker.Value<string>("resource_id");
                        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(resource_id) || !Directory.Exists(path))
                            continue;


                        string extraInfo = $"{resource_id}.ExtraInfoJson";
                        string extraInfoFilePath = Path.Combine(StickersDir, extraInfo);
                        if (!File.Exists(extraInfoFilePath))
                        {
                            JObject? extraInfoData = await CreateExtraInfoAsync(path, resource_id);
                            if (extraInfoData is null)
                                continue;
                            string extraInfoJson = JsonConvert.SerializeObject(extraInfoData, Formatting.Indented);
                            await File.WriteAllTextAsync(extraInfoFilePath, extraInfoJson);
                        }


                        string fileName = $"{resource_id}.json";
                        string jsonFilePath = Path.Combine(StickersDir, fileName);
                        if (!File.Exists(jsonFilePath))
                        {
                            string fixPath = "%userprofile%" + path.Replace('\\', '/').Substring(userprofile.Length);
                            sticker["path"] = fixPath;

                            Console.WriteLine($"Write sticker json: {name} ({fileName})");
                            string json = JsonConvert.SerializeObject(sticker, Formatting.Indented);
                            await File.WriteAllTextAsync(jsonFilePath, json);
                        }

                        string zipFileName = $"{resource_id}.zip";
                        string zipFilePath = Path.Combine(StickersDir, zipFileName);
                        if (!File.Exists(zipFilePath))
                        {
                            Console.WriteLine($"Write sticker zip: {name} ({zipFileName})");
                            ZipFile.CreateFromDirectory(path, zipFilePath);
                        }
                    }
                }
            }
        }

        public async Task ReRunExtraInfoJsonAsync()
        {
            var zipFiles = Directory.GetFiles(StickersDir, "*.zip", SearchOption.TopDirectoryOnly);
            foreach (var item in zipFiles)
            {
                FileInfo fileInfo = new FileInfo(item);
                string id = Path.GetFileNameWithoutExtension(fileInfo.Name);
                string extractDir = Path.Combine(StickersDir, id);
                try
                {
                    ZipFile.ExtractToDirectory(fileInfo.FullName, extractDir, true);
                    JObject? extraInfoData = await CreateExtraInfoAsync(extractDir, id);
                    if (extraInfoData is null)
                        continue;
                    string extraInfoJson = JsonConvert.SerializeObject(extraInfoData, Formatting.Indented);

                    string extraInfo = $"{id}.ExtraInfoJson";
                    string extraInfoFilePath = Path.Combine(StickersDir, extraInfo);
                    await File.WriteAllTextAsync(extraInfoFilePath, extraInfoJson);
                }
                finally
                {
                    if (Directory.Exists(extractDir))
                        Directory.Delete(extractDir, true);
                }
            }
        }

        async Task<JObject?> CreateExtraInfoAsync(string dir, string id)
        {
            string heycanInfo_json = Path.Combine(dir, "heycanInfo.json");
            JObject extraInfoData = new JObject();
            if (File.Exists(heycanInfo_json))
            {
                string json_heycanInfo_text = await File.ReadAllTextAsync(heycanInfo_json);
                JObject data_heycanInfo = (JObject)JsonConvert.DeserializeObject(json_heycanInfo_text)!;
                int singleWidth = data_heycanInfo.Value<int>("singleWidth");
                int singleHeight = data_heycanInfo.Value<int>("singleHeight");
                extraInfoData["Size"] = JToken.FromObject(new Size(singleWidth, singleHeight));
            }
            else
            {
                string[] pngFilePaths = Directory.GetFiles(dir, "*.png", SearchOption.AllDirectories);
                if (pngFilePaths.Length == 0)
                {
                    Console.WriteLine($"Sticker '{id}': Not found heycanInfo.json or *.png");
                    return null;
                }
                else if (pngFilePaths.Length == 1)
                {
                    FileInfo fileInfo_png = new FileInfo(pngFilePaths[0]);
                    string png_json_path = Path.Combine(fileInfo_png.Directory!.FullName, $"{Path.GetFileNameWithoutExtension(pngFilePaths[0])}.json");
                    if (!File.Exists(png_json_path))
                    {
                        var files = Directory.GetFiles(fileInfo_png.Directory.FullName, "*.json", SearchOption.TopDirectoryOnly);
                        if (files.Length == 1)
                            png_json_path = files[0];
                    }
                    if (File.Exists(png_json_path))
                    {
                        string json_png_text = await File.ReadAllTextAsync(png_json_path);
                        JObject data_png = (JObject)JsonConvert.DeserializeObject(json_png_text)!;
                        var frames = data_png["frames"];
                        if (frames?.Type == JTokenType.Array)
                        {
                            var frame = frames.First?["frame"];
                            if (frame is not null)
                            {
                                double w = frame.Value<double>("w");
                                double h = frame.Value<double>("h");
                                extraInfoData["Size"] = JToken.FromObject(new Size((int)w, (int)h));
                            }
                            else
                            {
                                Console.WriteLine($"not found frame");
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"not found frames");
                            return null;
                        }
                    }
                    else
                    {
                        using Image bitmap = Bitmap.FromFile(pngFilePaths[0]);
                        extraInfoData["Size"] = JToken.FromObject(bitmap.Size);
                    }
                }
                else
                {
                    Console.WriteLine($"Sticker {id}: Not found heycanInfo.json, found many *.png");
                    return null;
                }
            }
            return extraInfoData;
        }
    }
}
