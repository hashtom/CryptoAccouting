using System;
using System.Collections.Generic;

namespace CoinBalance.CoreAPI
{
    public class CoinCheckTransactions
    {
        public bool success { get; set; }
        public Pagination pagination { get; set; }
        public List<transaction> data { get; set; }

        public class Pagination
        {
            public int limit { get; set; }
            public string order { get; set; }
            public string starting_after { get; set; }
            public string ending_before { get; set; }
        }

        public class Funds
        {
            public decimal btc { get; set; }
            public decimal jpy { get; set; }
            public decimal Bch { get; set; }
            public decimal Eth { get; set; }
            public decimal Etc { get; set; }
            public decimal Lsk { get; set; }
            public decimal Xmr { get; set; }
            public decimal Rep { get; set; }
            public decimal Xrp { get; set; }
            public decimal Zec { get; set; }
            public decimal Xem { get; set; }
            public decimal Ltc { get; set; }
            public decimal Dash { get; set; }
        }

        public class transaction
        {
            public string id { get; set; }
            public string order_id { get; set; }
            public string created_at { get; set; }
            public Funds funds { get; set; }
            public string pair { get; set; }
            public decimal rate { get; set; }
            public string fee_currency { get; set; }
            public decimal fee { get; set; }
            public string liquidity { get; set; }
            public string side { get; set; }
        }
    }
}