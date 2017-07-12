using System;
using System.Collections.Generic;
using CryptoAccouting.CoreClass;

namespace CryptoAccouting
{
    public static class AppConfig
    {
        public static EnuBaseCCY BaseCurrency { get; set; }
        public static string Setting1 { get; set; }
    }


	public enum EnuBaseCCY
	{
		JPY,
		USD
	}


}
