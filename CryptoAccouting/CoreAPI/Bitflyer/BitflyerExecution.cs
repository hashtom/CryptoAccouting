using System;

namespace CoinBalance.CoreAPI
{
    public class BitflyerExecution
    {
        public int id { get; set; }
        public string child_order_id { get; set; }
        public string side { get; set; }
        public decimal price { get; set; }
        public decimal size { get; set; }
        public decimal commission { get; set; }
        public DateTime exec_date { get; set; }
        public string child_order_acceptance_id { get; set; }
    }
}