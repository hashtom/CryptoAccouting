using System.Collections.Generic;

namespace CoinBalance.CoreAPI
{
    public class QuoineProduct
    {
        public string Id { get; set; }
        public string product_type { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public decimal market_ask { get; set; }
        public decimal market_bid { get; set; }
        public int indicator { get; set; }
        public string currency { get; set; }
        public string currency_pair_code { get; set; }
        public string symbol { get; set; }
        public decimal? btc_minimum_withdraw { get; set; }
        public decimal? fiat_minimum_withdraw { get; set; }
        public string pusher_channel { get; set; }
        public decimal taker_fee { get; set; }
        public decimal maker_fee { get; set; }
        public decimal low_market_bid { get; set; }
        public decimal high_market_ask { get; set; }
        public decimal volume_24h { get; set; }
        public decimal last_price_24h { get; set; }
        public decimal? last_traded_price { get; set; }
        public decimal? last_traded_quantity { get; set; }
        public string quoted_currency { get; set; }
        public string base_currency { get; set; }
        public decimal exchange_rate { get; set; }
        public bool disabled { get; set; }
    }
}
