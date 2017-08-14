using System;


namespace CryptoAccouting.CoreClass
{
    public class CrossRate
    {
        public EnuCCY Currency { get; set; }
        public double Rate { get; set; }
        public double RateBefore24h { get; set; }
		public DateTime PriceDate { get; set; }

        public CrossRate(EnuCCY currency, double rate, DateTime pricedate)
        {
            this.Currency = currency;
            this.Rate = rate;
            this.PriceDate = pricedate;
        }
    }


	public enum EnuCCY
	{
		JPY,
		USD,
		EUR,
		BTC
	}
}
