namespace CoinBalance.CoreAPI
{
    public class BitflyerBalance
    {
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public decimal Available { get; set; }
    }
}