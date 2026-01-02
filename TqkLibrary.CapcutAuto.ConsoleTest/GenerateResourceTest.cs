using TqkLibrary.CapcutAuto.ResourceGenerate;
using TqkLibrary.CapcutAuto.ResourceGenerate.Helpers;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Animations;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks.Segments;

namespace TqkLibrary.CapcutAuto.ConsoleTest
{
    internal static class GenerateResourceTest
    {
        public static async Task TestAsync()
        {
            List<CapcutMaterialTransition> capcutMaterialTransitions = new List<CapcutMaterialTransition>();
            foreach (var file in Directory.GetFiles(".\\CapcutDatas\\Transitions", "*.json"))
            {
                string json_text = await File.ReadAllTextAsync(file);
                capcutMaterialTransitions.Add(CapcutMaterialTransition.Parse(json_text));
            }
            IReadOnlyList<CapcutAnimationText> text_in_animations = await CapcutAnimationText.FromDirAsync(".\\CapcutDatas\\Animations\\Texts\\Ins");
            IReadOnlyList<CapcutAnimationText> text_loop_animations = await CapcutAnimationText.FromDirAsync(".\\CapcutDatas\\Animations\\Texts\\Loops");
            IReadOnlyList<CapcutAnimationText> text_out_animations = await CapcutAnimationText.FromDirAsync(".\\CapcutDatas\\Animations\\Texts\\Outs");

            IReadOnlyList<CapcutAnimationVideo> video_in_animations = await CapcutAnimationVideo.FromDirAsync(".\\CapcutDatas\\Animations\\Videos\\Ins");
            IReadOnlyList<CapcutAnimationVideo> video_loop_animations = await CapcutAnimationVideo.FromDirAsync(".\\CapcutDatas\\Animations\\Videos\\Loops");
            IReadOnlyList<CapcutAnimationVideo> video_out_animations = await CapcutAnimationVideo.FromDirAsync(".\\CapcutDatas\\Animations\\Videos\\Outs");

            IReadOnlyList<CapcutAnimationSticker> sticker_in_animations = await CapcutAnimationSticker.FromDirAsync(".\\CapcutDatas\\Animations\\Stickers\\Ins");
            IReadOnlyList<CapcutAnimationSticker> sticker_loop_animations = await CapcutAnimationSticker.FromDirAsync(".\\CapcutDatas\\Animations\\Stickers\\Loops");
            IReadOnlyList<CapcutAnimationSticker> sticker_out_animations = await CapcutAnimationSticker.FromDirAsync(".\\CapcutDatas\\Animations\\Stickers\\Outs");


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
                        In = text_in_animations[Random.Shared.Next(text_in_animations.Count)]
                            .Clone()
                            .SetProp(x => x.Duration = TimeSpan.FromSeconds(2)),
                        Out = text_out_animations[Random.Shared.Next(text_out_animations.Count)]
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
                        In = text_in_animations[Random.Shared.Next(text_in_animations.Count)]
                            .Clone()
                            .SetProp(x => x.Duration = TimeSpan.FromSeconds(1.5)),
                        Out = text_out_animations[Random.Shared.Next(text_out_animations.Count)]
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
                        In = text_in_animations[Random.Shared.Next(text_in_animations.Count)]
                            .Clone()
                            .SetProp(x => x.Duration = TimeSpan.FromSeconds(1)),
                        Out = text_out_animations[Random.Shared.Next(text_out_animations.Count)]
                            .Clone()
                            .SetProp(x => x.Duration = TimeSpan.FromSeconds(1)),
                    },
                },
            });

            await capcutProjectHelper.CleanupProjectAsync();

            await capcutProjectHelper.WriteProjectAsync();

        }
    }
}
