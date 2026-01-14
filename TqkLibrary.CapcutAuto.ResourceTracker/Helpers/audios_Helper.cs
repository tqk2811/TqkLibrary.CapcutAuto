using NAudio.Wave;
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
    internal class audios_Helper : BaseHelper
    {
        protected override async Task _ParseAsync(JObject data)
        {
            var materials = data["materials"];
            if (materials is not null)
            {
                var audios = materials["audios"];
                if (audios?.Type == JTokenType.Array)
                {
                    foreach (var audio in audios)
                    {
                        string? type = audio.Value<string>("type");
                        string? path = audio.Value<string>("path");
                        string? name = audio.Value<string>("name");
                        string? music_id = audio.Value<string>("music_id");
                        if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(music_id) || !File.Exists(path))
                            continue;
                        if ("music".Equals(type))
                        {
                            string fileName = $"{music_id}.json";
                            string jsonFilePath = Path.Combine(AudiosDir, fileName);
                            if (!File.Exists(jsonFilePath))
                            {
                                string fixPath = "%userprofile%" + path.Replace('\\', '/').Substring(userprofile.Length);
                                audio["path"] = fixPath;

                                Console.WriteLine($"Write audio: {name} ({fileName})");
                                string json = JsonConvert.SerializeObject(audio, Formatting.Indented);
                                await File.WriteAllTextAsync(jsonFilePath, json);
                            }
                            string zipFileName = $"{music_id}.zip";
                            string zipFilePath = Path.Combine(AudiosDir, zipFileName);
                            if (!File.Exists(zipFilePath))
                            {
                                using FileStream zipToOpen = new FileStream(zipFilePath, FileMode.Create);
                                using ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create);
                                archive.CreateEntryFromFile(path, Path.GetFileName(path));
                            }

                            string extraInfo = $"{music_id}.ExtraInfoJson";
                            string extraInfoFilePath = Path.Combine(AudiosDir, extraInfo);
                            if (!File.Exists(extraInfoFilePath))
                            {
                                using (var reader = new AudioFileReader(path))
                                {
                                    float max = 0;
                                    double sumOfSquares = 0;
                                    long samplesRead = 0;
                                    float[] buffer = new float[reader.WaveFormat.SampleRate];

                                    int read;
                                    while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        for (int i = 0; i < read; i++)
                                        {
                                            float sample = Math.Abs(buffer[i]);
                                            if (sample > max) max = sample;
                                            sumOfSquares += sample * sample;
                                            samplesRead++;
                                        }
                                    }

                                    double rms = Math.Sqrt(sumOfSquares / samplesRead);
                                    double maxDb = 20 * Math.Log10(max);
                                    double avgDb = 20 * Math.Log10(rms);

                                    JObject extraInfoData = new JObject();
                                    extraInfoData["MaxDb"] = maxDb;
                                    extraInfoData["AvgDb"] = avgDb;
                                    string json_extraInfoData = JsonConvert.SerializeObject(extraInfoData, Formatting.Indented);
                                    await File.WriteAllTextAsync(extraInfoFilePath, json_extraInfoData);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
