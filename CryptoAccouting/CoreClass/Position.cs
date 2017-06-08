using System;
using System.Collections.Generic;

namespace CryptoAccouting
{
    public class Position
    {
        public AssetAttribute Asset { get; }    
        public int Amount { get; set; }
        public DateTime BalanceDate { get; set; }
        public double ClosePrice { get; set; }
        public string CcyPrice { get; set; }
        public DateTime UpdateTime { get; }

        public Position(AssetAttribute asset)
        {
            Asset = asset;
        }
    }
}
