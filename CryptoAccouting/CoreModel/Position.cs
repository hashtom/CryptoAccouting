using System;
using System.Collections.Generic;

namespace CoinBalance.CoreModel
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
        public double USDRet1d_Previous { get; set; }
        public double BTCRet1d_Previous { get; set; }
        public double BaseRet1d_Previous { get; set; }
        public double Volume_Previous { get; set; }
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

        public string ColumnCoinSymbol
        {
            get { return Coin.Symbol1; }
        }

        public double ColumnAmountBTC
        {
            get { return LatestAmountBTC; }
        }

        public double LatestAmountBTC
        {
            get
            {
                AmountBTC_Previous = Coin.MarketPrice == null ? AmountBTC_Previous : Coin.MarketPrice.LatestPriceBTC * this.Amount;
                return AmountBTC_Previous;
            }
        }

        public double LatestPriceUSD
        {
            get
            {
                PriceUSD_Previous = Coin.MarketPrice == null ? PriceUSD_Previous : Coin.MarketPrice.LatestPriceUSD;
                return PriceUSD_Previous;
            }
        }

        public double LatestPriceBase
        {
            get
            {
                PriceBase_Previous = Coin.MarketPrice == null ? PriceBase_Previous : Coin.MarketPrice.LatestPriceBase();
                return PriceBase_Previous;
            }
        }

        public double LatestPriceBTC
        {
            get
            {
                PriceBTC_Previous = Coin.MarketPrice == null ? PriceBTC_Previous : Coin.MarketPrice.LatestPriceBTC;
                return PriceBTC_Previous;
            }
        }

		public double USDRet1d
		{
            get
            {
                USDRet1d_Previous = Coin.MarketPrice == null ? USDRet1d_Previous : Coin.MarketPrice.USDRet1d();
                return USDRet1d_Previous;
            }
		}

        public double BaseRet1d
        {
            get
            {
                BaseRet1d_Previous = Coin.MarketPrice == null ? BaseRet1d_Previous : Coin.MarketPrice.BaseRet1d();
                return BaseRet1d_Previous;
            }
        }

        public double BTCRet1d
        {
            get
            {
                BTCRet1d_Previous = Coin.MarketPrice == null ? BTCRet1d_Previous : Coin.MarketPrice.BTCRet1d();
                return BTCRet1d_Previous;
            }
        }

        public double MarketDayVolume
        {
            get
            {
                Volume_Previous = Coin.MarketPrice == null ? Volume_Previous : Coin.MarketPrice.DayVolume;
                return Volume_Previous;
            }
        }

        public double LatestFiatValueUSD()
        {
            return Coin.MarketPrice == null ? PriceUSD_Previous * Amount : Coin.MarketPrice.LatestPriceUSD * this.Amount;
        }

		public double LatestFiatValueBase()
		{
            return Coin.MarketPrice == null ? PriceBase_Previous * Amount : Coin.MarketPrice.LatestPriceBase() * this.Amount;
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

        public void ClearAttributes()
        {
            BalanceDate = DateTime.MinValue;
            BookedExchange = null;

            if (!WatchOnly)
            {
                CoinStorage.StorageType = EnuCoinStorageType.TBA;
                CoinStorage.Code = EnuCoinStorageType.TBA.ToString();
            }
            else
            {
                CoinStorage = null;
            }
        }
	}

}
