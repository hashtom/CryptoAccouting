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

        public TradeList GetTradelist(string exchangeCode)
        {
            if (exchanges == null ? false : exchanges.Any(x => x.Code == exchangeCode))
            {
                return this.First(x => x.Code == exchangeCode).TradeList;
            }
            else
            {
                return null;
            }
        }

        public Exchange GetExchange(string Code)
        {
            if (!exchanges.Any(x => x.Code == Code))
            {
                this.Attach(new Exchange(Code, EnuCoinStorageType.Exchange));
            }

            return exchanges.First(x => x.Code == Code);
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

        public ExchangeList GetExchangesBySymbol(string symbol){

            var exchanged_applied = new ExchangeList();
            //exchanged_applied.AttachExchange(new Exchange("NotSpecified") { Name = "Not Specified" });

            foreach (var exc in exchanges)
            {
                if (exc.ListedCoin.Any(x => x.Symbol == symbol)) exchanged_applied.Attach(exc);
            }
            return exchanged_applied;

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
