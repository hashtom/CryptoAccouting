using System;

namespace CryptoAccouting.CoreClass
{
    public class Price
    {
        public Instrument Coin { get; }
        public string PriceSource { get; set; }
		public double LatestPriceBTC { get; set; }
        public double LatestPrice { get; set; }
        public double PriceBefore24h { get; set; }
		public EnuCCY SourceCurrency { get; set; }
        public double DayVolume { get; set; }
        public double MarketCap { get; set; }
        public double? FiatPct1h { get; set; }
        public double? FiatPct1d { get; set; }
        public double? FiatPct7d { get; set; }
        //public double PrevClose { get; set; }
		public DateTime PriceDate { get; set; }
        public FXRate SourceToBaseCurrency { get; set; }
        //public DateTime UpdateTime { get; set; }

        public Price(Instrument coin)
        {
            Coin = coin;
            //LatestPriceBTC = 0;
            LatestPrice = 0;
        }


        //public double DayReturn(){
        //    return LatestPrice.Equals((double)0) ? 0 : PrevClose / LatestPrice - 1;
        //}
    }
}
