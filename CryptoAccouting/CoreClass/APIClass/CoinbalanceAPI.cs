using System;

namespace CoinBalance.CoreClass.APIClass
{
    public static class CoinbalanceAPI
    {

#if DEBUG
        public const string coinbalance_url = "https://coinbalance.jp/develop";
        //public const string coinbalance_url = "https://coinbalance.jp/v1.1";
#else
        public const string coinbalance_url = "https://coinbalance.jp/v1.1";
#endif

    }
}
