using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.CommandLine;
using System.Text;
using System.Timers;
using TqkLibrary.CapcutAuto.ResourceTracker.Helpers;
using TqkLibrary.Linq;

Console.OutputEncoding = Encoding.UTF8;

#if DEBUG && !REMOTEDEBUG
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

    track_Helper track_Helper = new();
    await track_Helper.ParseAsync(data);

    List<Task> tasks = new();
    material_animations_Helper material_Animations = new(track_Helper);
    tasks.Add(material_Animations.ParseAsync(data));

    transitions_Helper transitions_Helper = new();
    tasks.Add(transitions_Helper.ParseAsync(data));

    effects_Helper effects_Helper = new();
    tasks.Add(effects_Helper.ParseAsync(data));

    stickers_Hepler stickers_Hepler = new();
    tasks.Add(stickers_Hepler.ParseAsync(data));

    audios_Helper audios_Helper = new();
    tasks.Add(audios_Helper.ParseAsync(data));

    texts_Helper texts_Helper = new();
    tasks.Add(texts_Helper.ParseAsync(data));

    await Task.WhenAll(tasks);
}