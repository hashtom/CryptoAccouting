using Foundation;
//using System;
//using UIKit;
//using System.Collections.Generic;
//using CoinBalance.CoreClass;

namespace CoinBalance.UIModel
{
    public static class AppSetting
    {
        //public static EnuCCY BaseCurrency { get; set; }
        //{
        //    get { return ApplicationCore.BaseCurrency; } 
        //    set { ApplicationCore.BaseCurrency = value; }
        //}
        //public static List<APIKey> APIKeys { get; set; }

        public static BalanceMainViewController balanceMainViewC { get; set; }
        public static TransactionViewController transViewC { get; set; }
        public static PLViewController plViewC { get; set; }
        public static SettingTableViewController settingViewC { get; set; }
        public static string Lang = NSBundle.MainBundle.PreferredLocalizations[0];
	}

}
