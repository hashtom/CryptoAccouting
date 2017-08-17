﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class Balance : IEnumerable<Position>
    {
        public string BalanceName { get; set; }
        public List<Position> positions { get; private set; }

		public Balance()
        {
            positions = new List<Position>();
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

            int i = 1;
            positions = positions.OrderByDescending(x => x.AmountBTC()).ToList();

            foreach (var p in positions){
                p.Id = i;
                i++;
            }
        }

        public Balance GetBalanceByExchangeCode(string ExchangeCode)
        {
            Balance Bal = new Balance();
            foreach (var pos in positions.Where(x => x.BookedExchange.Code == ExchangeCode))
            {
                Bal.AttachPosition(pos);
            }
			if (Bal.Count() > 0)
			{
				return Bal;
			}
			else
			{
				return null;
			}
        }

        public Balance GetBalanceByStorage(string storagecode)
        {
			Balance Bal = new Balance();
            foreach (var pos in positions.Where(x => x.Storage.Code == storagecode))
			{
				Bal.AttachPosition(pos);
			}

            if (Bal.Count() > 0)
            {
                return Bal;
            }
            else
            {
                return null;
            }
        }

        public void AttachPosition(Position position) //, bool CalcSummary = true)
		{
            if (positions.Any(x => x.Id == position.Id))
            {
                DetachPosition(position);
            }
            else
            {
                position.Id = positions.Count == 0 ? 1 : positions.Max(x => x.Id) + 1;
            }

			positions.Add(position);

		}

		public void DetachPosition(Position position)  //, bool CalcSummary = true)
		{
			positions.RemoveAll(x => x.Id == position.Id);
		}

        public void DetachPositionByCoin(string symbol)
        {
            positions.RemoveAll(x => x.Coin.Symbol == symbol);
        }

        public Position GetPositionByIndex(int indexNumber){
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
