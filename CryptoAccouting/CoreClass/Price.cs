using System;
namespace CryptoAccouting
{
    public class Price
    {
		public double ClosePrice { get; set; }
		public string BaseCurrency { get; set; }
        public int Volume_1D { get; set; }
        public DateTime PriceDate { get; set; }
        public Exchange PriceExchange { get; set; }
        public DateTime UpdateTime { get; private set; }
		
        public Price()
        {
        }
    }
}
