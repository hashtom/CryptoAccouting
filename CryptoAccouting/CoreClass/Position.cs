using System;
using System.Collections.Generic;

namespace CryptoAccouting.CoreClass
{
    public class Position
    {
        public int Id { get; }
        public Instrument Coin { get; }
		public DateTime BalanceDate { get; set; }
        public double Amount { get; set; }
        public double BookPrice { get; set; }
       //public DateTime UpdateTime { get; private set; }

		public Position(Instrument coin, int positionId)
        {
            Coin = coin;
            Id = positionId;
        }
        public Position() { }

		public double AmountBTC()
		{
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPriceBTC * this.Amount;
		}

        public double MarketPriceBTC(){
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPriceBTC;
        }

		public double MarketPrice()
		{
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPrice;
		}

        public double? Pct1d(){
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.Pct1d;
        }

		public double MarketDayVolume()
		{
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.DayVolume;
		}

        public double LatestFiatValue()
        {
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPrice * this.Amount;
        }

		public double BookValue()
		{
			return Amount * BookPrice;
		}
	}
}
