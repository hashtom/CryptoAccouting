using System;
using System.Collections.Generic;


namespace CryptoAccouting.CoreClass
{
    public class Transaction
    {
        public string TxId { get; set; }
        public Instrument Coin { get; }
		public DateTime TradeDate { get; set; }
        public EnuExchangeType TradeExchange { get; } // to be changed to exchange type
        public EnuBuySell BuySell { get; set; }
        public EnuCCY BaseCurrency { get; set; }
        public EnuCrypto BaseCrypto { get; set; }
        public int Quantity { get; set; }
        public double TradePrice { get; set; }
        public int Fee { get; set; }
		public double BookPrice { get; set; }
		//public bool IsMargin { get; set; }
		//public DateTime UpdateTime { get; set; }

		public string Symbol {
            get { return Coin.Symbol; }
        }

		public string Side
		{
			get { return BuySell == EnuBuySell.Buy ? "Buy" : "Sell"; }
		}

        public double TradeValueBaseCCY
        {
            get { return TradePrice * Quantity; }
        }

        public double RealizedPLBaseCCY
        {
            get { return (BookPrice != 0) ? (TradePrice - BookPrice) * Quantity : 0; }
        }

        public Transaction(Instrument coin, EnuExchangeType exchange)
        {
            Coin = coin;
            TradeExchange = exchange;
            BookPrice = 0;
        }

    }

	public enum EnuBuySell
	{
		Buy,
		Sell,
        Check
	}
}
