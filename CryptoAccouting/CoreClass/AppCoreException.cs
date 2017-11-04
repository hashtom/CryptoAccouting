using System;
namespace CryptoAccouting.CoreClass
{
    public class AppCoreException : Exception
    {
        public AppCoreException(string message) : base(message)
        {
        }
    }

    public class AppCoreParseException : Exception
    {
        public AppCoreParseException(string message) : base(message)
        {
        }
    }

    public class AppCoreNetworkException : Exception
    {
        public AppCoreNetworkException(string message) : base(message)
        {
        }
    }

}
