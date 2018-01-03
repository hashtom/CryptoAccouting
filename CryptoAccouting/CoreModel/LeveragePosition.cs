using System;
namespace CoinBalance.CoreModel
{
    public class LeveragePosition
    {
        public int Id { get; set; }
        public EnuMarginStatus Status { get; set; }
        public Instrument TradedCoin { get; }
        public AssetType Type { get; set; }
        public EnuCCY SettlementCCY { get; set; }
        public DateTime TradeDate { get; set; }
        public Exchange TradeExchange { get; }
        public EnuSide Side { get; set; }
        public decimal Quantity { get; set; }
        public decimal TradePriceSettle { get; set; }
        public decimal Fee { get; set; }
        public decimal Profit { get; set; }
    }

    //public enum EnuMarginSide
    //{
    //    Long,
    //    Short
    //}

    public enum EnuMarginStatus
    {
        Open,
        Closed
    }
}
