using System;


namespace CryptoAccouting.CoreClass
{
    public class USDCrossRate
    {
        public EnuCCY Currency { get; set; }
        public string Code { get; set; }
        public double Rate { get; set; }
        public double RateBefore24h { get; set; }
		public DateTime PriceDate { get; set; }

        public USDCrossRate(string code, double rate, DateTime pricedate)
        {
            this.Code = code;
            this.Rate = rate;
            this.PriceDate = pricedate;
        }
    }
}
