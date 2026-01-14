using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Linq;

namespace TqkLibrary.CapcutAuto.ResourceTracker.Helpers
{
    internal abstract class BaseHelper
    {
        protected static readonly string userprofile;
        protected static readonly string CapcutDatasDir;
        protected static readonly string AnimationsDir;
        protected static readonly string TransitionsDir;
        protected static readonly string EffectsDir;
        protected static readonly string TextEffectsDir;
        protected static readonly string TextShapesDir;
        protected static readonly string StickersDir;
        protected static readonly string TextsDir;
        protected static readonly string AudiosDir;
        protected static readonly string FontsDir;
        static BaseHelper()
        {
            userprofile = Environment.ExpandEnvironmentVariables("%userprofile%").Replace('\\', '/');
            CapcutDatasDir = Path.Combine(AppContext.BaseDirectory, "CapcutDatas");
            AnimationsDir = Path.Combine(CapcutDatasDir, "Animations");
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

            TransitionsDir = Path.Combine(CapcutDatasDir, "Transitions");
            Directory.CreateDirectory(TransitionsDir);

            EffectsDir = Path.Combine(CapcutDatasDir, "Effects");

            TextEffectsDir = Path.Combine(EffectsDir, "TextEffects");
            Directory.CreateDirectory(TextEffectsDir);

            TextShapesDir = Path.Combine(EffectsDir, "TextShapes");
            Directory.CreateDirectory(TextShapesDir);

            StickersDir = Path.Combine(CapcutDatasDir, "Stickers");
            Directory.CreateDirectory(StickersDir);

            TextsDir = Path.Combine(CapcutDatasDir, "Texts");
            Directory.CreateDirectory(TextsDir);

            AudiosDir = Path.Combine(CapcutDatasDir, "Audios");
            Directory.CreateDirectory(AudiosDir);

            FontsDir = Path.Combine(CapcutDatasDir, "Fonts");
            Directory.CreateDirectory(FontsDir);
        }


        public abstract Task ParseAsync(JObject data);
    }
}
