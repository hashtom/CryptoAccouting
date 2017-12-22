using System;
namespace CoinBalance.CoreAPI
{
    public class BitflyerTicker
    {
        public string product_code { get; set; }
        public DateTime timestamp { get; set; }
        public int tick_id { get; set; }
        public decimal best_bid { get; set; }
        public decimal best_ask { get; set; }
        public decimal best_bid_size { get; set; }
        public decimal best_ask_size { get; set; }
        public decimal total_bid_depth { get; set; }
        public decimal total_ask_depth { get; set; }
        public decimal ltp { get; set; }
        public decimal volume { get; set; }
        public decimal volume_by_product { get; set; }
    }
}
