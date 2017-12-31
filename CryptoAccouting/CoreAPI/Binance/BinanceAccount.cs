using System;
using System.Collections.Generic;

namespace CoinBalance.CoreAPI
{
    public class BinanceAccount
    {
        public decimal makerCommission { get; set; }
        public decimal takerCommission { get; set; }
        public decimal buyerCommission { get; set; }
        public decimal sellerCommission { get; set; }
        public bool canTrade { get; set; }
        public bool canWithdraw { get; set; }
        public bool canDeposit { get; set; }
        public List<balance> balances { get; set; }
    }

    public class balance
    {
        public string asset { get; set; }
        public decimal free { get; set; }
        public decimal locked { get; set; }
    }
}
