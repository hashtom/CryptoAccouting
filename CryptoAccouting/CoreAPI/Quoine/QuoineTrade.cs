using System;

namespace CoinBalance.CoreAPI
{
    public class QuoineTrade
    {
        public int Id { get; set; }
        public string currency_pair_code { get; set; }
        public string status { get; set; }
        public string side { get; set; }
        public decimal margin_used { get; set; }
        public decimal open_quantity { get; set; }
        public decimal close_quantity { get; set; }
        public decimal quantity { get; set; }
        public decimal leverage_level { get; set; }
        public string product_code { get; set; }
        public int product_id { get; set; }
        public decimal open_price { get; set; }
        public decimal close_price { get; set; }
        public int trader_id { get; set; }
        public decimal open_pnl { get; set; }
        public decimal close_pnl { get; set; }
        public decimal pnl { get; set; }
        public decimal stop_loss { get; set; }
        public decimal take_profit { get; set; }
        public string funding_currency { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public decimal total_interest { get; set; }
    }
}
