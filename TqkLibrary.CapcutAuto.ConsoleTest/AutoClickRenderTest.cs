namespace TqkLibrary.CapcutAuto.ConsoleTest
{
    internal static class AutoClickRenderTest
    {
        public static async Task TestAsync()
        {
            await GenerateResourceTest.TestAsync();
            CapcutAutoClickHelper capcutAutoClickHelper = new CapcutAutoClickHelper();
            try
            {
                await capcutAutoClickHelper.OpenCapcutAsync();
                await capcutAutoClickHelper.ClickProjectWhiteCoverAsync();
                await capcutAutoClickHelper.ClickExportAsync();
                await capcutAutoClickHelper.ExportRenderAsync();
            }
            finally
            {
                await CapcutAutoClickHelper.CloseWindowAsync();
            }
        }
    }
}
