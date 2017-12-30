using System;
namespace CoinBalance.CoreAPI
{
    public class BinanceTicker
    {
        public string symbol { get; set; }
        public decimal priceChange { get; set; }
        public decimal priceChangePercent { get; set; }
        public decimal weightedAvgPrice { get; set; }
        public decimal prevClosePrice { get; set; }
        public decimal lastPrice { get; set; }
        public decimal bidPrice { get; set; }
        public decimal askPrice { get; set; }
        public decimal openPrice { get; set; }
        public decimal highPrice { get; set; }
        public decimal lowPrice { get; set; }
        public decimal volume { get; set; }
        public decimal quoteVolume { get; set; }
        public DateTime openTime { get; set; }
        public DateTime closeTime { get; set; }
    }
}
