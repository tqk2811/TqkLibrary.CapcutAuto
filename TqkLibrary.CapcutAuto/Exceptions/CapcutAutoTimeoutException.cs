using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.CapcutAuto.Exceptions
{
    public class CapcutAutoTimeoutException : Exception
    {
        public CapcutAutoTimeoutException() { }
        public CapcutAutoTimeoutException(string? message) : base(message) { }
    }
}
