

using TqkLibrary.CapcutAuto;
using TqkLibrary.CapcutAuto.Helpers;
using TqkLibrary.CapcutAuto.Models;
using TqkLibrary.CapcutAuto.Models.Materials;
using TqkLibrary.CapcutAuto.Models.Resources;
using TqkLibrary.CapcutAuto.Models.Tracks;
using TqkLibrary.CapcutAuto.Models.Tracks.Segments;

List<CapcutMaterialTransition> capcutMaterialTransitions = new List<CapcutMaterialTransition>();
foreach (var file in Directory.GetFiles(".\\CapcutDatas\\Transitions", "*.json"))
{
    string json_text = await File.ReadAllTextAsync(file);
    capcutMaterialTransitions.Add(CapcutMaterialTransition.Parse(json_text));
}
List<CapcutAnimation> in_animations = new List<CapcutAnimation>();
List<CapcutAnimation> out_animations = new List<CapcutAnimation>();
foreach (var file in Directory.GetFiles(".\\CapcutDatas\\Animations\\Ins", "*.json"))
{
    string json_text = await File.ReadAllTextAsync(file);
    in_animations.Add(CapcutAnimation.Parse(json_text));
}
foreach (var file in Directory.GetFiles(".\\CapcutDatas\\Animations\\Outs", "*.json"))
{
    string json_text = await File.ReadAllTextAsync(file);
    out_animations.Add(CapcutAnimation.Parse(json_text));
}



FileInfo videoFileInfo = new FileInfo(".\\Resources\\OriginalVideo.mp4");
#if REMOTEDEBUG
videoFileInfo = videoFileInfo.CopyTo("C:\\OriginalVideo.mp4", true);
#endif
CapcutMaterialVideo capcutMaterialVideo = await CapcutMaterialVideo.CreateAsync(videoFileInfo.FullName);

FileInfo audioFileInfo = new FileInfo(".\\Resources\\tts_000.ogg");
#if REMOTEDEBUG
audioFileInfo = audioFileInfo.CopyTo("C:\\tts_000.ogg", true);
#endif
CapcutMaterialAudio capcutMaterialAudio = await CapcutMaterialAudio.CreateAsync(audioFileInfo.FullName);




CapcutTrackCollection capcutTracks = new()
{
    new CapcutTrackVideo()
    {
        new CapcutSegmentVideo()
        {
            MaterialVideo = capcutMaterialVideo.Clone(),
            MaterialSpeed = new()
            {

            },
            MaterialTransition = capcutMaterialTransitions.First().Clone(),
            TargetTimerange = new()
            {

            },
            Speed = 1.0,

            Volume = 0.0,
        },
        new CapcutSegmentVideo()
        {
            MaterialVideo = capcutMaterialVideo.Clone(),
            MaterialSpeed = new()
            {

            },
            MaterialTransition = capcutMaterialTransitions.First().Clone(),
            TargetTimerange = new()
            {

            },
            Speed = 1.0,

            Volume = 0.0,
        },
    },
    new CapcutTrackAudio()
    {
        new CapcutSegmentAudio()
        {
            MaterialAudio = capcutMaterialAudio.Clone(),
            TargetTimerange = new()
            {

            },
            Volume = 1.0,
            Speed = 1.0,
        }
    },
    new CapcutTrackText()
    {
        //new CapcutSegmentText()
        //{
            
        //},
    }
};
CapcutDraftContentHelper capcutDraftContentHelper = new(capcutTracks);
//capcutDraftContentHelper.DraftContent.
string draft_content_json = capcutDraftContentHelper.BuildJson();


string rootDir = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    "AppData\\Local\\CapCut\\User Data\\Projects\\com.lveditor.draft"
    );
string root_meta_info_file = Path.Combine(rootDir, "root_meta_info.json");

string projectName = "myproj";
string projectDir = Path.Combine(rootDir, projectName);
Directory.CreateDirectory(projectDir);
string draft_meta_info_file = Path.Combine(projectDir, "draft_meta_info.json");

foreach (var file in Directory.GetFiles(".\\Resources\\Projects"))
{
    FileInfo fileInfo = new FileInfo(file);
    fileInfo.CopyTo(Path.Combine(projectDir, fileInfo.Name), true);
}

string draft_content_file = Path.Combine(projectDir, "draft_content.json");
await File.WriteAllTextAsync(draft_content_file, draft_content_json);

string draft_cover_file = Path.Combine(projectDir, "draft_cover.jpg");
File.Copy(".\\Resources\\cover.jpg", draft_cover_file, true);

RootMetaInfo rootMetaInfo = new RootMetaInfo()
{
    DraftIds = 2,
    RootPath = rootDir,
    AllDraftStore = new()
    {
        new()
        {
            DraftFoldPath = projectDir,
            DraftJsonFile =  draft_content_file,
            DraftName = projectName,
            DraftRootPath = rootDir,
        }
    },
};
await File.WriteAllTextAsync(root_meta_info_file, rootMetaInfo.GetCapcutJsonString());

DraftMetaInfo draftMetaInfo = DraftMetaInfo.Create(projectDir, "draft_cover.jpg", projectName, rootDir);
await File.WriteAllTextAsync(draft_meta_info_file, draftMetaInfo.GetJsonString());

Console.ReadLine();