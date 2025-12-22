

using TqkLibrary.CapcutAuto;
using TqkLibrary.CapcutAuto.Helpers;
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
CapcutMaterialVideo capcutMaterialVideo = await CapcutMaterialVideo.CreateAsync(videoFileInfo.FullName);

FileInfo audioFileInfo = new FileInfo(".\\Resources\\tts_000.ogg");
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
            MaterialTransition = capcutMaterialTransitions.First(),
            TargetTimerange = new()
            {

            },
            Speed = 1.0,

            Volume = 0.0,
        }
    },
    new CapcutTrackText()
    {

    },
    new CapcutTrackAudio()
    {

    }
};
CapcutDraftContentHelper capcutDraftContentHelper = new(capcutTracks);
string json = capcutDraftContentHelper.BuildJson();
Console.ReadLine();