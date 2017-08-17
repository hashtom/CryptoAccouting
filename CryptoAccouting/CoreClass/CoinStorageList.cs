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

        public CoinStorage GetCoinStorage(string Code, EnuCoinStorageType storagetype)
		{
			if (!storages.Any(x => x.Code == Code))
			{
                if (storagetype == EnuCoinStorageType.Exchange)
                {
                    this.Attach(ApplicationCore.GetExchange(Code));
                }
                else
                {
                    this.Attach(new Wallet(Code, storagetype));
                }
			}

			return storages.First(x => x.Code == Code);
		}

        public void Attach(CoinStorage storage)
		{
			if (storages.Any(x => x.Code == storage.Code)) Detach(storage);
			storages.Add(storage);
		}

		public void Detach(CoinStorage storage)
		{
			storages.RemoveAll(x => x.Code == storage.Code);
		}

        public CoinStorage GetByIndex(int indexNumber)
		{
			return storages[indexNumber];
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
