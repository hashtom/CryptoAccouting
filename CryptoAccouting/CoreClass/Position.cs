using System;
using System.Collections.Generic;

namespace CryptoAccouting
{
    public class Position
    {
        public string PositionID { get; set; }
        public Instrument Asset { get; }    
        public int Amount { get; set; }
        public DateTime BalanceDate { get; set; }
        public double ClosePrice { get; set; }
        public string CcyPrice { get; set; }
        public DateTime UpdateTime { get; }

        public Position(Instrument asset)
        {
            Asset = asset;
        }
    }
}
