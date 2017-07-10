using System;

namespace CryptoAccouting
{
    public static class AppConfig
    {
        public static EnuBaseCCY BaseCurrency { get; set; }
    }


	public enum EnuBaseCCY
	{
		JPY,
		USD
	}
}
