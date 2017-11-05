using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CoinBalance.CoreClass
{
    public class ExchangeList : IEnumerable<Exchange>
    {
        private List<Exchange> exchanges;

        public ExchangeList()
        {
            exchanges = new List<Exchange>();
        }

        public TradeList GetTradelist(string exchangecode)
        {
            if (exchanges != null && exchangecode != "" && exchanges.Any(x => x.Code == exchangecode))
            {
                return this.First(x => x.Code == exchangecode).TradeList;
            }
            else
            {
                throw new AppCoreException("No Tradelist");
            }
        }

        public Exchange GetExchange(string exchangecode)
        {
            if (exchangecode != "")
            {
                if (!exchanges.Any(x => x.Code == exchangecode))
                {
                    this.Attach(new Exchange(exchangecode, EnuCoinStorageType.Exchange));
                }

                return exchanges.First(x => x.Code == exchangecode);
            }
            else
            {
                throw new AppCoreException("No Exchange");
            }
        }

        //      public Exchange GetExchange(string ExchangeName)
        //{
        //          if (exchanges == null ? false : exchanges.Where(x => x.ExchangeName == ExchangeName).Any())
        //	{
        //		return exchanges.First(x => x.ExchangeName == ExchangeName);
        //	}
        //	else
        //	{
        //		return null;
        //	}
        //}

        public ExchangeList GetExchangesByInstrument(string id)
        {
            var exchanged_applied = new ExchangeList();
            foreach (var exc in exchanges)
            {
                if (exc.IsListed(id)) exchanged_applied.Attach(exc);
            }
            return exchanged_applied;
        }

        public void ClearAPIKeys()
        {
            foreach (var ex in exchanges.Where(x=>x.APIKeyAvailable()))
            {
                ex.ClearAPIKeys();
            }
        }

        public void Attach(Exchange exc)
		{
            if (exchanges.Any(x => x.Code == exc.Code)) Detach(exc);
			exchanges.Add(exc);
		}

        public void Detach(Exchange exc)
		{
            exchanges.RemoveAll(x => x.Code == exc.Code);
		}

        public Exchange GetByIndex(int indexNumber)
		{
			return exchanges[indexNumber];
		}

		public int Count()
		{
			return exchanges.Count;
		}

        public IEnumerator<Exchange> GetEnumerator()
		{
			for (int i = 0; i <= exchanges.Count - 1; i++) yield return exchanges[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
    }


}
