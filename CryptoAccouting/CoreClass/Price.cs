using System;

namespace CryptoAccouting.CoreClass
{
    public class Price
    {
        public Instrument Coin { get; }
        public string PriceSource { get; set; }
		public double LatestPriceBTC { get; set; }
        public double LatestPriceUSD { get; set; }
        public double PriceBTCBefore24h { get; set; }
        public double PriceUSDBefore24h { get; set; }
		//public EnuCCY SourceCurrency { get; set; } //USD or BTC
        public double DayVolume { get; set; }
        public double MarketCap { get; set; }
        //public double SourceRet1h { get; set; }
        //public double SourceRet1d { get; set; }
        //public double SourceRet7d { get; set; }
        //public double PrevClose { get; set; }
		public DateTime PriceDate { get; set; }
        //public CrossRate CrossRate { get; set; }
        //public DateTime UpdateTime { get; set; }

        public Price(Instrument coin)
        {
            Coin = coin;
            //LatestPriceBTC = 0;
            LatestPriceUSD = 0;
        }

        public double LatestPriceBase(CrossRate CrossRate)
        {
            return (CrossRate is null) ? 0 : LatestPriceUSD * CrossRate.Rate;
        }

        public double MarketCapBase(CrossRate CrossRate)
        {
            return (CrossRate is null) ? 0 : MarketCap * CrossRate.Rate;
        }

        public double Ret1dBase(CrossRate CrossRate)
        {
            return (CrossRate is null) ? 0 : ((LatestPriceUSD * CrossRate.Rate) / (PriceUSDBefore24h * CrossRate.RateBefore24h) - 1) * 100;
        }

        public double SourceRet1d(){
            return (LatestPriceUSD / PriceUSDBefore24h - 1) * 100;
        }

		public double BTCRet1d()
		{
            return (LatestPriceBTC / PriceBTCBefore24h - 1) * 100;
		}

    }
}
