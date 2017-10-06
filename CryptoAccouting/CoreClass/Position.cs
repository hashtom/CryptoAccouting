using System;
using System.Collections.Generic;

namespace CryptoAccouting.CoreClass
{
    public class Position
    {
        //
        // You may need to update Balance.ReloadBalanceByCoin() 
        // when you change properties
        //
        public int Id { get; set; }
        public Instrument Coin { get; private set; }
		public DateTime BalanceDate { get; set; }
        public double Amount { get; set; }
        public double AmountBTC_Previous { get; set; }
        public Exchange BookedExchange { get; set; }
        public CoinStorage CoinStorage { get; private set; }
        public double PriceUSD_Previous { get; set; }
        public double PriceBTC_Previous { get; set; }
        public double PriceBase_Previous { get; set; }
        public bool WatchOnly { get; set; }

        public Position(Instrument coin)
        {
            Id = int.MaxValue;
            Coin = coin;
            BalanceDate = DateTime.Now.Date;
            WatchOnly = false;
            //this.SourceCurrency = coin.MarketPrice.SourceCurrency; //todo network issue = marketprice is null
        }

        public Position()
        {
            Id = int.MaxValue;
            Coin = new Instrument("N/A");
            BalanceDate = DateTime.Now.Date;
        }

        public bool IsIdAssigned()
        {
            return Id != int.MaxValue;
        }

        public void AttachInstrument(Instrument coin)
        {
            this.Coin = coin;
        }

        public void AttachCoinStorage(CoinStorage storage)
        {
            CoinStorage = storage;
        }

        //public double BookPriceBase(CrossRate USDCrossRate) //todo booking fx rate
        //{
        //    return Coin.MarketPrice == null ? 0 : BookPriceUSD * USDCrossRate.Rate;
        //}

        public double LatestAmountBTC()
        {
            AmountBTC_Previous = Coin.MarketPrice == null ? AmountBTC_Previous : Coin.MarketPrice.LatestPriceBTC * this.Amount;
            return AmountBTC_Previous;
        }

        public double LatestPriceUSD()
        {
            PriceUSD_Previous = Coin.MarketPrice == null ? PriceUSD_Previous : Coin.MarketPrice.LatestPriceUSD;
            return PriceUSD_Previous;
        }

        public double LatestPriceBase()
        {
            PriceBase_Previous = Coin.MarketPrice == null ? PriceBase_Previous : Coin.MarketPrice.LatestPriceBase();
            return PriceBase_Previous;
        }

        public double LatestPriceBTC()
        {
            PriceBTC_Previous = Coin.MarketPrice == null ? PriceBTC_Previous : Coin.MarketPrice.LatestPriceBTC;
            return PriceBTC_Previous;
        }

        public double LatestSourceRet1d()
        {
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.USDRet1d();
        }

		public double USDRet1d()
		{
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.USDRet1d();
		}

        public double BaseRet1d()
        {
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.BaseRet1d();
        }

        public double BTCRet1d()
        {
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.BTCRet1d();
        }

        public double MarketDayVolume()
        {
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.DayVolume;
        }

        public double LatestFiatValueUSD()
        {
            return Coin.MarketPrice == null ? PriceUSD_Previous * Amount : Coin.MarketPrice.LatestPriceUSD * this.Amount;
        }

		public double LatestFiatValueBase()
		{
            return Coin.MarketPrice == null ? PriceBase_Previous * Amount : Coin.MarketPrice.LatestPriceBase() * this.Amount;
		}

		//public double BookValueUSD()
		//{
		//	return Amount * BookPriceUSD;
		//}

        //public double PLBase()
        //{
        //    return 0;
            //return BookValueUSD() - LatestFiatValueBase();
        //}

        //public double PLUSD()
        //{
        //    return BookValueUSD() - LatestFiatValueUSD(); 
        //}
	}

}
