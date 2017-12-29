using System.Collections.Generic;

namespace CoinBalance.CoreAPI
{
    public class ZaifPositions
    {
        public int success { get; set; }
        public Dictionary<string, position> return_ { get; set; }
        public string error { get; set; }

        public class position
        {
            public string group_id { get; set; }
            public string currency_pair { get; set; }
            public string action { get; set; }
            public decimal amount { get; set; }
            public decimal price { get; set; }
            public decimal limit { get; set; }
            public decimal stop { get; set; }
            public int timestamp { get; set; }
            public int term_end { get; set; }
            public decimal leverage { get; set; }
            public decimal fee_spent { get; set; }
            public int timestamp_closed { get; set; }
            public decimal price_avg { get; set; }
            public decimal amount_done { get; set; }
            public decimal close_avg { get; set; }
            public decimal close_done { get; set; }
            public decimal deposit_jpy { get; set; }
            public decimal deposit_price_jpy { get; set; }
            public decimal refunded_jpy { get; set; }
            public decimal refunded_price_jpy { get; set; }
            public decimal guard_fee { get; set; }
        }
    }
}