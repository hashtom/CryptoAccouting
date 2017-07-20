using System;
using System.Collections.Generic;


namespace CryptoAccouting.CoreClass
{
    public class Transaction
    {
        public string TxId { get; set; }
        public Instrument Coin { get; }
		public DateTime TradeDate { get; set; }
        public Exchange TradeExchange { get; }
        public EnuBuySell BuySell { get; set; }
        public EnuCCY BaseCCY { get; set; }
        public EnuCrypto TradedCCY { get; set; } //Always Crypto
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

        public double TradeValueBase
        {
            get { return TradePrice * Quantity; }
        }

        public double TradeValue(EnuCCY baseFiatCCY)
        {
            // JPY<->MONA
            // BTC<->REP
            return 0;
        }

        public double RealizedPLBase
        {
            // calculate Realized PL when one side of trase is Base Fiat Currency
            // ignore trades both sides are Crypto for Realized PL calculation 
            get { return (BuySell == EnuBuySell.Sell) ? (TradePrice - BookPrice) * Quantity : 0; }
        }

        public double RelizedPL(EnuCCY baseFiatCCY)
        {
            return 0;
        }

        public Transaction(Instrument coin, Exchange exchange)
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
