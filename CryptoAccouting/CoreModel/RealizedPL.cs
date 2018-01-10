using System;
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
        public decimal OpenPrice { get; set; }
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

        public RealizedPL(Instrument coin, EnuPLType plType, DateTime tradeDate, EnuSide side, EnuBaseFiatCCY ccy, 
                          decimal quantity, decimal avgBookPrice, decimal closePrice, Exchange exchange)
        {
            this.TradedCoin = coin;
            this.PLType = plType;
            this.TradeDate = tradeDate;
            this.Side = side;
            this.SettlementCCY = ccy;
            this.Quantity = quantity;
            this.OpenPrice = avgBookPrice;
            this.ClosePrice = closePrice;
            this.exchange = exchange;
        }

        public decimal OpenValue
        {
            get
            {
                return Quantity * OpenPrice;
            }
        }

        public decimal CloseValue
        {
            get
            {
                return Quantity * ClosePrice;
            }
        }

        public decimal GrossPL
        {
            get
            {
                return Side == EnuSide.Sell ? (ClosePrice - OpenPrice) * Quantity : -(ClosePrice - OpenPrice) * Quantity;
            }
        }

        public decimal NetPL
        {
            get
            {
                return GrossPL - TradeFee - MarginFee;
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
