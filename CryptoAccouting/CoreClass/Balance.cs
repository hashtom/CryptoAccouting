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

		public Balance()
        {
            positions = new List<Position>();
        }

        public void ReCalculate()
        {
            ReloadBalanceByCoin();
        }

        public void AttachInstruments(InstrumentList instrumentist)
        {
            positions.ForEach(x => x.AttachInstrument(instrumentist.First(y => y.Symbol1 == x.Coin.Symbol1)));
        }

        public double LatestFiatValueUSD(){
            return  positions.Sum(x => x.LatestFiatValueUSD());
        }

		public double LatestFiatValueBase()
		{
            return positions.Sum(x => x.LatestFiatValueBase());
		}

		public double AmountBTC()
		{
            return positions.Sum(x => x.LatestAmountBTC());
		}

        public void SortPositionByHolding(){
            positions = positions.OrderByDescending(x => x.LatestAmountBTC()).ToList();
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

            foreach (var symbol in positions.Select(x => x.Coin.Symbol1).Distinct())
            {
                var position = new Position(positions.Select(x => x.Coin).First(x => x.Symbol1 == symbol))
                {
                    Amount = positions.Where(x => x.Coin.Symbol1 == symbol).Sum(x => x.Amount),
                    AmountBTC_Previous = positions.Where(x => x.Coin.Symbol1 == symbol).Sum(x => x.LatestAmountBTC()),
                    PriceBTC_Previous = positions.First(x => x.Coin.Symbol1 == symbol).LatestPriceBTC(),
                    PriceUSD_Previous = positions.First(x => x.Coin.Symbol1 == symbol).LatestPriceUSD(),
                    PriceBase_Previous = positions.First(x => x.Coin.Symbol1 == symbol).LatestPriceBase(),
                    WatchOnly = positions.First(x => x.Coin.Symbol1 == symbol).WatchOnly
                };
                BalanceByCoin.Attach(position);
            }

        }

        public double USDRet1d()
        {
            double ret1d = 0;
            var total = positions.Select(x => x.LatestFiatValueUSD()).Sum();

            foreach (var p in positions)
            {
                ret1d += p.USDRet1d() * p.LatestFiatValueUSD() / total;
            }

            return ret1d;
        }

		public double BaseRet1d()
		{
			double ret1d = 0;
            var total = positions.Select(x => x.LatestFiatValueBase()).Sum();

			foreach (var p in positions)
			{
                ret1d += p.BaseRet1d() * p.LatestFiatValueBase() / total;
			}

			return ret1d;
		}

        public void Attach(Position position) //, bool CalcSummary = true)
		{
            
            if (position.IsIdAssigned() && positions.Any(x => x.Id == position.Id))
            {
                Detach(position);
            }
            else
            {
                position.Id = positions.Count == 0 ? 1 : positions.Max(x => x.Id) + 1;
            }

			positions.Add(position);

		}

        public bool HasBalance(Instrument coin)
        {
            if(positions.Select(x => x.Coin).Any(x => x.Id == coin.Id))
            {
                return positions.Where(x => x.Coin.Id == coin.Id).Sum(x => x.Amount) > 0 ? true : false;

            }else
            {
                return false;
            }
        }

        public void Clear()
        {
            positions.Clear();
        }

		public void Detach(Position position)  //, bool CalcSummary = true)
		{
            positions.Remove(position);
			//positions.RemoveAll(x => x.Id == position.Id);
		}

        public void DetachPositionByCoin(string InstrumentId)
        {
            positions.RemoveAll(x => x.Coin.Id == InstrumentId);
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
