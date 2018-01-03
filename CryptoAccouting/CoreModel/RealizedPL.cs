﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CoinBalance.CoreModel
{

    public class RealizedPL
    {
        public Instrument TradedCoin { get; private set; }
        public EnuPLType PLType { get; set; }
        public DateTime TradeDate { get; set; }
        public EnuSide Side { get; set; }
        public EnuBaseFiatCCY SettlementCCY { get; set; }
        public decimal Quantity { get; set; }
        public decimal AvgBookPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal TradeFee { get; set; }
        public decimal MarginFee { get; set; }
        public decimal Swap { get; set; }
        public decimal DepWithFee { get; set; }
        public Exchange exchange { get; private set; }

        public string ColumnCoinSymbol
        {
            get { return TradedCoin.Symbol1; }
        }

        public RealizedPL(Exchange exchange)
        {
            this.exchange = exchange;
        }

        public decimal GrossPL
        {
            get
            {
                var priceDiff = Side == EnuSide.Buy ? (ClosePrice - AvgBookPrice) : -(ClosePrice - AvgBookPrice);
                return TradeQty * priceDiff;
            }
        }

        public decimal NetPL
        {
            get
            {
                var priceDiff = Side == EnuSide.Buy ? (ClosePrice - AvgBookPrice) : -(ClosePrice - AvgBookPrice);
                return TradeQty * priceDiff - TradeFee - MarginFee;
            }
        }
    }

    public enum EnuPLType
    {
        CashTrade,
        MarginTrade,
        FuturesTrade,
        CashDeposit,
        CashWithdrawal,
        CryptoDeposit,
        CryptoWithdrawal
    }
}