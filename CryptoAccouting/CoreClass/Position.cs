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
        public double BookPriceUSD { get; set; }
        //public EnuCCY SourceCurrency { get; set; }
        public Exchange BookedExchange { get; set; }
        public CoinStorage CoinStorage { get; private set; }

        public Position(Instrument coin)
        {
            Coin = coin;
            BalanceDate = DateTime.Now.Date;
            Amount = 0;
            BookPriceUSD = 0;
            //PositionType = positoinType;
            //this.SourceCurrency = coin.MarketPrice.SourceCurrency; //todo network issue = marketprice is null
        }
        public Position()
        {
            Coin = new Instrument("N/A","N/A","N/A");
            BalanceDate = DateTime.Now.Date;
            Amount = 0;
            BookPriceUSD = 0;
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

        public double BookPriceBase(CrossRate USDCrossRate)
        {
            return Coin.MarketPrice == null ? 0 : BookPriceUSD * USDCrossRate.Rate;
        }

        public double AmountBTC()
        {
            //if (PositionType is EnuPositionType.ExchangeLevel)
            //{
            //    return this.Amount;
            //}
            //else
            //{
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPriceBTC * this.Amount;
            //}
        }

		public double LatestPriceUSD()
		{
            //if (PositionType is EnuPositionType.ExchangeLevel)
            //{
            //    return 0;
            //}
            //else
            //{
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPriceUSD;
            //}
		}

		public double LatestPriceBase(CrossRate USDCrossRate)
		{
			//if (PositionType is EnuPositionType.ExchangeLevel)
			//{
			//	return 0;
			//}
			//else
			//{
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPriceBase(USDCrossRate);
			//}
		}

        public double LatestPriceBTC()
        {
            //if (PositionType is EnuPositionType.ExchangeLevel)
            //{
            //    return 0;
            //}
            //else
            //{
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPriceBTC;
            //}
        }

        public double SourceRet1d()
        {
            //if (PositionType is EnuPositionType.ExchangeLevel)
            //{
            //    return 0;
            //}
            //else
            //{
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.SourceRet1d();
            //}
        }

		public double Ret1dBase(CrossRate USDCrossRate)
		{
			//if (PositionType is EnuPositionType.ExchangeLevel)
			//{
			//	return 0;
			//}
			//else
			//{
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.Ret1dBase(USDCrossRate);
			//}
		}

        public double BTCRet1d()
        {
            //if (PositionType is EnuPositionType.ExchangeLevel)
            //{
            //    return 0;
            //}
            //else
            //{
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.BTCRet1d();
            //}
        }

        public double MarketDayVolume()
        {
            //if (PositionType is EnuPositionType.ExchangeLevel)
            //{
            //    return 0;
            //}
            //else
            //{
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.DayVolume;
            //}
        }

        public double LatestFiatValueUSD()
        {
            //if (PositionType is EnuPositionType.ExchangeLevel)
            //{
            //    return FiatValueTotalExchange;
            //}
            //else
            //{
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPriceUSD * this.Amount;
            //}
        }

		public double LatestFiatValueBase(CrossRate USDCrossRate)
		{
			//if (PositionType is EnuPositionType.ExchangeLevel)
			//{
			//	return FiatValueTotalExchange;
			//}
			//else
			//{
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPriceBase(USDCrossRate) * this.Amount;
			//}
		}

		public double BookValue()
		{
			return Amount * BookPriceUSD;
		}
	}

    //public enum EnuPositionType
    //{
    //    Detail,
    //    CoinLevel,
    //    ExchangeLevel
    //}
}
