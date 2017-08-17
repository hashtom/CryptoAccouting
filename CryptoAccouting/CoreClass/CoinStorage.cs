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

        public CoinStorage(string Code)
        {
            this.Code = Code;
        }

		public Balance BalanceOnStorage()
		{
			if (ApplicationCore.Balance is null)
			{
				return null;
			}
			else
			{
				Balance bal = new Balance();
				foreach (var pos in ApplicationCore.Balance.Where(x => x.Storage.Code == Code))
				{
					bal.AttachPosition(pos);
				}
				return bal;
			}
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
