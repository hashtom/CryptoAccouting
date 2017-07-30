using System;
using System.Collections.Generic;
using CryptoAccouting.CoreClass;

namespace CryptoAccouting.UIClass
{
    public static class AppSetting
    {
        public static EnuCCY BaseCurrency { get; set; }
        public static string Setting1 { get; set; }

        public static BalanceMainViewController balanceMainViewC { get; set; }
        public static TransactionViewController transViewC  { get; set; }
        public static PLTableViewController plViewC { get; set; }
        public static PerfomViewController perfViewC { get; set; }
        public static SettingTableViewController settingViewC { get; set; }

	}

}
