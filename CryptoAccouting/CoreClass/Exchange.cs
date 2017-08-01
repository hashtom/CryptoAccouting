using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class Exchange
    {
        public string ExchangeName { get; set; }
        public EnuExchangeType ExchangeType { get; }
        public List<Instrument> ListedCoin { get; set; }
        public TradeList TradeList { get; set; } //If needed?
        //public Balance Balance { get; set; }     // If needed?
        public string LogoFileName { get; set; }
        public DateTime UpdateTime { get; private set; }

        public Exchange(EnuExchangeType excType)
        {
            ExchangeType = excType;
            ListedCoin = new List<Instrument>();
        }

		public void AttachListedCoin(Instrument coin)
		{
            if (!ListedCoin.Any(c => c.Symbol == coin.Symbol)) ListedCoin.Add(coin);
		}
    }

	public enum EnuExchangeType
	{
        NotSelected = 0,
		Zaif = 1,
		BitFlyer = 2,
		Kraken = 3,
		CoinCheck = 4,
		BitBank = 5,
        Poloniex = 6,
        Bittrex = 7
	}
}
