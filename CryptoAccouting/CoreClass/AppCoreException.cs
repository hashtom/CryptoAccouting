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

    public class AppCoreStorageException : Exception
    {
        public AppCoreStorageException(string message) : base(message)
        {
        }
    }

    public class AppCoreInstrumentException : Exception
    {
        public AppCoreInstrumentException(string message) : base(message)
        {
        }
    }

    public class AppCoreExchangeException : Exception
    {
        public AppCoreExchangeException(string message) : base(message)
        {
        }
    }

    public class AppCoreBalanceException : Exception
    {
        public AppCoreBalanceException(string message) : base(message)
        {
        }
    }

    public class AppCoreWarning : Exception
    {
        public AppCoreWarning(string message) : base(message)
        {
        }
    }
}
