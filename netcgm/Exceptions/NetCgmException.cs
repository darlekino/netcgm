using System;

namespace netcgm.Exceptions
{
    public class NetCgmException : Exception
    {
        public NetCgmException()
        {
        }

        public NetCgmException(string message) : base(message)
        {
        }

        public NetCgmException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
