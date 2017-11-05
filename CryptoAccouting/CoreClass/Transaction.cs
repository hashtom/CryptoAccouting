using System;
using System.Collections.Generic;


namespace CoinBalance.CoreClass
{
    public class Transaction
    {
        public string TxId { get; set; }
        public Instrument TradedCoin { get; }
        public EnuCCY SettlementCCY { get; set; }
        public DateTime TradeDate { get; set; }
        public Exchange TradeExchange { get; }
        public EnuBuySell BuySell { get; set; }
        public double Quantity { get; set; }
        public double TradePriceSettle { get; set; }
        public double Fee { get; set; }
        public double BookPrice { get; set; }
        public int NumTransaction { get; set; }
        //public bool IsMargin { get; set; }
        //public DateTime UpdateTime { get; set; }

        public string TradecCoinSymbol
        {
            get { return TradedCoin.Symbol1; }
        }

        public double TradeGrossValue
        {
            get { return TradePriceSettle * Quantity; }
        }

        public double TradeNetValue
        {
            get { return BuySell == EnuBuySell.Buy ? TradePriceSettle * Quantity - Fee : TradePriceSettle * Quantity + Fee; }
        }

        public string ColumnTradePriceSettle
        {
            get { return AppCore.NumberFormat(TradePriceSettle); }
        }

        public string ColumnTradeNetValue
        {
            get { return AppCore.NumberFormat(TradeNetValue); }
        }

        public double RealizedBookValue
        {
            get { return BuySell == EnuBuySell.Sell ? BookPrice * Quantity : 0; }
        }

        public double RealizedPL
        {
            // calculate Realized PL when one side of trase is Base Fiat Currency
            // ignore trades both sides are Crypto for Realized PL calculation 
            get { return (BuySell == EnuBuySell.Sell) ? (TradePriceSettle - BookPrice) * Quantity - Fee : 0; }
        }

        public Transaction(Instrument coin, Exchange exchange)
        {
            TradedCoin = coin;
            TradeExchange = exchange;
            BookPrice = 0;
        }

    }

    public enum EnuBuySell
    {
        Buy,
        Sell,
        Check
    }
}
