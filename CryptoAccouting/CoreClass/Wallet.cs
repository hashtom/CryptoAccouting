using System;


namespace CryptoAccouting.CoreClass
{
    public class Wallet : CoinStorage
    {
        public string Xpubkey { get; set; }

        public Wallet(string code, EnuCoinStorageType storagetype) : base(code, storagetype)
        {
        }

    }
}
