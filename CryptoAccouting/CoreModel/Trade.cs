using System;
using System.Collections.Generic;


namespace CoinBalance.CoreModel
{
    public class Trade
    {
        public int TxId { get; set; }
        public Instrument TradedCoin { get; }
        public AssetType Type { get; set; }
        public EnuCCY SettlementCCY { get; set; }
        public DateTime TradeDate { get; set; }
        public Exchange TradeExchange { get; }
        public EnuSide Side { get; set; }
        public double Quantity { get; set; }
        public double TradePriceSettle { get; set; }
        public double Fee { get; set; }
        //public double BookPrice { get; set; } = 0;
        //public string BrokerCode { get; set; }
        public int NumTransaction { get; set; }

        public string CoinId
        {
            get { return TradedCoin.Id; }
        }

        public string ColumnCoinSymbol
        {
            get { return TradedCoin.Symbol1; }
        }

        public double TradeGrossValue
        {
            get { return TradePriceSettle * Quantity; }
        }

        public double TradeNetValue
        {
            get { return Side == EnuSide.Buy ? TradePriceSettle * Quantity - Fee : TradePriceSettle * Quantity + Fee; }
        }

        public string ColumnTradePriceSettle
        {
            get { return AppCore.NumberFormat(TradePriceSettle); }
        }

        public string ColumnTradeNetValue
        {
            get { return AppCore.NumberFormat(TradeNetValue); }
        }

        //public double RealizedBookValue
        //{
        //    get { return Side == EnuSide.Sell ? BookPrice * Quantity : 0; }
        //}

        //public double RealizedPL
        //{
        //    get { return (Side == EnuSide.Sell) ? (TradePriceSettle - BookPrice) * Quantity - Fee : 0; }
        //}

        public Trade(Instrument coin, Exchange exchange)
        {
            TradedCoin = coin;
            TradeExchange = exchange;
        }

    }

    public enum EnuSide
    {
        Buy,
        Sell
    }
}
