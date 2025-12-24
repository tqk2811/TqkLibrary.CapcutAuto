

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
CapcutMaterialText capcutMaterialText = CapcutMaterialText.CreateDefault();


FileInfo videoFileInfo = new FileInfo(".\\Resources\\OriginalVideo.mp4");
FileInfo audioFileInfo = new FileInfo(".\\Resources\\tts_000.ogg");

#if REMOTEDEBUG
videoFileInfo = videoFileInfo.CopyTo("C:\\OriginalVideo.mp4", true);
audioFileInfo = audioFileInfo.CopyTo("C:\\tts_000.ogg", true);
#endif

CapcutProjectHelper capcutProjectHelper = new CapcutProjectHelper("myproj");

var materialVideo = await capcutProjectHelper.AddVideoFileAsync(videoFileInfo.FullName);
var materialAudio = await capcutProjectHelper.AddAudioFileAsync(audioFileInfo.FullName);


capcutProjectHelper.DraftContent.CapcutTracks.Add(new CapcutTrackVideo()
{
    new CapcutSegmentVideo()
    {
        MaterialVideo = materialVideo.CreateMaterial(),
        TargetTimerange = new()
        {
            Start = TimeSpan.Zero,
            Duration = TimeSpan.FromSeconds(5),
        },
        Speed = 1.0,
        Volume = 0.0,
        MaterialTransition = capcutMaterialTransitions[Random.Shared.Next(capcutMaterialTransitions.Count)]
            .CloneWithRandomId()
            .SetProp(x => x.Duration = TimeSpan.FromSeconds(1.5)),
    },
    new CapcutSegmentVideo()
    {
        MaterialVideo = materialVideo.CreateMaterial(),
        TargetTimerange = new()
        {
            Start = TimeSpan.FromSeconds(25),
            Duration = TimeSpan.FromSeconds(5),
        },
        Speed = 1.0,
        Volume = 0.0,
        MaterialTransition = capcutMaterialTransitions[Random.Shared.Next(capcutMaterialTransitions.Count)]
            .CloneWithRandomId()
            .SetProp(x => x.Duration = TimeSpan.FromSeconds(2.5)),
    },
    new CapcutSegmentVideo()
    {
        MaterialVideo = materialVideo.CreateMaterial(),
        TargetTimerange = new()
        {
            Start = TimeSpan.FromSeconds(45),
            Duration =TimeSpan.FromSeconds(5),
        },
        Speed = 1.0,
        Volume = 0.0,
    },
});
capcutProjectHelper.DraftContent.CapcutTracks.Add(new CapcutTrackAudio()
{
    new CapcutSegmentAudio()
    {
        MaterialAudio = materialAudio.CreateMaterial(),
        TargetTimerange = new()
        {
            Start = TimeSpan.Zero,
            Duration = materialAudio.Duration,
        },
        Volume = 1.0,
        Speed = 1.0,
    }
});

var text0 = capcutMaterialText.CloneWithRandomId();
text0.ContentHelper.Text = "Đây là văn bản A";
text0.ContentHelper.Styles.First().Range = new() { 0, text0.ContentHelper.Text.Length };
var text1 = capcutMaterialText.CloneWithRandomId();
text1.ContentHelper.Text = "Đây là văn bản GHI";
text1.ContentHelper.Styles.First().Range = new() { 0, text1.ContentHelper.Text.Length };
var text2 = capcutMaterialText.CloneWithRandomId();
text2.ContentHelper.Text = "Đây là văn bản sdsadsadsadsa";
text2.ContentHelper.Styles.First().Range = new() { 0, text2.ContentHelper.Text.Length };
capcutProjectHelper.DraftContent.CapcutTracks.Add(new CapcutTrackText()
{
    new CapcutSegmentText()
    {
        MaterialText = text0,
        TargetTimerange = new()
        {
            Start = TimeSpan.Zero,
            Duration = TimeSpan.FromSeconds(5),
        },
        Volume = 0.0,
        MaterialAnimation = new()
        {
            In = in_animations[Random.Shared.Next(in_animations.Count)]
                .Clone()
                .SetProp(x => x.Duration = TimeSpan.FromSeconds(2)),
            Out = out_animations[Random.Shared.Next(out_animations.Count)]
                .Clone()
                .SetProp(x => x.Duration = TimeSpan.FromSeconds(2)),
        },
    },
    new CapcutSegmentText()
    {
        MaterialText = text1,
        TargetTimerange = new()
        {
            Start = TimeSpan.FromSeconds(5),
            Duration = TimeSpan.FromSeconds(5),
        },
        Volume = 0.0,
        MaterialAnimation = new()
        {
            In = in_animations[Random.Shared.Next(in_animations.Count)]
                .Clone()
                .SetProp(x => x.Duration = TimeSpan.FromSeconds(1.5)),
            Out = out_animations[Random.Shared.Next(out_animations.Count)]
                .Clone()
                .SetProp(x => x.Duration = TimeSpan.FromSeconds(1.5)),
        },
    },
    new CapcutSegmentText()
    {
        MaterialText = text2,
        TargetTimerange = new()
        {
            Start = TimeSpan.FromSeconds(10),
            Duration = TimeSpan.FromSeconds(5),
        },
        Volume = 0.0,
        MaterialAnimation = new()
        {
            In = in_animations[Random.Shared.Next(in_animations.Count)]
                .Clone()
                .SetProp(x => x.Duration = TimeSpan.FromSeconds(1)),
            Out = out_animations[Random.Shared.Next(out_animations.Count)]
                .Clone()
                .SetProp(x => x.Duration = TimeSpan.FromSeconds(1)),
        },
    },
});

await capcutProjectHelper.WriteProjectAsync();

Console.ReadLine();