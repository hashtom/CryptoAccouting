using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class InstrumentList : IEnumerable<Instrument>
    {
        private List<Instrument> instruments;

        public InstrumentList()
        {
            instruments = new List<Instrument>();
        }

        public void Attach(Instrument coin)
		{
            if (instruments.Any(x => x.Id == coin.Id)) Detach(coin);
			instruments.Add(coin);
		}

		public void Detach(Instrument coin)
		{
            instruments.RemoveAll(x => x.Id == coin.Id);
		}

        public void Insert(int i, Instrument coin)
        {
            instruments.Insert(i, coin);
        }

		public Instrument GetByIndex(int indexNumber)
		{
			return instruments[indexNumber];
		}

        public Instrument GetByInstrumentId(string instrumentid)
        {
            return instruments.Any(x => x.Id == instrumentid) ? instruments.First(x => x.Id == instrumentid) : null;
        }

        public void DetachByInstrumentId(string instrumentid)
        {
            if (instruments.Any(x => x.Id == instrumentid))
                Detach(instruments.First(x => x.Id == instrumentid));
        }

		public Instrument GetBySymbol1(string symbol)
		{
            return instruments.Any(x => x.Symbol1 == symbol) ? instruments.First(x => x.Symbol1 == symbol) : null;
		}

		public Instrument GetBySymbol2(string symbol)
		{
			return instruments.Any(x => x.Symbol2 == symbol) ? instruments.First(x => x.Symbol2 == symbol) : null;
		}

        //public void AttachCrossRate(CrossRate usdcrossrate)
        //{
        //    foreach (var coin in instruments.Where(x => x.MarketPrice != null))
        //    {
        //        coin.MarketPrice.USDCrossRate = usdcrossrate;
        //    }
        //}

        public void Clear()
        {
            instruments.Clear();
        }

		public int Count()
		{
			return instruments.Count();
		}

        public IEnumerator<Instrument> GetEnumerator()
		{
			for (int i = 0; i <= instruments.Count - 1; i++) yield return instruments[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
    }
}
