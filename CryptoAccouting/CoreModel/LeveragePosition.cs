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
        public EnuMarginSide Side { get; set; }
        public double Quantity { get; set; }
        public double TradePriceSettle { get; set; }
        public double Fee { get; set; }
        public double Profit { get; set; }
    }

    public enum EnuMarginSide
    {
        Long,
        Short
    }

    public enum EnuMarginStatus
    {
        Open,
        Closed
    }
}
