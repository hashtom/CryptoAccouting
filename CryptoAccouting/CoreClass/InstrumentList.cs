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
            if (instruments.Any(x => x.Symbol == coin.Symbol)) Detach(coin);
			instruments.Add(coin);
		}

		public void Detach(Instrument coin)
		{
            instruments.RemoveAll(x => x.Symbol == coin.Symbol);
		}

        public void Insert(int i, Instrument coin)
        {
            instruments.Insert(i, coin);
        }

		public Instrument GetByIndex(int indexNumber)
		{
			return instruments[indexNumber];
		}

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
