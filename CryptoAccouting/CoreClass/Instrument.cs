using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class Instrument
    {
        public string Id { get; } //CoinMarketCap ID
        public int rank { get; set; }
        public string Symbol { get; }
        public string Name { get; }
        public InstrumentType Type { get; set; }
		public Price MarketPrice { get; set; }
        public bool IsActive { get; set; }

        public Instrument(string id, string symbol, string name)
        {
            Id = id;
            Symbol = symbol;
            Name = name;
            IsActive = true;
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
                foreach (var pos in totalbalance.Where(x => (x.Coin.Symbol == Symbol && x.Coin.Id == Id)))
				{
					bal.Attach(pos);
				}
				return bal;
			}
		}

        //public Position TotalPosition()
        //{

        //    if (ApplicationCore.Balance is null)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        Position totalpos = new Position(ApplicationCore.GetInstrument(Symbol))
        //        {
        //            Id = 0,
        //            Amount = ApplicationCore.Balance.Where(x => x.Coin.Symbol == Symbol).Sum(x => x.Amount),
        //            BookPriceUSD = ApplicationCore.Balance.Where(x => x.Coin.Symbol == Symbol).Average(x => (x.Amount * x.BookPriceUSD) / x.Amount)
        //        };

        //        return totalpos;
        //    }
        //}
    }

	public enum InstrumentType
	{
		Crypto,
		Cash,
        Other
	}
}
