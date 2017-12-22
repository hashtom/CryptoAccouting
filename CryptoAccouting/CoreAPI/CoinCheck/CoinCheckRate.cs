namespace CoinBalance.CoreAPI
{
    public class CoinCheckRate
    {
        public bool success { get; set; }
        public decimal rate { get; set; }
        public decimal price { get; set; }
        public decimal amount { get; set; }
    }
}
