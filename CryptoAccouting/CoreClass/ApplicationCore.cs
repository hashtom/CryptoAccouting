﻿using System; using System.Collections.Generic; using System.Linq; using System.Threading.Tasks; using CryptoAccouting.CoreClass.APIClass;  namespace CryptoAccouting.CoreClass {     public static class ApplicationCore     {         public const string AppName = "CryptoAccounting";         public const string InstrumentsFile = "instruments.xml";
		public const string InstrumentsBundleFile = "InstrumentList.json";         public const string BalanceFile = "mybalance.xml";         public const string AppSettingFile = "AppSetting.xml";          public static Balance Balance { get; private set; }         private static EnuCCY baseCurrency;         public static InstrumentList InstrumentList { get; private set; }
        public static ExchangeList PublicExchangeList { get; private set; }          public static CrossRate USDCrossRate { get; private set; }         private static bool HasCrossRateUpdated = false;          public static EnuCCY BaseCurrency
        {             get
            {
                return baseCurrency;
            }             set
            {
                baseCurrency = value;
                HasCrossRateUpdated = false;
            }         }          //public static NavigationDrawer Navigation { get; set; }

        //public static NavigationDrawer InitializeSlideMenu(UIView BalanceTableView,         //                                                   UITableViewController PositionViewC,         //                                                   UIViewController TransactionViewC,         //                                                   UITableViewController PLViewC,         //                                                   UIViewController PerfViewC,
        //                                                   UIViewController SettingViewC)
        //{         //    Navigation = new NavigationDrawer(BalanceTableView.Frame.Width, BalanceTableView.Frame.Height,         //                                      PositionViewC,
        //                                      TransactionViewC,         //                                      PLViewC,         //                                      PerfViewC,         //                                      SettingViewC);         //    Navigation.AddView(BalanceTableView);         //    return Navigation;         //}          public static EnuAPIStatus InitializeCore()
        {             //Load Instruments Data and ExchangeList
            if (LoadInstruments(false) is EnuAPIStatus.Success) LoadExchangeList(); 
			//Load Balance Data
            Balance = StorageAPI.LoadBalanceXML(BalanceFile, InstrumentList);             Balance.ReCalculate();              //Load App Configuration + API keys             if (StorageAPI.LoadAppSettingXML(AppSettingFile) != EnuAPIStatus.Success)             {                 BaseCurrency = EnuCCY.USD; //Default setting             }              return EnuAPIStatus.Success;          }          public static async Task<EnuAPIStatus> LoadCoreDataAsync(){

            //Load FX
            if (!HasCrossRateUpdated)
            {
                USDCrossRate = await MarketDataAPI.FetchUSDCrossRateAsync(BaseCurrency);                 if (USDCrossRate is null)                 {
                    return EnuAPIStatus.NotAvailable;                 }
                else
                {
                    HasCrossRateUpdated = true;                 }
            }              return EnuAPIStatus.Success;         } 
        public static async Task<EnuAPIStatus> FetchMarketDataFromBalanceAsync()
        { 
            if (Balance != null)
            {                 var mycoins = new InstrumentList();
                Balance.Select(x => x.Coin).Distinct().ToList().ForEach(x => mycoins.Attach(x));
                //return await MarketDataAPI.FetchCoinMarketDataAsync(mycoins, USDCrossRate);
                return await MarketDataAPI.FetchCoinPricesAsync(PublicExchangeList, mycoins, USDCrossRate);             }
            else
            {                 return EnuAPIStatus.NotAvailable;             }
        }          public static EnuAPIStatus SaveAppSetting()
        {             return StorageAPI.SaveAppSettingXML(AppSettingFile, AppName, BaseCurrency, PublicExchangeList);         }          private static EnuAPIStatus LoadExchangeList()
        {
            if (PublicExchangeList is null) PublicExchangeList = new ExchangeList();             return MarketDataAPI.FetchExchangeList(PublicExchangeList);         }          public static EnuAPIStatus LoadInstruments(bool forceRefresh)
        {             if (InstrumentList is null) InstrumentList = new InstrumentList();              //Force update online             if (forceRefresh)
            {                 var status = MarketDataAPI.FetchAllCoinData(InstrumentList, true);                 if (status == EnuAPIStatus.Success)
                {
                    if (Balance != null) Balance.AttachInstruments(InstrumentList);                 }                 return status;
            }
            else
            {
                // 1. Load the latest file
                if (StorageAPI.LoadInstrumentXML(InstrumentsFile, InstrumentList) != EnuAPIStatus.Success)
                {
					// 2. Try update online
					if (LoadInstruments(true) != EnuAPIStatus.Success)                     {
						// 3. Use Bundled file 
						return MarketDataAPI.FetchAllCoinData(InstrumentList, false);                     }                 }
                 return EnuAPIStatus.Success;
            }         }           public static void SaveInstrumentXML()         {             StorageAPI.SaveInstrumentXML(InstrumentList, InstrumentsFile);         }          public static void SaveMyBalanceXML(){              StorageAPI.SaveBalanceXML(Balance, BalanceFile);         }          //public static Instrument GetInstrumentSymbol1(string symbol)
        //{
        //    if (InstrumentList.Any(i => i.Symbol1 == symbol))
        //    {
        //        return InstrumentList.First(i => i.Symbol1 == symbol);
        //    }
        //    else
        //    {         //        return null;         //    }         //}


        //public static InstrumentList InstrumentList()
        //{
            //if (!OnlyActive)
            //{             //    return InstrumentList;             //}
            //else
            //{                 //var list = new InstrumentList();                 //InstrumentList.Where(x => x.IsActive is true).ToList().ForEach(x => list.Attach(x));
                //return list;             //}
        //}          public static CoinStorageList GetStorageList()
        {             return Balance is null ? null : Balance.CoinStorageList;         }          //public static CoinStorage GetCoinStorage(string code, EnuCoinStorageType storagetype)         //{         //    return Balance is null ? null : StorageList().GetCoinStorage(code, storagetype);         //}           //取引データ取得         public static async Task LoadTradeListsAsync(string ExchangeCode, bool isAggregatedDaily = true, bool readfromFile = false)
        {
            var exchange = GetExchange(ExchangeCode);             //var apikey = APIKeys.Where(x => x.ExchangeType == extype).First();              exchange.TradeList = await ExchangeAPI.FetchTradeListAsync(exchange, isAggregatedDaily, readfromFile);             PublicExchangeList.Attach(exchange);              //return exchange.TradeLists;         }          public static Exchange GetExchange(string Code)
        {             return PublicExchangeList.GetExchange(Code);         }

        public static ExchangeList GetExchangeListByInstrument(string id)
        {
            return PublicExchangeList.GetExchangesByInstrument(id);
        }          public static TradeList GetExchangeTradeList(string exchangeCode)
        {             return PublicExchangeList.GetTradelist(exchangeCode);
        }

        public static DateTime FromEpochSeconds(long EpochSeconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(EpochSeconds);

        } 
        //public static void LoadMarketDataXML()
        //{
        //    InstrumentList = StorageAPI.LoadMarketDataXML("marketdata.xml");
        //}

        //public static void SaveMarketDataXML()
        //{

        //        StorageAPI.SaveMarketDataXML("marketdata.xml");

        //}

        public static async Task FetchMarketDataAsync(Instrument coin)
        {
            //await MarketDataAPI.FetchCoinMarketDataAsync(coin);
            var mycoins = new InstrumentList();             if (coin.Symbol1 != "BTC") mycoins.Attach(InstrumentList.First(x => x.Symbol1 == "BTC"));             mycoins.Attach(coin);             await MarketDataAPI.FetchCoinPricesAsync(PublicExchangeList, mycoins, USDCrossRate);
        }

        public static string NumberFormat(double number)
        {             double epsilon = 1e-10;
            var digit = unchecked((int)Math.Log10(Math.Abs(number))) + 1;
             if (digit > 6)
            {
                return String.Format("{0:n2}", number / 1000000) + "MM";
            }
            else if (digit <= 1)
            {
                return Math.Abs(number) < epsilon ? "0" : String.Format("{0:n6}", number);
            }             else             {                 return String.Format("{0:n2}", number);             }
        }      }      public enum EnuAPIStatus{         Success,         FailureNetwork,         FailureStorage,         FailureParameter,         NotAvailable,         FatalError     }  } 