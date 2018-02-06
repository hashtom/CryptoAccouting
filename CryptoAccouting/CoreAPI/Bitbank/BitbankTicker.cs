using System;

namespace CoinBalance.CoreAPI
{
    public class BitbankTicker
    {
        public int success { get; set; }
        public Tick _data { get; set; }

        public class Tick
        {
            public decimal sell { get; set; }
            public decimal buy { get; set; }
            public decimal high { get; set; }
            public decimal low { get; set; }
            public decimal last { get; set; }
            public decimal vol { get; set; }
            public DateTime timestamp { get; set; }
        }
    }
}
