using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class Exchange
    {
        public string ExchangeName { get; set; }
        public EnuExchangeType ExchangeType { get; }
        public List<TradeList> TradeLists;

        public DateTime UpdateTime { get; private set; }

        public Exchange(EnuExchangeType excType)
        {
            ExchangeType = excType;
            TradeLists = new List<TradeList>();
        }

        public TradeList GetTradeList(string symbol)
        {
            return TradeLists.Count == 0 ? null : TradeLists.Where(x => x.TradedCoin.Symbol == symbol).First();
        }

    }

	public enum EnuExchangeType
	{
		Zaif = 1,
		BitFlyer = 2,
		Kraken = 3,
		CoinCheck = 4,
		BitBank = 5
	}
}
