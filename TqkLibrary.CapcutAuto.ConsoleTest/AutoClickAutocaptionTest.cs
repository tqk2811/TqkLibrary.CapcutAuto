using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.CapcutAuto.ConsoleTest
{
    internal static class AutoClickAutocaptionTest
    {
        public static async Task TestAsync()
        {
            await GenerateResourceTest.TestAsync();
            CapcutAutoClickHelper capcutAutoClickHelper = new CapcutAutoClickHelper();
            try
            {
                await capcutAutoClickHelper.OpenCapcutAsync();
                await capcutAutoClickHelper.ClickProjectWhiteCoverAsync();

                await capcutAutoClickHelper.AutocaptionAsync();
            }
            finally
            {
                await CapcutAutoClickHelper.CloseWindowAsync();
            }
        }
    }
}
