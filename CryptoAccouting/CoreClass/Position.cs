using System;
using System.Collections.Generic;

namespace CryptoAccouting.CoreClass
{
    public class Position
    {
        public string Id { get; }
        public Instrument Coin { get; }
		public DateTime BalanceDate { get; set; }
        public double Amount { get; set; }
        public double BookPrice { get; set; }
        //public Price PriceData { get; private set; }
        public Transaction Tx { get; private set; }
        public DateTime UpdateTime { get; private set; }

		public Position(Instrument coin, string positionId)
        {
            Coin = coin;
            Id = positionId;
        }

        //public double ProfitLoss(){
        //    return MarketValuePrevClose() - BookValue();
        //}

        public double MarketPrice(){
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPriceBTC;
        }

        public double? Pct1d(){
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.Pct1d;
        }

		public double MarketDayVolume()
		{
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.DayVolume;
		}

		public double BookValue()
		{
			return Amount * BookPrice;
		}

  //      public double MarketValuePrevClose()
		//{
		//	return Amount * Amount * PriceData.PrevClose;
		//}

	}
}
