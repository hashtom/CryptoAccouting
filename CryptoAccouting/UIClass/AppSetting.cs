﻿﻿using System;
using System.Collections.Generic;
using CoinBalance.CoreClass;
using CoinBalance.CoreClass.APIClass;

namespace CoinBalance.UIClass
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
        public static TransactionViewController transViewC  { get; set; }
        public static PLTableViewController plViewC { get; set; }
        public static SettingTableViewController settingViewC { get; set; }

	}

}
