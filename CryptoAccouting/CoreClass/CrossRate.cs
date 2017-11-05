using System;


namespace CoinBalance.CoreClass
{
    public class CrossRate
    {
        public EnuBaseFiatCCY Currency { get; set; }
        public double Rate { get; set; }
        public double RateBefore24h { get; set; }
		public DateTime PriceDate { get; set; }

        public CrossRate(EnuBaseFiatCCY currency, double rate, DateTime pricedate)
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
        GBP,
        AUD,
        CNY,
		BTC,
        ETH,
        USDT
	}

    public enum EnuBaseFiatCCY
    {
        JPY,
        USD,
        EUR,
        GBP,
        AUD,
        CNY
    }
}
