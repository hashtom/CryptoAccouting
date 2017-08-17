using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class CoinStorageList : IEnumerable<CoinStorage>
    {
        private List<CoinStorage> storages;

        public CoinStorageList()
        {
            storages = new List<CoinStorage>();
            RefreshCoinStorage();
		}

        public void RefreshCoinStorage()
        {
            

        }

		public int Count()
		{
			return storages.Count();
		}

        public IEnumerator<CoinStorage> GetEnumerator()
		{
			for (int i = 0; i <= storages.Count - 1; i++) yield return storages[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
    }
}
