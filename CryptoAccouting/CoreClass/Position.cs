using System;
using System.Collections.Generic;

namespace CryptoAccouting.CoreClass
{
    public class Position
    {
        public int Id { get; set; }
        public Instrument Coin { get; private set; }
		public DateTime BalanceDate { get; set; }
        public double Amount { get; set; }
        public double AmountBTC { get; set; }
        public double BookPriceUSD { get; set; }
        public Exchange BookedExchange { get; set; }
        public CoinStorage CoinStorage { get; private set; }
        public double PriceUSD { get; set; }
        public double PriceBTC { get; set; }
        public double PriceBase { get; set; }

        public Position(Instrument coin)
        {
            Coin = coin;
            BalanceDate = DateTime.Now.Date;
            //this.SourceCurrency = coin.MarketPrice.SourceCurrency; //todo network issue = marketprice is null
        }

        public Position()
        {
            Coin = new Instrument("N/A","N/A","N/A");
            BalanceDate = DateTime.Now.Date;
        }

        public void AttachInstrument(Instrument coin)
        {
            this.Coin = coin;
        }

        public void AttachNewStorage(string storagecode, EnuCoinStorageType storagetype)
        {
            switch (storagetype)
            {
                case EnuCoinStorageType.Exchange:
                    CoinStorage = BookedExchange;
                    break;
                default:
                    CoinStorage = new Wallet(storagecode, storagetype);
                    break;
            }
        }

        public void AttachCoinStorage(CoinStorage storage)
        {
            CoinStorage = storage;
        }

        public double BookPriceBase(CrossRate USDCrossRate) //todo booking fx rate
        {
            return Coin.MarketPrice == null ? 0 : BookPriceUSD * USDCrossRate.Rate;
        }

        public double LatestAmountBTC()
        {
            AmountBTC = Coin.MarketPrice == null ? AmountBTC : Coin.MarketPrice.LatestPriceBTC * this.Amount;
            return AmountBTC;
        }

        public double LatestPriceUSD()
        {
            PriceUSD = Coin.MarketPrice == null ? PriceUSD : Coin.MarketPrice.LatestPriceUSD;
            return PriceUSD;
        }

        public double LatestPriceBase()
        {
            PriceBase = Coin.MarketPrice == null ? PriceBase : Coin.MarketPrice.LatestPriceBase();
            return PriceBase;
        }

        public double LatestPriceBTC()
        {
            PriceBTC = Coin.MarketPrice == null ? PriceBTC : Coin.MarketPrice.LatestPriceBTC;
            return PriceBTC;
        }

        public double LatestSourceRet1d()
        {
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.SourceRet1d();
        }

        public double Ret1dBase()
        {
            return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.Ret1dBase();
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
            return Coin.MarketPrice == null ? PriceUSD * Amount : Coin.MarketPrice.LatestPriceUSD * this.Amount;
        }

		public double LatestFiatValueBase()
		{
            return Coin.MarketPrice == null ? PriceBase * Amount : Coin.MarketPrice.LatestPriceBase() * this.Amount;
		}

		public double BookValue()
		{
			return Amount * BookPriceUSD;
		}
	}

}
