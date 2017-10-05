﻿using System;
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
		}

        public static CoinStorageList GetStorageListSelection()
        {
            var list = new CoinStorageList();
			foreach (EnuCoinStorageType storagetype in Enum.GetValues(typeof(EnuCoinStorageType)))
			{
                var storage = new CoinStorage(storagetype.ToString(), storagetype);
                list.Attach(storage, false);
			}
            return list;
        }

        public void RecalculateWeights()
        {
            double sum = storages.Sum(x => x.AmountBTC());
            storages.ForEach(w=>w.Weight = w.AmountBTC() / sum);
        }

        public void Attach(CoinStorage storage, bool recalculate = true)
		{
			if (storages.Any(x => x.Code == storage.Code)) Detach(storage);
			storages.Add(storage);
            if (recalculate) RecalculateWeights();
		}

        public void Detach(CoinStorage storage, bool recalculate = true)
		{
			storages.RemoveAll(x => x.Code == storage.Code);
            if (recalculate) RecalculateWeights();
		}

        public void DetachPositionByCoin(string InstrumentId)
        {
            foreach(var storage in storages)
            {
                storage.BalanceOnStorage.DetachPositionByCoin(InstrumentId);
            }
        }

        public void Clear()
        {
            storages.Clear();
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
