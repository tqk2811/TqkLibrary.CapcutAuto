using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Enums
{
    [Flags]
    public enum TextCheckFlag : int
    {
        Blend = 4,
        Stroke = 8,
        Background = 16,
        Shadow = 32,
        Glow = 64,
        Curve = 128,//should disable Background
    }
}
