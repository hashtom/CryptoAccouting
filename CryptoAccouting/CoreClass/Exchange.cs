using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class Exchange : CoinStorage
    {
        public InstrumentList Coins { get; private set; }
        public bool APIReady { get; set; }
		public string Key { get; set; }
		public string Secret { get; set; }
		public TradeList TradeList { get; set; }
        public string LogoFileName { get; set; }


        public Exchange(string code, EnuCoinStorageType storagetype) : base(code, storagetype)
        {
            Coins = new InstrumentList();
            if (storagetype != EnuCoinStorageType.Exchange) throw new Exception();
        }

        public void AttachListedCoin(Instrument coin)
		{
            if (!Coins.Any(c => c.Id == coin.Id)) Coins.Attach(coin);
		}

    }

}
