using System;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Exceptions
{
    public class UnsupportedElementClassException : NetCgmException
    {
        public UnsupportedElementClassException(int elementClass) : base($"unsupported elementClass  = {elementClass}")
        {
        }
    }
}
