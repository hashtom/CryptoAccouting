using System;
using System.Collections.Generic;

namespace CryptoAccouting.CoreClass
{
    public class Position
    {
        public int Id { get; set; }
        public Instrument Coin { get; }
		public DateTime BalanceDate { get; set; }
        public double Amount { get; set; }
        public double BookPrice { get; set; }
        public EnuExchangeType TradedExchange { get; set; }
       //public DateTime UpdateTime { get; private set; }

		public Position(Instrument coin)
        {
            Coin = coin;
            BalanceDate = DateTime.Now.Date;
            Amount = 0;
            BookPrice = -99999999;
        }
        public Position()
        {
            Coin = new Instrument("N/A","N/A","N/A");
            BalanceDate = DateTime.Now.Date;
            Amount = 0;
            BookPrice = -99999999;
        }

		public double AmountBTC()
		{
            return Coin.MarketPrice == null ? 0 :  Coin.MarketPrice.LatestPriceBTC * this.Amount;
		}

        //public double MarketPriceBTC(){
        //    return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPriceBTC;
        //}

		public double LatestPrice()
		{
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPrice;
		}

		public double LatestPriceBTC()
		{
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPriceBTC;
		}

        public double SourceRet1d(){
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.SourceRet1d();
        }

		public double BTCRet1d()
		{
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.BTCRet1d();
		}

		public double MarketDayVolume()
		{
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.DayVolume;
		}

        public double LatestFiatValue()
        {
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPrice * this.Amount;
        }

		public double LatestBTCValue()
		{
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPriceBTC * this.Amount;
		}

		public double BookValue()
		{
			return Amount * BookPrice;
		}
	}
}
