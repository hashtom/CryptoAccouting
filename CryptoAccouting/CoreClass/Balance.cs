﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class Balance : IEnumerable<Position>
    {
        public string BalanceName { get; set; }
        private List<Position> positions; //{ get; private set; }
        public Balance BalanceByCoin { get; private set; }
        public CoinStorageList CoinStorageList { get; private set; }

		public Balance()
        {
            positions = new List<Position>();
        }

        public void ReCalculate()
        {
            ReloadBalanceByCoin();
            ReloadStorageList();
        }

        public void AttachInstruments(InstrumentList instrumentist)
        {
            positions.ForEach(x => x.AttachInstrument(instrumentist.First(y => y.Symbol == x.Coin.Symbol)));
        }

        public double LatestFiatValueUSD(){
            return positions.Sum(x => x.LatestFiatValueUSD());
        }

		public double LatestFiatValueBase(CrossRate CrossRate)
		{
            return positions.Sum(x => x.LatestFiatValueBase(CrossRate));
		}

		public double AmountBTC()
		{
            return positions.Sum(x => x.AmountBTC());
		}

        public void SortPositionByHolding(){
            positions = positions.OrderByDescending(x => x.AmountBTC()).ToList();
        }

        private void ReloadBalanceByCoin()
        {
            if (BalanceByCoin != null)
            {
                BalanceByCoin.Clear();
            }
            else
            {
                BalanceByCoin = new Balance();
            }

            foreach (var symbol in positions.Select(x => x.Coin.Symbol).Distinct())
            {
                var amount = positions.Where(x => x.Coin.Symbol == symbol).Sum(x => x.Amount);
                var bookprice = positions.Where(x => x.Coin.Symbol == symbol).Sum(x => x.BookPriceUSD);
                var position = new Position(positions.Select(x => x.Coin).First(x => x.Symbol == symbol)) { Amount = amount, BookPriceUSD = bookprice };
                BalanceByCoin.Attach(position);
            }

        }

		private void ReloadStorageList()
		{
			if (CoinStorageList != null)
			{
				CoinStorageList.Clear();
			}
			else
			{
                CoinStorageList = new CoinStorageList();
			}

			foreach (var storage in positions.Where(x => x.CoinStorage != null).Select(x => x.CoinStorage).Distinct())
			{
				if (storage != null)
				{
					storage.ClearBalanceOnStorage();
					foreach (var pos in positions.Where(x => x.CoinStorage != null).Where(x => x.CoinStorage.Code == storage.Code))
					{
						storage.AttachPosition(pos);
					}
					CoinStorageList.Attach(storage);
				}
			}

		}

   //     private Balance GetBalanceByExchangeCode(string ExchangeCode)
   //     {
   //         Balance Bal = new Balance();
   //         foreach (var pos in positions.Where(x => x.BookedExchange.Code == ExchangeCode))
   //         {
   //             Bal.Attach(pos);
   //         }
			//if (Bal.Count() > 0)
			//{
			//	return Bal;
			//}
			//else
			//{
			//	return null;
			//}
        //}

   //     private Balance GetBalanceByStorage(string storagecode)
   //     {
   //         var Bal = new Balance();
   //         foreach (var pos in positions.Where(x => x.CoinStorage.Code == storagecode))
			//{
			//	Bal.Attach(pos);
			//}

        //    if (Bal.Count() > 0)
        //    {
        //        return Bal;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public CoinStorage GetCoinStorage(string storagecode, EnuCoinStorageType storagetype)
		{
            if (positions.Where(x => x.CoinStorage != null).Select(x=>x.CoinStorage).Any(x => x.Code == storagecode && x.StorageType == storagetype))
            {
                return positions.Where(x => x.CoinStorage != null).Select(x => x.CoinStorage).Where(x => x.Code == storagecode && x.StorageType == storagetype).First();
            }
            else
            {
                return null;
            }
		}

        public void Attach(Position position) //, bool CalcSummary = true)
		{
            if (positions.Any(x => x.Id == position.Id))
            {
                Detach(position);
            }
            else
            {
                position.Id = positions.Count == 0 ? 1 : positions.Max(x => x.Id) + 1;
            }

			positions.Add(position);

		}

        public void Clear()
        {
            positions.Clear();
        }

		public void Detach(Position position)  //, bool CalcSummary = true)
		{
			positions.RemoveAll(x => x.Id == position.Id);
		}

        public void DetachPositionByCoin(string symbol)
        {
            positions.RemoveAll(x => x.Coin.Symbol == symbol);
        }

        public Position GetByIndex(int indexNumber){
            return positions[indexNumber];
        }

        public int Count()
        {
            return positions.Count();
        }

        public IEnumerator<Position> GetEnumerator()
		{
            for (int i = 0; i <= positions.Count - 1; i++) yield return positions[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
    }
}
