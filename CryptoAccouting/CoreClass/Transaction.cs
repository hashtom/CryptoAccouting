using System;
using System.Collections.Generic; 


namespace CryptoAccouting
{
    public class Transaction
    {
        public string txid { get; set; }
        public Instrument Coin { get; }
        public EnuExchangeType TradeExchange { get; }
        public string BuySell { get; set; }
        public double Amount { get; set; }
        public double TradePrice { get; set; }
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
}
