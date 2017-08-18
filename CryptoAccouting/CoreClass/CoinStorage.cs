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

        public CoinStorage(string Code)
        {
            this.Code = Code;
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
