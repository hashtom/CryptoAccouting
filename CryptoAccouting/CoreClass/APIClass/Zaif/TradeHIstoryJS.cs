using System.Collections.Generic;

namespace CoinBalance.CoreClass.APIClass
{
    public class TradeHIstoryJS
    {
        public int success { get; set; }
        public Dictionary<string, trade> return_ { get; set; }
        public string error { get; set; }

        public class trade
        {
            public string currency_pair { get; set; }
            public string action { get; set; }
            public decimal amount { get; set; }
            public decimal price { get; set; }
            public decimal fee { get; set; }
            public decimal? fee_amount { get; set; }
            public string your_action { get; set; }
            public decimal bonus { get; set; }
            public int timestamp { get; set; }
            public string comment { get; set; }
        }
    }
}
