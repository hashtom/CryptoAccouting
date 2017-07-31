using System;

namespace CryptoAccouting.CoreClass
{
    public class Price
    {
        public Instrument Coin { get; }
        public string PriceSource { get; set; }
		public double LatestPriceBTC { get; set; }
        public double LatestPrice { get; set; }
        public double PriceBTCBefore24h { get; set; }
        public double PriceBefore24h { get; set; }
		public EnuCCY SourceCurrency { get; set; }
        public double DayVolume { get; set; }
        public double MarketCap { get; set; }
        //public double SourceRet1h { get; set; }
        //public double SourceRet1d { get; set; }
        //public double SourceRet7d { get; set; }
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

        public double BaseCurrencyRet1d()
        {
            return SourceToBaseCurrency is null ? 0 : ((LatestPrice * SourceToBaseCurrency.Rate) / (PriceBefore24h * SourceToBaseCurrency.RateBefore24h) - 1) * 100;
        }

        public double SourceRet1d(){
            return (LatestPrice / PriceBefore24h - 1) * 100;
        }

		public double BTCRet1d()
		{
            return (LatestPriceBTC / PriceBTCBefore24h - 1) * 100;
		}

        //public double DayReturn(){
        //    return LatestPrice.Equals((double)0) ? 0 : PrevClose / LatestPrice - 1;
        //}
    }
}
