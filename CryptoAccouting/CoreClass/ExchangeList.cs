﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CryptoAccouting.UIClass;

namespace CryptoAccouting.CoreClass
{
    public class ExchangeList : IEnumerable<Exchange>
    {
        private List<Exchange> exchanges;

        public ExchangeList()
        {
            exchanges = new List<Exchange>();
        }

        public TradeList GetTradelist(EnuExchangeType exc)
        {

            if (exchanges == null ? false : exchanges.Where(x => x.ExchangeType == exc).Any())
            {

                return exchanges.Where(x => x.ExchangeType == exc).First().TradeList;
            }
            else
            {
                return null;
            }
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

        public Exchange GetTransactionByIndex(int indexNumber)
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
