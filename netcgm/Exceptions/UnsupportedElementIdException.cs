using System;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Exceptions
{
    public class UnsupportedElementIdException : NetCgmException
    {
        public UnsupportedElementIdException(int elementId) : base($"unsupported elementId = {elementId}")
        {
        }
    }
}
