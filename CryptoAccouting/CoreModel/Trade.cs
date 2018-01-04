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
        public decimal Quantity { get; set; }
        public decimal TradePriceSettle { get; set; }
        public decimal Fee { get; set; }
        public int NumTransaction { get; set; }

        public string CoinId
        {
            get { return TradedCoin.Id; }
        }

        public string ColumnCoinSymbol
        {
            get { return TradedCoin.Symbol1; }
        }

        public decimal TradeGrossValue
        {
            get { return TradePriceSettle * Quantity; }
        }

        public decimal TradeNetValue
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
