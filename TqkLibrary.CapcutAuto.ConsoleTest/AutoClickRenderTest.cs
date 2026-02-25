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
            }
            finally
            {
                await CapcutAutoClickHelper.CloseWindowAsync();
            }
        }
    }
}
