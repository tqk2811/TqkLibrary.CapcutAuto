using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Timers;
using System.Transactions;
using System.Xml.Linq;
using TqkLibrary.Linq;

Console.OutputEncoding = Encoding.UTF8;

string test = JsonConvert.SerializeObject(Guid.NewGuid());

string CapcutDatasDir = Path.Combine(AppContext.BaseDirectory, "CapcutDatas");

string AnimationsDir = Path.Combine(CapcutDatasDir, "Animations");
IEnumerable<IEnumerable<string>> ItemsInAnimationsDir = new IEnumerable<string>[]
{
    new string[]
    {
        "Texts",
        "Videos",
        "Stickers"
    },
    new string[]
    {
        "Ins",
        "Outs",
        "Loops"
    }
};
foreach (var item in ItemsInAnimationsDir.TwoDimensionCombinations())
{
    List<string> names = new List<string>()
    {
        AnimationsDir,
    };
    names.AddRange(item);
    string dir = Path.Combine(names.ToArray());
    Directory.CreateDirectory(dir);
}

string TransitionsDir = Path.Combine(CapcutDatasDir, "Transitions");
Directory.CreateDirectory(TransitionsDir);

string EffectsDir = Path.Combine(CapcutDatasDir, "Effects");

string TextEffectsDir = Path.Combine(EffectsDir, "TextEffects");
Directory.CreateDirectory(TextEffectsDir);

string TextShapesDir = Path.Combine(EffectsDir, "TextShapes");
Directory.CreateDirectory(TextShapesDir);

string StickersDir = Path.Combine(CapcutDatasDir, "Stickers");
Directory.CreateDirectory(StickersDir);

string TextsDir = Path.Combine(CapcutDatasDir, "Texts");
Directory.CreateDirectory(TextsDir);

string AudiosDir = Path.Combine(CapcutDatasDir, "Audios");
Directory.CreateDirectory(AudiosDir);



#if DEBUG
await RunAsync(Path.Combine(AppContext.BaseDirectory, "draft_content.json"));
#endif


Option<string> o_filePath = new("--draft_content")
{
    Required = true,
    Validators =
    {
        result =>
        {
            string? value = result.GetValueOrDefault<string>();
            if (!File.Exists(value))
            {
                result.AddError($"draft_content file not exist: '{value}'");
                return;
            }
        }
    }
};

RootCommand rootCommand = new RootCommand("CapcutAuto.ResourceTracker")
{
    o_filePath
};

rootCommand.TreatUnmatchedTokensAsErrors = false;
ParseResult parseResult = rootCommand.Parse(args);
if (parseResult.Errors.Any())
{
    foreach (var error in parseResult.Errors)
    {
        Console.WriteLine($"{error.Message}");
    }
    Environment.Exit(1);
    return;
}

FileInfo fileInfo = new FileInfo(parseResult.GetValue(o_filePath)!);

System.Timers.Timer timer = new(500);
timer.Elapsed += Timer_Elapsed;
timer.AutoReset = false;
bool isRunning = false;

using FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(fileInfo.Directory!.FullName);
fileSystemWatcher.Changed += FileSystemWatcher_Changed;
fileSystemWatcher.Filter = fileInfo.Name;
fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
fileSystemWatcher.IncludeSubdirectories = false;
fileSystemWatcher.EnableRaisingEvents = true;
while (true)
{
    await Task.Delay(1000);
}

void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
{
    if (e.ChangeType == WatcherChangeTypes.Changed
        && fileInfo.FullName.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase)
        )
    {
        lock (timer)
        {
            if (isRunning)
                return;
        }
        timer.Stop();
        timer.Start();
    }
}
async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
{
    lock (timer)
    {
        isRunning = true;
    }
    try
    {
        await RunAsync(fileInfo.FullName);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
    }
    finally
    {
        lock (timer)
        {
            isRunning = false;
        }
    }
}
async Task RunAsync(string draftContentFilePath)
{
    string json_text = await File.ReadAllTextAsync(draftContentFilePath);
    JObject data = (JObject)JsonConvert.DeserializeObject(json_text)!;

    List<string> videoMaterialIds = new();
    List<string> textMaterialIds = new();
    List<string> stickerMaterialIds = new();
    var tracks = data["tracks"];
    if (tracks?.Type == JTokenType.Array)
    {
        foreach (var track in tracks)
        {
            string? type = track.Value<string>("type");
            if (string.IsNullOrWhiteSpace(type))
                continue;

            List<string>? materialIds = type switch
            {
                "video" => videoMaterialIds,
                "text" => textMaterialIds,
                "sticker" => stickerMaterialIds,
                _ => null
            };
            if (materialIds is null)
                continue;

            var segments = track["segments"];
            if (segments?.Type == JTokenType.Array)
            {
                foreach (var segment in segments)
                {
                    var extra_material_refs = segment["extra_material_refs"];
                    if (extra_material_refs?.Type == JTokenType.Array)
                    {
                        foreach (var extra_material_ref in extra_material_refs)
                        {
                            materialIds.Add(extra_material_ref.ToString());
                        }
                    }
                }
            }
        }
    }

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
                if (videoMaterialIds.Contains(id, StringComparer.OrdinalIgnoreCase))
                {
                    dirName = "Videos";
                }
                else if (textMaterialIds.Contains(id, StringComparer.OrdinalIgnoreCase))
                {
                    dirName = "Texts";
                }
                else if (stickerMaterialIds.Contains(id, StringComparer.OrdinalIgnoreCase))
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

                string fileName = $"{resource_id}.json";
                string jsonFilePath = Path.Combine(StickersDir, fileName);
                if (!File.Exists(jsonFilePath))
                {
                    Console.WriteLine($"Write sticker: {name} ({fileName})");
                    string json = JsonConvert.SerializeObject(sticker, Formatting.Indented);
                    await File.WriteAllTextAsync(jsonFilePath, json);
                }
                string zipFileName = $"{resource_id}.zip";
                string zipFilePath = Path.Combine(StickersDir, zipFileName);
                if (!File.Exists(zipFilePath))
                {
                    ZipFile.CreateFromDirectory(path, zipFilePath);
                }
            }
        }

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
                }
            }
        }

        var texts = materials["texts"];
        if (texts is not null && (texts?.Type) == JTokenType.Array)
        {
            foreach (var text in texts)
            {
                int count = Directory.GetFiles(TextsDir).Length;
                string fileName = $"{count:000}.json";
                string jsonFilePath = Path.Combine(TextsDir, fileName);
                Console.WriteLine($"Write text: {fileName}");
                string json = JsonConvert.SerializeObject(text, Formatting.Indented);
                await File.WriteAllTextAsync(jsonFilePath, json);
            }
        }
    }
}