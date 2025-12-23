using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.CapcutAuto.Models;
using TqkLibrary.CapcutAuto.Models.Tracks;

namespace TqkLibrary.CapcutAuto.Helpers
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




        public async Task WriteProjectAsync(CancellationToken cancellationToken = default)
        {
            //update _rootMetaInfo and _draftMetaInfo from DraftContent



            Directory.CreateDirectory(_draftMetaInfo.DraftFolderPath);

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
