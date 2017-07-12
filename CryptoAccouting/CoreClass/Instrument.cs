using System;
using System.Collections.Generic;

namespace CryptoAccouting.CoreClass
{
    public class Instrument
    {
        public string Symbol { get; }
        public string Name { get; set; }
        public InstrumentType Type { get; set; }
        public DateTime ListedDate { get; set; }
		public Price MarketPrice { get; set; }
        public string LogoFileName { get; set; }

        public Instrument(string symbol)
        {
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
