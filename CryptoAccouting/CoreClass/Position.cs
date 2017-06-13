using System;
using System.Collections.Generic;

namespace CryptoAccouting
{
    public class Position
    {
        public string Id { get; }
        public Instrument CoinAsset { get; }    
        public int Amount { get; set; }
        public Price PriceData { get; private set; }

		public DateTime BalanceDate { get; set; }
        public DateTime UpdateTime { get; private set; }



		public Position(Instrument asset, string positionId)
        {
            CoinAsset = asset;
            Id = positionId;
        }

        public void AttachPriceData(Price p){
           PriceData = p;
        }

    }
}
