using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.CapcutAuto.Exceptions
{
    public class CapcutAutoException : Exception
    {
        public CapcutAutoException() { }
        public CapcutAutoException(string? message) : base(message) { }
    }
}
