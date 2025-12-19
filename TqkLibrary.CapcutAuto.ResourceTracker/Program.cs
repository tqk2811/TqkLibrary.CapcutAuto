using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.CommandLine;
using System.Timers;
using System.Transactions;

string CapcutDatasDir = Path.Combine(AppContext.BaseDirectory, "CapcutDatas");

string AnimationsDir = Path.Combine(CapcutDatasDir, "Animations");
string AnimationsInsDir = Path.Combine(AnimationsDir, "Ins");
string AnimationsOutsDir = Path.Combine(AnimationsDir, "Outs");

string TransitionsDir = Path.Combine(CapcutDatasDir, "Transitions");

Directory.CreateDirectory(AnimationsInsDir);
Directory.CreateDirectory(AnimationsOutsDir);
Directory.CreateDirectory(TransitionsDir);

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
                result.AddError("draft_content file not exist");
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
    var materials = data["materials"];
    if (materials is not null)
    {
        var material_animations = materials["material_animations"];
        if (material_animations is not null && (material_animations?.Type) == JTokenType.Array)
        {
            foreach (var material_animation in material_animations)
            {
                var animations = material_animation["animations"];
                if (animations is null || animations?.Type != JTokenType.Array)
                    continue;
                foreach (var animation in animations)
                {
                    string? name = animation.Value<string>("name");
                    string? resource_id = animation.Value<string>("resource_id");
                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(resource_id))
                        continue;

                    string? type = animation.Value<string>("type");
                    string fileName = $"{resource_id}.json";
                    string? jsonFilePath = type switch
                    {
                        "in" => Path.Combine(AnimationsInsDir, fileName),
                        "out" => Path.Combine(AnimationsOutsDir, fileName),
                        _ => null
                    };
                    if (string.IsNullOrWhiteSpace(jsonFilePath))
                        continue;

                    if (!File.Exists(jsonFilePath))
                    {
                        Console.WriteLine($"Write animation {type}: {name} ({fileName})");
                        string json = JsonConvert.SerializeObject(animation, Formatting.Indented);
                        await File.WriteAllTextAsync(jsonFilePath, json);
                    }
                }
            }
        }

        var transitions = materials["transitions"];
        if (transitions is not null && (transitions?.Type) == JTokenType.Array)
        {
            foreach (var transition in transitions)
            {
                string? name = transition.Value<string>("name");
                string? resource_id = transition.Value<string>("resource_id");
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(resource_id))
                    continue;

                string fileName = $"{resource_id}.json";
                string jsonFilePath = Path.Combine(TransitionsDir, fileName);
                if (!File.Exists(jsonFilePath))
                {
                    Console.WriteLine($"Write transition: {name} ({fileName})");
                    string json = JsonConvert.SerializeObject(transition, Formatting.Indented);
                    await File.WriteAllTextAsync(jsonFilePath, json);
                }
            }
        }
    }
}