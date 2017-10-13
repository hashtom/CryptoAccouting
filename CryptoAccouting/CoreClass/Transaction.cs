using System;
using System.Collections.Generic;


namespace CryptoAccouting.CoreClass
{
    public class Transaction
    {
        public string TxId { get; set; }
        public Instrument TradedCoin { get; }
        public EnuCCY SettlementCCY { get; set; } //Fiat or BTC(or, ETH, USDT,etc)
        public DateTime TradeDate { get; set; }
        public Exchange TradeExchange { get; }
        public EnuBuySell BuySell { get; set; }
        public double Quantity { get; set; }
        public double TradePrice { get; set; }
        public double Fee { get; set; }
        public double BookPrice { get; set; }
        public int NumOfTransaction { get; set; }
        //public bool IsMargin { get; set; }
        //public DateTime UpdateTime { get; set; }

        public string TradecCoinSymbol {
            get { return TradedCoin.Symbol1; }
        }

        public string SettlementCCY_Str
        {
            get { return SettlementCCY.ToString(); }
        }

        public string Side
        {
            get { return BuySell == EnuBuySell.Buy ? "Buy" : "Sell"; }
        }

        //public string Quantity_Str
        //{ 
        //    get { return ApplicationCore.NumberFormat(Quantity); }
        //}

  //      public string TradePrice_Str
  //      {
  //          get { return ApplicationCore.NumberFormat(TradePrice); }
  //      }

  //      public string Fee_Str
  //      {
  //          get { return ApplicationCore.NumberFormat(Fee); }
  //      }

  //      public string TradeGrossValue_Str
  //      {
  //          get { return ApplicationCore.NumberFormat(TradePrice * Quantity); }
  //      }

  //      public string TradeNetValue_Str
		//{
  //          get { return BuySell == EnuBuySell.Buy ? ApplicationCore.NumberFormat(TradePrice * Quantity - Fee) : ApplicationCore.NumberFormat(TradePrice * Quantity + Fee); }
		//}

        //public string RealizedBookValue_Str
        //{
        //    get { return BuySell == EnuBuySell.Sell ? ApplicationCore.NumberFormat(BookPrice * Quantity) : "0"; }
        //}

        public double TradeGrossValue
        {
            get { return TradePrice * Quantity; }
        }

        public double TradeNetValue
        {
            get { return BuySell == EnuBuySell.Buy ? TradePrice * Quantity - Fee : TradePrice * Quantity + Fee; }
        }

        public double RealizedBookValue
        {
            get { return BuySell == EnuBuySell.Sell ? BookPrice * Quantity : 0; }
        }

        //public double TradeValueConvert(EnuCCY baseCCY)
        //{
        //    // JPY<->MONA
        //    // BTC<->REP : ignore unless baseccy is BTC
        //    return 0;
        //}

        public double RealizedPL
        {
            // calculate Realized PL when one side of trase is Base Fiat Currency
            // ignore trades both sides are Crypto for Realized PL calculation 
            get { return (BuySell == EnuBuySell.Sell) ? (TradePrice - BookPrice) * Quantity - Fee : 0; }
        }

        //public double RelizedPLConvert(EnuCCY baseCCY)
        //{
        //    return 0;
        //}

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
