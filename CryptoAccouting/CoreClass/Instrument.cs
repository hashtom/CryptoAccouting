using System;
using System.Collections.Generic;

namespace CryptoAccouting.CoreClass
{
    public class Instrument
    {
        public string Id { get; } //CoinMarketCap ID
        public string Symbol { get; }
        public string Name { get; set; }
        public InstrumentType Type { get; set; }
        public DateTime ListedDate { get; set; }
		public Price MarketPrice { get; set; }
        public string LogoFileName { get; set; }

        public Instrument(string id, string symbol)
        {
            Id = id;
            Symbol = symbol;
        }
    }

	public enum InstrumentType
	{
		Crypto,
		Cash,
        Other
	}
}
