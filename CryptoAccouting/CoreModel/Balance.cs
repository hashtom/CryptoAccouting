﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CoinBalance.CoreModel
{
    public class Balance : IEnumerable<Position>
    {
        public string BalanceName { get; set; }
        private List<Position> positions; //{ get; private set; }
        public List<Position> BalanceByCoin { get; private set; }
        public DateTime PriceDateTime { get; set; }

        public Balance()
        {
            positions = new List<Position>();
            BalanceByCoin = new List<Position>();
        }

        public List<Position> BalanceCollection
        {
            get { return BalanceByCoin.Where(x => x.WatchOnly == false).Where(x => x.LatestAmountBTC > 0.0001).ToList(); }
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

        public double Amount()
        {
            return positions.Sum(x => x.Amount);
        }
		public double AmountBTC()
		{
            return positions.Sum(x => x.LatestAmountBTC);
		}

        public void RefreshBalanceData()
        {
            if (BalanceByCoin != null)
            {
                BalanceByCoin.Clear();
            }
            else
            {
                BalanceByCoin = new List<Position>();
            }

            int newid = 0;
            foreach (var instrumentid in positions.OrderByDescending(x => x.LatestAmountBTC).ThenBy(x => x.Coin.rank).Select(x => x.Coin.Id).Distinct())
            {
                var position = new Position(positions.Select(x => x.Coin).First(x => x.Id == instrumentid))
                {
                    Id = newid,
                    Amount = positions.Where(x => x.Coin.Id == instrumentid).Sum(x => x.Amount),
                    AmountBTC_Previous = positions.Where(x => x.Coin.Id == instrumentid).Sum(x => x.LatestAmountBTC),
                    PriceBTC_Previous = positions.First(x => x.Coin.Id == instrumentid).LatestPriceBTC,
                    PriceUSD_Previous = positions.First(x => x.Coin.Id == instrumentid).LatestPriceUSD,
                    PriceBase_Previous = positions.First(x => x.Coin.Id == instrumentid).LatestPriceBase,
                    BTCRet1d_Previous = positions.First(x => x.Coin.Id == instrumentid).BTCRet1d,
                    USDRet1d_Previous = positions.First(x => x.Coin.Id == instrumentid).USDRet1d,
                    BaseRet1d_Previous = positions.First(x => x.Coin.Id == instrumentid).BaseRet1d,
                    Volume_Previous = positions.First(x => x.Coin.Id == instrumentid).MarketDayVolume,
                    WatchOnly = positions.First(x => x.Coin.Id == instrumentid).WatchOnly
                };
                newid++;
                BalanceByCoin.Add(position);
            }
        }

        public double USDRet1d()
        {
            double ret1d = 0;
            var total = positions.Select(x => x.LatestFiatValueUSD()).Sum();

            foreach (var p in positions)
            {
                ret1d += p.USDRet1d * p.LatestFiatValueUSD() / total;
            }

            return ret1d;
        }

		public double BaseRet1d()
		{
			double ret1d = 0;
            var total = positions.Select(x => x.LatestFiatValueBase()).Sum();

            if (Math.Abs(total) < double.Epsilon)
            {
                ret1d = 0;
            }
            else
            {
                foreach (var p in positions)
                {
                    ret1d += p.BaseRet1d * p.LatestFiatValueBase() / total;
                }
            }

            return ret1d;
		}

        public void Attach(Position position)
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
            return positions.Select(x => x.Coin).Any(x => x.Id == coin.Id);
            //if(positions.Select(x => x.Coin).Any(x => x.Id == coin.Id))
            //{
            //    return positions.Where(x => x.Coin.Id == coin.Id).Sum(x => x.Amount) > 0 ? true : false;

            //}else
            //{
            //    return false;
            //}
        }

        public void ClearBalance()
        {
            positions.Clear();
            BalanceByCoin.Clear();
        }

		public void Detach(Position position)
		{
            positions.Remove(position);
		}

        public void DetachPositionByCoin(string InstrumentId)
        {
            positions.RemoveAll(x => x.Coin.Id == InstrumentId);
        }

        public void DetachPositionByStorage(CoinStorage storage)
        {
            foreach (var pos in positions
                     .Where(x => x.CoinStorage != null)
                     .Where(x => (x.CoinStorage.Code == storage.Code && x.CoinStorage.StorageType == storage.StorageType)).ToList())
            {
                
                positions.Remove(pos);
            }
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
