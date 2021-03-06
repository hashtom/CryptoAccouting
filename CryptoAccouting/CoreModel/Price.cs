﻿using System;

namespace CoinBalance.CoreModel
{
    public class Price
    {
        public Instrument Coin { get; }
        public string PriceSource { get; set; }
		public double LatestPriceBTC { get; set; }
        public double LatestPriceUSD { get; set; }
        public double PriceBTCBefore24h { get; set; }
        public double PriceUSDBefore24h { get; set; }
        public double DayVolume { get; set; }
        public double MarketCap { get; set; }
        public CrossRate USDCrossRate { get; set; }
		public DateTime PriceDate { get; set; }

        public Price(Instrument coin)
        {
            Coin = coin;
        }

        public double LatestPriceBase()
        {
            return (USDCrossRate is null) ? 0 : LatestPriceUSD * USDCrossRate.Rate;
        }

        public double MarketCapBase()
        {
            return (USDCrossRate is null) ? 0 : MarketCap * USDCrossRate.Rate;
        }

        public double BaseRet1d()
        {
            return (USDCrossRate is null) ? 0 : ((LatestPriceUSD * USDCrossRate.Rate) / (PriceUSDBefore24h * USDCrossRate.RateBefore24h) - 1) * 100;
        }

        public double USDRet1d(){
            return (LatestPriceUSD / PriceUSDBefore24h - 1) * 100;
        }

		public double BTCRet1d()
		{
            return (LatestPriceBTC / PriceBTCBefore24h - 1) * 100;
		}

    }
}
