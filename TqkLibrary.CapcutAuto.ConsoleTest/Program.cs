using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using TqkLibrary.CapcutAuto;
using TqkLibrary.CapcutAuto.ConsoleTest;

//await CapcutAutoClickHelper.CloseWindowAsync();
//await GenerateResourceTest.TestAsync();

CapcutAutoClickHelper capcutAutoClickHelper = new CapcutAutoClickHelper();
try
{
    await capcutAutoClickHelper.OpenCapcutAsync();
    //await capcutAutoClickHelper.ClickProjectWhiteCoverAsync();
    await capcutAutoClickHelper.ClickExportAsync();
    await capcutAutoClickHelper.ExportAsync();
}
finally
{
    await CapcutAutoClickHelper.CloseWindowAsync();
}

