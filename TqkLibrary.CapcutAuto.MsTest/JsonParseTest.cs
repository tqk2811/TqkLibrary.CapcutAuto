using System.Threading.Tasks;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials;

namespace TqkLibrary.CapcutAuto.MsTest
{
    [TestClass]
    public sealed class JsonParseTest
    {
        [TestMethod]
        public async Task Test_CapcutMaterialText()
        {
            var file = new FileInfo(".\\CapcutDatas\\text.json");
            string json = await File.ReadAllTextAsync(file.FullName);

            CapcutMaterialText capcutMaterialText = CapcutMaterialText.Parse(json);
            string json_out = capcutMaterialText.GetJsonString();
        }


        [TestMethod]
        public async Task Test_CapcutMaterialEffectTextEffect()
        {
            var files = Directory.GetFiles(".\\CapcutDatas\\Effects");

            foreach (var file in files)
            {
                string json = await File.ReadAllTextAsync(file);
                CapcutMaterialEffectTextEffect capcutMaterialText = CapcutMaterialEffectTextEffect.Parse(json);
                string json_out = capcutMaterialText.GetJsonString();
            }
        }
    }
}
