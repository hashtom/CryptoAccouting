using System;
namespace CryptoAccouting
{
    public class Price
    {
        public Instrument Asset { get; }
        public double LatestPrice { get; set; }
		public string BaseCurrency { get; set; }
        public int Volume_1D { get; set; }
        public DateTime PriceDate { get; set; }
        public Exchange PriceExchange { get; set; }
		public double PrevClose { get; set; }
		public DateTime UpdateTime { get; }

        public Price(Instrument asset)
        {
            Asset = asset;
            UpdateTime = DateTime.Now;
        }
    }
}
