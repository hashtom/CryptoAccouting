using System;
using System.Collections.Generic;

namespace CoinBalance.CoreAPI
{
    public class BitbankTradeHistory
    {
        public int success { get; set; }
        public List<BitbankTrade> _data { get; set; }
        public DateTime timestamp { get; set; }

        public class BitbankTrade
        {
            public string trade_id { get; set; }
            public string pair { get; set; }
            public string order_id { get; set; }
            public string side { get; set; }
            public string type { get; set; }
            public decimal amount { get; set; }
            public decimal price { get; set; }
            public string maker_taker { get; set; }
            public decimal fee_amount_base { get; set; }
            public decimal fee_amount_quote { get; set; }
            public DateTime executed_at { get; set; }
            public int code { get; set; }
        }
    }
}
