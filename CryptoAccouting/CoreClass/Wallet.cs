using System;
using System.Collections.Generic;

namespace CryptoAccouting.CoreClass
{
    public class Wallet
    {
		public string Code { get; set; }
		public string Name { get; set; }
        public EnuWalletType Type { get; set; }
        public Balance BalanceOnWallet { get; private set; }

        public Wallet(string Code, string Name, EnuWalletType Type)
        {
            this.Code = Code;
            this.Name = Name;
            this.Type = Type;
            //this.BalanceOnWallet = BalanceOnWallet;
        }

		public void AttachPosition(Position pos)
		{
			if (BalanceOnWallet is null) BalanceOnWallet = new Balance();
			BalanceOnWallet.AttachPosition(pos);
		}

		public void DetachPosition(Position pos)
		{
			BalanceOnWallet.DetachPosition(pos, true);
		}

    }

    public enum EnuWalletType
    {
        Hardware,
        Desktop,
        Mobile,
        WebService,
        Exchange
    }
}
