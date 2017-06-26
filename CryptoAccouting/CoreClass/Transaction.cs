using System;
using System.Collections.Generic; 


namespace CryptoAccouting
{
    public class Transaction
    {
        public Instrument Coin { get; }
        public Exchange TradeExchange { get; }
        public string BuySell { get; set; }
        public double Amount { get; set; }
        public double TradePrice { get; set; }
        public bool IsMargin { get; set; }
        public DateTime UpdateTime { get; private set; }

        public Transaction(Instrument coin, Exchange exchange)
        {
			Coin = coin;
            TradeExchange = exchange;
			UpdateTime = DateTime.Now;
        }
    }
}
