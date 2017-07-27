﻿using System;
namespace CryptoAccouting.CoreClass
{
    public class Price
    {
        public Instrument Coin { get; }
        public string PriceSource { get; set; }
		public double LatestPriceBTC { get; set; }
        public double LatestPrice { get; set; }
		public EnuCCY BaseCurrency { get; set; }
        public double DayVolume { get; set; }
        public double? Pct1h { get; set; }
        public double? Pct1d { get; set; }
        public double? Pct7d { get; set; }
        //public double PrevClose { get; set; }
		public DateTime PriceDate { get; set; }
        //public DateTime UpdateTime { get; set; }

        public Price(Instrument coin)
        {
            Coin = coin;
            LatestPriceBTC = 0;
            LatestPrice = 0;
        }

        public double LatestMainPrice(){
            return Coin.Symbol is "BTC" ? LatestPrice : LatestPriceBTC;
        }


        //public double DayReturn(){
        //    return LatestPrice.Equals((double)0) ? 0 : PrevClose / LatestPrice - 1;
        //}
    }
}
