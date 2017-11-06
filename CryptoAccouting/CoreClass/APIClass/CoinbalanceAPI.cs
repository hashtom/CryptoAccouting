using System;

namespace CoinBalance.CoreClass.APIClass
{
    public static class CoinbalanceAPI
    {

#if DEBUG
        public const string coinbalance_url = "https://coinbalance.jp/develop";
#else
        public const string coinbalance_url = "https://coinbalance.jp";
#endif

    }
}
