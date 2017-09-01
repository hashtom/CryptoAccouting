using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class Instrument
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Symbol1 { get; set; }
        public string Symbol2 { get; set; }
        public InstrumentType Type { get; set; }
		public Price MarketPrice { get; set; }
        public string PriceSourceCode { get; set; }
		public int rank { get; set; }
        //public bool IsActive { get; set; }

        public Instrument(string id)
        {
            Id = id;
            //TEMP
            if (id == "bitcoin")
            {
                PriceSourceCode = "Bitstamp";
            }
            else
            {
                PriceSourceCode = "Bittrex";
            }
        }

        public Balance BalanceOnInstrument(Balance totalbalance)
		{
			if (totalbalance is null)
			{
				return null;
			}
			else
			{
				Balance bal = new Balance();
                foreach (var pos in totalbalance.Where(x => (x.Coin.Symbol1 == Symbol1 && x.Coin.Id == Id)))
				{
					bal.Attach(pos);
				}
				return bal;
			}
		}
    }

	public enum InstrumentType
	{
		Crypto,
		Cash,
        Other
	}
}
