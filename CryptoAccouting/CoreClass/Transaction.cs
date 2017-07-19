using System;
using System.Collections.Generic;


namespace CryptoAccouting.CoreClass
{
    public class Transaction
    {
        public string TxId { get; set; }
        public Instrument Coin { get; }
        public EnuExchangeType TradeExchange { get; }
        public EnuBuySell BuySell { get; set; }
        public int Quantity { get; set; }
        public double TradePrice { get; set; }
        public int Fee { get; set; }
        public bool IsMargin { get; set; }
        public DateTime TradeDate { get; set; }
        //public DateTime UpdateTime { get; set; }

        public string Symbol {
            get { return Coin.Symbol; }
        }

		public string Side
		{
			get { return BuySell == EnuBuySell.Buy ? "Buy" : "Sell"; }
		}

        public double TradeValue
        {
            get { return TradePrice * Quantity; }
        }

        public Transaction(Instrument coin, EnuExchangeType exchange)
        {
			Coin = coin;
            TradeExchange = exchange;
			//UpdateTime = DateTime.Now;
        }
    }

	public enum EnuBuySell
	{
		Buy,
		Sell,
        Check
	}
}
