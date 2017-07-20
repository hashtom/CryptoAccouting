using System;
using System.Collections.Generic;

namespace CryptoAccouting.CoreClass
{
    public class Exchange
    {
        public string ExchangeName { get; set; }
        public EnuExchangeType ExchangeType { get; }
        public TradeList TradeList;

        public DateTime UpdateTime { get; private set; }

        public Exchange(EnuExchangeType excType)
        {
            ExchangeType = excType;
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
