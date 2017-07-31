using System;


namespace CryptoAccouting.CoreClass
{
    public class FXRate
    {
        public EnuCCY BaseCurrency { get; set; }
        public EnuCCY Currency2 { get; set; }
        public double Rate { get; set; }
        public double RateBefore24h { get; set; }
		public DateTime PriceDate { get; set; }

        public FXRate()
        {
        }
    }
}
