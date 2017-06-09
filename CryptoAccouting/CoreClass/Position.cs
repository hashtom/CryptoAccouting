using System;
using System.Collections.Generic;

namespace CryptoAccouting
{
    public class Position
    {
        public string PositionID { get; set; }
        public Instrument Asset { get; }    
        public int Amount { get; set; }
        public Price PriceData { get; }

		public DateTime BalanceDate { get; set; }
		public DateTime UpdateTime { get; }



		public Position(Instrument asset)
        {
            Asset = asset;
        }

        public void attachPriceData(Price p){
           // PriceData = p;
        }

    }
}
