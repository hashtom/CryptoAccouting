using System;
namespace CryptoAccouting
{
    public class Position
    {
        public string Code { get; set; }
        public int Amount { get; set; }
        public DateTime BalanceDate { get; set; }
        public double ClosePrice { get; set; }
        public string CcyPrice { get; set; }
        public DateTime LastUpdate { get; set; }

        public Position()
        {
        }
    }
}
