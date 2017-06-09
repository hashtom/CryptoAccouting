using System;
using System.Collections.Generic;

namespace CryptoAccouting
{
    public class Position
    {
        public string PositionID { get; }
        public Instrument Asset { get; }    
        public int Amount { get; set; }
        public Price PriceData { get; private set; }

		public DateTime BalanceDate { get; set; }
        public DateTime UpdateTime { get; private set; }



		public Position(Instrument asset)
        {
            Asset = asset;
        }

        public void AttachPriceData(Price p){
           PriceData = p;
        }

    }
}
