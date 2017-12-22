namespace CoinBalance.CoreAPI
{
    public class BitflyerPosition
    {
        public string product_code { get; set; }
        public string side { get; set; }
        public decimal price { get; set; }
        public decimal size { get; set; }
        public decimal commission { get; set; }
        public decimal swap_point_accumulate { get; set; }
        public decimal require_collateral { get; set; }
        public string open_date { get; set; }
        public int leverage { get; set; }
        public decimal pnl { get; set; }
    }
}
