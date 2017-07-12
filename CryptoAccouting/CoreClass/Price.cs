﻿using System;
namespace CryptoAccouting.CoreClass
{
    public class Price
    {
        public Instrument Coin { get; }
        public EnuBaseCCY BaseCurrency { get; set; }
        public string PriceSource { get; set; }
		public double LatestPrice { get; set; }
        public double DayVolume { get; set; }
		public double PrevClose { get; set; }
		public DateTime PriceDate { get; set; }
		//public DateTime UpdateTime { get; set; }

        public Price(Instrument coin)
        {
            Coin = coin;
            //UpdateTime = DateTime.Now;
        }

        public double DayReturn(){
            return LatestPrice.Equals((double)0) ? 0 : PrevClose / LatestPrice - 1;

        }
    }
}
