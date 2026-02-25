using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using TqkLibrary.CapcutAuto;
using TqkLibrary.CapcutAuto.ConsoleTest;

await CapcutAutoClickHelper.CloseWindowAsync();
await AutoClickRenderTest.TestAsync();

await CapcutAutoClickHelper.CloseWindowAsync();
await AutoClickAutocaptionTest.TestAsync();
