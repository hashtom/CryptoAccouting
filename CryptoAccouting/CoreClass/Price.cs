using System;
namespace CryptoAccouting
{
    public class Price
    {
        public Instrument Asset { get; }
		public string BaseCurrency { get; set; }
        public Exchange PriceExchange { get; set; }

		public double LatestPrice { get; set; }
		public int DayVolume { get; set; }
		public double PrevClose { get; set; }
		public DateTime PriceDate { get; set; }

		public DateTime UpdateTime { get; }

        public Price(Instrument asset)
        {
            Asset = asset;
            UpdateTime = DateTime.Now;
        }

        public double DayReturn(){
            return LatestPrice.Equals((double)0) ? 0 : PrevClose / LatestPrice - 1;

        }
    }
}
