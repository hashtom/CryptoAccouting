﻿﻿using System; using System.Linq; using System.Threading.Tasks; using CryptoAccouting.CoreClass.APIClass;  namespace CryptoAccouting.CoreClass {     public static class ApplicationCore     {         public const string AppName = "CryptoAccounting";         public const string InstrumentsFile = "instruments.xml";
		public const string InstrumentsBundleFile = "InstrumentList.json";         public const string BalanceFile = "mybalance.xml";         public const string AppSettingFile = "AppSetting.xml";         public static Balance Balance { get; private set; }         public static InstrumentList InstrumentList { get; private set; }
        public static ExchangeList PublicExchangeList { get; private set; }         public static CoinStorageList CoinStorageList { get; private set; }         public static CrossRate USDCrossRate { get; private set; }         private static EnuCCY baseCurrency;         private static bool HasCrossRateUpdated = false;          public static EnuCCY BaseCurrency
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
        {
            //Initialize 
            CoinStorageList = new CoinStorageList();              //Load Instruments Data and ExchangeList
            if (LoadInstruments(false) is EnuAPIStatus.Success) LoadExchangeList(); 
			//Load Balance Data
            Balance = StorageAPI.LoadBalanceXML(BalanceFile, InstrumentList);
            RefreshBalance();              //Load App Configuration + API keys             if (StorageAPI.LoadAppSettingXML(AppSettingFile) != EnuAPIStatus.Success)             {                 BaseCurrency = EnuCCY.USD; //Default setting             }              return EnuAPIStatus.Success;          }          public static async Task<EnuAPIStatus> LoadUSDCrossRateAsync(){

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
                 var bitcoin = InstrumentList.GetByInstrumentId("bitcoin");
                if (!mycoins.Any(x => x.Id == "bitcoin")) mycoins.DetachByInstrumentId("bitcoin");                 mycoins.Insert(0, bitcoin);                  await MarketDataAPI.FetchCoinPricesAsync(PublicExchangeList, mycoins, USDCrossRate);                 RefreshBalance(); //update weights,etc with latest price                 SaveMyBalanceXML();//save balance with latest price                 return EnuAPIStatus.Success;             }
            else
            {                 return EnuAPIStatus.NotAvailable;             }
        }          public static EnuAPIStatus SaveAppSetting()
        {             return StorageAPI.SaveAppSettingXML(AppSettingFile, AppName, BaseCurrency, PublicExchangeList);         }          private static EnuAPIStatus LoadExchangeList()
        {
            if (PublicExchangeList is null) PublicExchangeList = new ExchangeList();             return MarketDataAPI.FetchExchangeList(PublicExchangeList);         }          public static EnuAPIStatus LoadInstruments(bool forceRefresh)
        {                          //Force update online             if (forceRefresh)
            {                 InstrumentList = new InstrumentList();                  var status = MarketDataAPI.FetchAllCoinData(InstrumentList, false);                 if (status == EnuAPIStatus.Success)
                {
                    if (Balance != null) Balance.AttachInstruments(InstrumentList);                 }                  Task.Run(async () => await FetchCoinLogoAsync());                  return status;
            }
            else
            {                 if (InstrumentList is null) InstrumentList = new InstrumentList(); 
                // 1. Load the latest file
                if (StorageAPI.LoadInstrumentXML(InstrumentsFile, InstrumentList) != EnuAPIStatus.Success)
                {
					//  Update online
					//if (LoadInstruments(true) != EnuAPIStatus.Success)                     //{
						// 2.Use Bundled file 
                    return MarketDataAPI.FetchAllCoinData(InstrumentList, true);                     //}                 }
                 return EnuAPIStatus.Success;
            }         }          public static async Task<EnuAPIStatus>FetchCoinLogoAsync()         {             if (InstrumentList == null)
            {
                return EnuAPIStatus.FatalError;             }
            else
            {
                foreach (var coin in InstrumentList)
                {
                    await MarketDataAPI.FetchCoinLogoAsync(coin.Id, false);
                }
                return EnuAPIStatus.Success;             }         }          public static void SaveInstrumentXML()         {             StorageAPI.SaveInstrumentXML(InstrumentList, InstrumentsFile);         }          public static void SaveMyBalanceXML(){              StorageAPI.SaveBalanceXML(Balance, BalanceFile);         }          public static async Task<EnuAPIStatus> LoadTradeListsAsync(string ExchangeCode, string calendarYear = null, bool isAggregatedDaily = true)
        {
            var exchange = GetExchange(ExchangeCode);             //var apikey = APIKeys.Where(x => x.ExchangeType == extype).First();              if (exchange.APIKeyAvailable())             {                 exchange.TradeList = await ExchangeAPI.FetchTradeListAsync(exchange, calendarYear, isAggregatedDaily);                 //PublicExchangeList.Attach(exchange); //do you need?                 return EnuAPIStatus.Success;             }else             {                 return EnuAPIStatus.FailureParameter;             }         }          public static Exchange GetExchange(string Code)
        {             return PublicExchangeList.GetExchange(Code);         }

        public static ExchangeList GetExchangeListByInstrument(string id)
        {
            return PublicExchangeList.GetExchangesByInstrument(id);
        }          public static TradeList GetExchangeTradeList(string exchangeCode)
        {             return PublicExchangeList.GetTradelist(exchangeCode);
        }
         public static long ToEpochSeconds(DateTime dt)         {             var dto = new DateTimeOffset(dt.Ticks, new TimeSpan(+09, 00, 00));             return dto.ToUnixTimeSeconds();         } 
        public static DateTime FromEpochSeconds(long EpochSeconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(EpochSeconds);

        }          public static void RefreshBalance()         {             Balance.RefreshBalanceData();             CoinStorageList.RecalculateWeights();         }          public static void AttachPosition(Position position, bool DoRefreshBalance = true)         {             Balance.Attach(position);             if (DoRefreshBalance) RefreshBalance();         }          public static void DetachPosition(Position position, bool DoRefreshBalance = true)         {             Balance.Detach(position);             CoinStorageList.DetachPosition(position);             if (DoRefreshBalance) RefreshBalance();         }          public static void DetachPositionByCoin(string InstrumentId, bool DoRefreshBalance = true)         {             Balance.DetachPositionByCoin(InstrumentId);             CoinStorageList.DetachPositionByCoin(InstrumentId);             if (DoRefreshBalance) RefreshBalance();         }          public static void DetachPositionByExchange(Exchange exchange, bool DoRefreshBalance = true)         {             Balance.DetachPositionByExchange(exchange);             CoinStorageList.Detach(exchange);             if (DoRefreshBalance) RefreshBalance();         } 
        public static async Task FetchMarketDataAsync(Instrument coin)
        {
            //await MarketDataAPI.FetchCoinMarketDataAsync(coin);
            var mycoins = new InstrumentList();             if (coin.Symbol1 != "BTC") mycoins.Attach(InstrumentList.First(x => x.Symbol1 == "BTC"));             mycoins.Attach(coin);             await MarketDataAPI.FetchCoinPricesAsync(PublicExchangeList, mycoins, USDCrossRate);
        }          public static CoinStorage GetCoinStorage(string storagecode, EnuCoinStorageType storagetype)         {             return CoinStorageList.Any(x => (x.Code == storagecode && x.StorageType == storagetype))
                                  ? CoinStorageList.First(x => (x.Code == storagecode && x.StorageType == storagetype))                                       : null;         }          public static void AttachCoinStorage(string storagecode, EnuCoinStorageType storagetype, Position pos)         {             var storage = GetCoinStorage(storagecode, storagetype); 
            if (storage is null)             {
                switch (storagetype)
                {
                    case EnuCoinStorageType.Exchange:                         storage = GetExchange(storagecode);                         CoinStorageList.Attach(storage);
                        break;
                    default:                         storage = new Wallet(storagecode, storagetype);                         CoinStorageList.Attach(storage);
                        break;
                }
            }             storage.AttachPosition(pos);             pos.AttachCoinStorage(storage);         } 
		public static bool IsInternetReachable()
		{
			return Reachability.IsHostReachable("http://bridgeplace.sakura.ne.jp");
		} 
        public static string NumberFormat(double number, bool addPlus = false, bool digitAdjust = true)
        {             double epsilon = 1e-10;
            var digit = unchecked((int)Math.Log10(Math.Abs(number))) + 1;
            string strnumber;              if (digit > 6 && digitAdjust)
            {
                strnumber = String.Format("{0:n2}", number / 1000000) + "MM"; 
            }             else if (digit > 3 && digitAdjust)             {                 strnumber = String.Format("{0:n0}", number);              }
            else if (digit <= 1 && digitAdjust)
            {
                strnumber = Math.Abs(number) < epsilon ? "0" : String.Format("{0:n7}", number);
            }             else             {                 strnumber = String.Format("{0:n2}", number);             }              if (addPlus && number > 0) strnumber = "+" + strnumber;             return strnumber;
        }          public static EnuAPIStatus RemoveAllCache()         {             return StorageAPI.RemoveAllCache();         }          public static Instrument Bitcoin()         {             return InstrumentList.GetByInstrumentId("bitcoin");         }      }      public enum EnuAPIStatus{         Success,         FailureNetwork,         FailureStorage,         FailureParameter,         NotAvailable,         FatalError     }  } 