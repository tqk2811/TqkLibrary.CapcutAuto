using FFMpegCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Helpers
{
    public class CapcutProjectHelper
    {
        public static string DraftRootPath { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "AppData/Local/CapCut/User Data/Projects/com.lveditor.draft"
            );


        readonly RootMetaInfo _rootMetaInfo;
        readonly DraftMetaInfo _draftMetaInfo;
        public CapcutProjectHelper(string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName)) throw new ArgumentNullException(nameof(projectName));
            _draftMetaInfo = new()
            {
                DraftCover = "draft_cover.jpg",
                DraftRootPath = DraftRootPath,
                DraftFolderPath = Path.Combine(DraftRootPath, projectName),
                DraftName = projectName,
            };
            _rootMetaInfo = new()
            {
                DraftIds = 2,
                RootPath = DraftRootPath,
                AllDraftStore = new()
                {
                    new()
                    {
                        DraftName = projectName,
                        DraftFoldPath = _draftMetaInfo.DraftFolderPath,
                        DraftJsonFile = Path.Combine(_draftMetaInfo.DraftFolderPath,"draft_content.json"),
                        DraftRootPath = DraftRootPath,
                        DraftCover = Path.Combine(_draftMetaInfo.DraftFolderPath,"draft_cover.jpg"),
                    }
                }
            };
        }

        public CapcutDraftContentHelper DraftContent { get; } = new();

        public async Task<DraftMetaInfo.DraftMaterialValueVideo> AddVideoFileAsync(string videoFilePath, CancellationToken cancellationToken = default)
        {
            FileInfo fileInfo = new(videoFilePath);
            if (!fileInfo.Exists)
                throw new FileNotFoundException(videoFilePath);

            var draftMaterial = _draftMetaInfo.DraftMaterials.First(x => x.Type == 0);
            if (draftMaterial.Value.Any(x => x.FilePath.Equals(fileInfo.FullName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("Dupp file input");
            }

            IMediaAnalysis mediaAnalysis = await FFProbe.AnalyseAsync(videoFilePath, cancellationToken: cancellationToken);
            if (mediaAnalysis.PrimaryVideoStream is null)
                throw new InvalidOperationException($"File had no VideoStream");

            DraftMetaInfo.DraftMaterialValueVideo value = new()
            {
                ExtraInfo = fileInfo.Name,
                FilePath = fileInfo.FullName,
                Height = mediaAnalysis.PrimaryVideoStream.Height,
                Width = mediaAnalysis.PrimaryVideoStream.Width,
                RoughcutTimeRange = new()
                {
                    Duration = mediaAnalysis.Duration,
                    Start = TimeSpan.Zero,
                },
                Duration = mediaAnalysis.Duration,
                HasAudio = mediaAnalysis.PrimaryAudioStream is not null,
            };
            draftMaterial.Value.Add(value);
            return value;
        }
        public async Task<DraftMetaInfo.DraftMaterialValueAudio> AddAudioFileAsync(string audioFilePath, CancellationToken cancellationToken = default)
        {
            FileInfo fileInfo = new(audioFilePath);
            if (!fileInfo.Exists)
                throw new FileNotFoundException(audioFilePath);

            var draftMaterial = _draftMetaInfo.DraftMaterials.First(x => x.Type == 0);
            if (draftMaterial.Value.Any(x => x.FilePath.Equals(fileInfo.FullName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("Dupp file input");
            }

            IMediaAnalysis mediaAnalysis = await FFProbe.AnalyseAsync(audioFilePath, cancellationToken: cancellationToken);
            if (mediaAnalysis.PrimaryAudioStream is null)
                throw new InvalidOperationException($"File had no AudioStream");

            DraftMetaInfo.DraftMaterialValueAudio value = new()
            {
                ExtraInfo = fileInfo.Name,
                FilePath = fileInfo.FullName,
                RoughcutTimeRange = new()
                {
                    Duration = mediaAnalysis.Duration,
                    Start = TimeSpan.Zero,
                },
                Duration = mediaAnalysis.Duration,
            };
            draftMaterial.Value.Add(value);
            return value;
        }


        public async Task CleanupProjectAsync(CancellationToken cancellationToken = default)
        {
            if (Directory.Exists(_draftMetaInfo.DraftFolderPath))
            {
                await Task.Factory.StartNew(() => Directory.Delete(_draftMetaInfo.DraftFolderPath, true), TaskCreationOptions.LongRunning);
            }
            Directory.CreateDirectory(_draftMetaInfo.DraftFolderPath);
        }

        public async Task WriteProjectAsync(CancellationToken cancellationToken = default)
        {
            //update _rootMetaInfo and _draftMetaInfo from DraftContent
            if (Directory.Exists(_draftMetaInfo.DraftFolderPath))
                Directory.Delete(_draftMetaInfo.DraftFolderPath, true);
            Directory.CreateDirectory(_draftMetaInfo.DraftFolderPath);
            using (Stream cover_stream = Extensions.GetEmbeddedResourceStream("cover.jpg"))
            {
                using FileStream fileStream = new FileStream(
                    Path.Combine(_draftMetaInfo.DraftFolderPath, "draft_cover.jpg"),
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.Read
                    );
                await cover_stream.CopyToAsync(fileStream, cancellationToken);
            }

            await File.WriteAllTextAsync(
                Path.Combine(DraftRootPath, "root_meta_info.json"),
                _rootMetaInfo.GetCapcutJsonString(),
                cancellationToken
                );
            await File.WriteAllTextAsync(
                Path.Combine(_draftMetaInfo.DraftFolderPath, "draft_meta_info.json"),
                _draftMetaInfo.GetCapcutJsonString(),
                cancellationToken
                );
            await File.WriteAllTextAsync(
                Path.Combine(_draftMetaInfo.DraftFolderPath, "draft_content.json"),
                DraftContent.BuildJson(),
                cancellationToken
                );
        }
    }
}
