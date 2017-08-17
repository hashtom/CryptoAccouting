using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class Exchange : CoinStorage
    {
        //public string Code { get; set; }
        //public string Name { get; set; }
        public List<Instrument> ListedCoin { get; set; }
        public bool APIReady { get; set; }
		public string Key { get; set; }
		public string Secret { get; set; }
		public TradeList TradeList { get; set; }
        public string LogoFileName { get; set; }
        //public DateTime UpdateTime { get; private set; }


        public Exchange(string code) :base (code)
        {
            ListedCoin = new List<Instrument>();
            StorageType = EnuCoinStorageType.Exchange;
        }

		public void AttachListedCoin(Instrument coin)
		{
            if (!ListedCoin.Any(c => c.Symbol == coin.Symbol)) ListedCoin.Add(coin);
		}

    }

}
