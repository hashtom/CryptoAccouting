using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class CoinStorage
    {
		public string Code { get; set; }
		public string Name { get; set; }
        public EnuCoinStorageType StorageType { get; set; }
        public Balance BalanceOnStorage { get; private set; }

        public CoinStorage(string code, EnuCoinStorageType storatetype)
        {
            this.Code = code;
            this.Name = code;
            this.StorageType = storatetype;
            BalanceOnStorage = new Balance();
        }

        public void ClearBalanceOnStorage()
        {
            BalanceOnStorage = new Balance();
        }

        public void AttachPosition(Position position)
        {
            BalanceOnStorage.Attach(position);
        }

        public void DetachPosition(Position position)
        {
            BalanceOnStorage.Detach(position);
        }

        public double AmountBTC()
        {
            return BalanceOnStorage.AmountBTC();
        }

        public double LatestFiatValueUSD()
        {
            return BalanceOnStorage.LatestFiatValueUSD();
        }

		public double LatestFiatValueBase(CrossRate crossrate)
		{
			return BalanceOnStorage.LatestFiatValueBase(crossrate);
		}

    }

    public enum EnuCoinStorageType
    {
        HardwareWallet,
        DesktopWallet,
        MobileWallet,
        WebService,
        Exchange
    }
}
