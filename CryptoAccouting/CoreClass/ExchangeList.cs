using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class ExchangeList : IEnumerable<Exchange>
    {
        private List<Exchange> exchanges;

        public ExchangeList()
        {
            exchanges = new List<Exchange>();
        }

        public TradeList GetTradelist(EnuExchangeType extype, string symbol)
        {
            if (exchanges == null ? false : exchanges.Where(x => x.ExchangeType == extype).Any())
            {
                return this.First(x => x.ExchangeType == extype).TradeList;
            }
            else
            {
                return null;
            }
        }

        public Exchange GetExchange(EnuExchangeType extype)
        {
            if (exchanges == null ? false : exchanges.Where(x => x.ExchangeType == extype).Any())
            {
                return exchanges.First(x => x.ExchangeType == extype);
            }
            else
            {
                return new Exchange(extype);
            }
        }

        public ExchangeList GetExchangesBySymbol(string symbol){

            var exchanged_applied = new ExchangeList();
            foreach (var exc in exchanges)
            {
                if (exc.ListedCoin.Any(x => x.Symbol == symbol)) exchanged_applied.AttachExchange(exc);
            }
            return exchanged_applied;

        }

        public void AttachExchange(Exchange exc)
		{
            if (exchanges.Any(x => x.ExchangeType == exc.ExchangeType)) DetachExchange(exc);
			exchanges.Add(exc);
		}

        public void DetachExchange(Exchange exc)
		{
            exchanges.RemoveAll(x => x.ExchangeType == exc.ExchangeType);
		}

        public Exchange GetExchangeByIndex(int indexNumber)
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
