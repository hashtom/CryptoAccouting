using System;
using System.Collections.Generic;

namespace CoinBalance.CoreAPI
{
    public class BitbankAsset
    {
        public int success { get; set; }
        public Assets _data { get; set; }
        public DateTime timestamp { get; set; }

        public class Assets
        {
            public List<Asset> assets { get; set; }
            public int code { get; set; }
        }

        public class Asset
        {
            public string asset { get; set; }
            public int amount_precision { get; set; }
            public decimal onhand_amount { get; set; }
            public decimal locked_amount { get; set; }
            public decimal free_amount { get; set; }
            public bool stop_deposit { get; set; }
            public bool stop_withdrawal { get; set; }
            //public Fee withdrawal_fee { get; set; }
            //public decimal withdrawal_fee { get; set; }
        }

        //public class Fee
        //{
        //    public decimal threshold { get; set; }
        //    public decimal under { get; set; }
        //    public decimal over { get; set; }
        //}

    }
}
