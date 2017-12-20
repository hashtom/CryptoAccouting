using System.Collections.Generic;

namespace CoinBalance.CoreClass.APIClass
{
    public class ZaifBalance
    {
        public int success { get; set; }
        public Info return_ { get; set; }
        public string error { get; set; }

        public class Info
        {
            public Dictionary<string, decimal> funds { get; set; }
            public Dictionary<string, decimal> deposit { get; set; }
            public Dictionary<string, decimal> rights { get; set; }
            public int trade_count { get; set; }
            public int open_orders { get; set; }
            public int server_time { get; set; }
        }
    }
}