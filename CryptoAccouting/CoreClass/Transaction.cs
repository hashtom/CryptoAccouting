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
        public string Side
        {
            get { return BuySell == EnuBuySell.Buy ? "Buy" : "Sell"; }
        }
        public double Amount { get; set; }
        public double TradePrice { get; set; }
        public int Fee { get; set; }
        public bool IsMargin { get; set; }
        public DateTime TradeDate { get; set; }
        public DateTime UpdateTime { get; set; }

        public string CoinName {
            get { return Coin.Symbol; }
            set { this.Coin.Symbol = value; }
        }

        public double TradeValue
        {
            get { return TradePrice * Amount; }
        }

        public Transaction(Instrument coin, EnuExchangeType exchange)
        {
			Coin = coin;
            TradeExchange = exchange;
			UpdateTime = DateTime.Now;
        }
    }

	public enum EnuBuySell
	{
		Buy,
		Sell,
        Check
	}
}
