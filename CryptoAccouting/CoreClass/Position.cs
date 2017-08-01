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
        public EnuPositionType PositionType { get; }
        public double FiatValueTotalExchange { get; set; }
        public EnuCCY SourceCurrency { get; set; }
       //public DateTime UpdateTime { get; private set; }

        public Position(Instrument coin, EnuPositionType positoinType)
        {
            Coin = coin;
            BalanceDate = DateTime.Now.Date;
            Amount = 0;
            BookPrice = 0;
            PositionType = positoinType;
            this.SourceCurrency = coin.MarketPrice.SourceCurrency;
        }
        public Position(EnuPositionType positoinType)
        {
            Coin = new Instrument("N/A","N/A","N/A");
            BalanceDate = DateTime.Now.Date;
            Amount = 0;
            BookPrice = 0;
            PositionType = positoinType;
            //this.SourceCurrency = sourceCurrencyrency; //todo must be one currency 
        }

        public double AmountBTC()
        {
            if (PositionType is EnuPositionType.ExchangeLevel)
            {
                return this.Amount;
            }
            else
            {
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPriceBTC * this.Amount;
            }
        }

		public double LatestPrice()
		{
            if (PositionType is EnuPositionType.ExchangeLevel)
            {
                return 0;
            }
            else
            {
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPrice;
            }
		}

        public double LatestPriceBTC()
        {
            if (PositionType is EnuPositionType.ExchangeLevel)
            {
                return 0;
            }
            else
            {
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPriceBTC;
            }
        }

        public double SourceRet1d()
        {
            if (PositionType is EnuPositionType.ExchangeLevel)
            {
                return 0;
            }
            else
            {
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.SourceRet1d();
            }
        }

        public double BTCRet1d()
        {
            if (PositionType is EnuPositionType.ExchangeLevel)
            {
                return 0;
            }
            else
            {
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.BTCRet1d();
            }
        }

        public double MarketDayVolume()
        {
            if (PositionType is EnuPositionType.ExchangeLevel)
            {
                return 0;
            }
            else
            {
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.DayVolume;
            }
        }

        public double LatestFiatValue()
        {
            if (PositionType is EnuPositionType.ExchangeLevel)
            {
                return FiatValueTotalExchange;
            }
            else
            {
                return Coin.MarketPrice == null ? 0 : Coin.MarketPrice.LatestPrice * this.Amount;
            }
        }

		public double BookValue()
		{
			return Amount * BookPrice;
		}
	}

    public enum EnuPositionType
    {
        Detail,
        CoinLevel,
        ExchangeLevel
    }
}
