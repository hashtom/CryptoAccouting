﻿using System; using System.Linq; using System.Threading.Tasks; using System.Collections.Generic; using CryptoAccouting.CoreClass.APIClass;  namespace CryptoAccouting.CoreClass {     public static class ApplicationCore     {         public const string AppName = "CryptoAccounting";         public const string PriceSourceFile = "pricesource.xml";
		public const string InstrumentListFile = "InstrumentList.json";         public const string ExchangeListFile = "ExchangeList.json";         public const string BalanceFile = "mybalance.xml";         public const string BalanceBundleFile = "BalanceData.xml";         public const string AppSettingFile = "AppSetting.xml";         public const string CrossRatefile_today = "crossrate_today.json";         public const string CrossRatefile_yesterday = "crossrate_yesterday.json";         public static Balance Balance { get; private set; }         public static InstrumentList InstrumentList { get; private set; }
        public static ExchangeList PublicExchangeList { get; private set; }         public static CoinStorageList CoinStorageList { get; private set; }         public static List<CrossRate> USDCrossRates { get; private set; }         private static EnuBaseFiatCCY baseCurrency;          public static EnuBaseFiatCCY BaseCurrency
        {             get
            {
                return baseCurrency;
            }             set
            {
                baseCurrency = value;
            }         }          public static CrossRate USDCrossRate         {             get             {                 return USDCrossRates.First(x => x.Currency == baseCurrency);             }         }          public static Instrument Bitcoin         {             get
            {                 return InstrumentList.GetByInstrumentId("bitcoin");
            }         }          //public static NavigationDrawer Navigation { get; set; }

        //public static NavigationDrawer InitializeSlideMenu(UIView BalanceTableView,         //                                                   UITableViewController PositionViewC,         //                                                   UIViewController TransactionViewC,         //                                                   UITableViewController PLViewC,         //                                                   UIViewController PerfViewC,
        //                                                   UIViewController SettingViewC)
        //{         //    Navigation = new NavigationDrawer(BalanceTableView.Frame.Width, BalanceTableView.Frame.Height,         //                                      PositionViewC,
        //                                      TransactionViewC,         //                                      PLViewC,         //                                      PerfViewC,         //                                      SettingViewC);         //    Navigation.AddView(BalanceTableView);         //    return Navigation;         //}          public static EnuAPIStatus InitializeCore()
        {
            //Initialize 
            CoinStorageList = new CoinStorageList();

            // Load the latest file (or Bundle file)             InstrumentList = StorageAPI.LoadInstrument();              //Load ExchangeList             LoadExchangeList();

            //Load Balance Data
            Balance = StorageAPI.LoadBalanceXML(InstrumentList);
            RefreshBalance();              //Load App Configuration + API keys             if (StorageAPI.LoadAppSettingXML(AppSettingFile) != EnuAPIStatus.Success)             {                 BaseCurrency = EnuBaseFiatCCY.USD; //Default setting             }              return EnuAPIStatus.Success;          }          public static async Task<EnuAPIStatus> LoadCrossRateAsync()
        {
            USDCrossRates = await MarketDataAPI.FetchCrossRateAsync();
            if (USDCrossRates is null)
            {
                USDCrossRates = await StorageAPI.LoadCrossRateAsync();             }

            return USDCrossRates != null ? EnuAPIStatus.Success : EnuAPIStatus.NotAvailable;         } 
        public static async Task<EnuAPIStatus> FetchMarketDataFromBalanceAsync()
        { 
            if (Balance != null)
            {                 var mycoins = new InstrumentList(); 
                Balance.Select(x => x.Coin).Distinct().ToList().ForEach(x => mycoins.Attach(x));

                if (!mycoins.Any(x => x.Id == "bitcoin")) mycoins.DetachByInstrumentId("bitcoin");                 mycoins.Insert(0, Bitcoin);                  var status = await MarketDataAPI.FetchCoinPricesAsync(PublicExchangeList, mycoins, USDCrossRates);                 if (status != EnuAPIStatus.Success)                 {                     return status;                 }
                else                 {
                    RefreshBalance(); //update weights,etc with latest price                     SaveMyBalanceXML();//save balance with latest price                     return EnuAPIStatus.Success;                 }             }
            else
            {                 return EnuAPIStatus.NotAvailable;             }
        }          public static EnuAPIStatus SaveAppSetting()
        {             return StorageAPI.SaveAppSettingXML(AppSettingFile, AppName, BaseCurrency, PublicExchangeList);         }          private static EnuAPIStatus LoadExchangeList()
        {
            if (PublicExchangeList is null) PublicExchangeList = new ExchangeList();             return ExchangeAPI.FetchExchangeList(PublicExchangeList);         }          public static EnuAPIStatus SyncLatestCoins()         {
            var list = MarketDataAPI.FetchAllCoinData();             if (list != null)             {                 InstrumentList = list;                 if (Balance != null) Balance.AttachInstruments(InstrumentList);                 Task.Run(async () => await FetchCoinLogoTop100Async());                 return EnuAPIStatus.Success;             }             else             {                 return EnuAPIStatus.FatalError;             }         }          public static async Task<EnuAPIStatus> FetchCoinLogoAsync(Instrument coin)         {              return await MarketDataAPI.FetchCoinLogoAsync(coin.Id, false);         }          public static async Task<EnuAPIStatus>FetchCoinLogoTop100Async()         {             if (InstrumentList == null)
            {
                return EnuAPIStatus.FatalError;             }
            else
            {
                foreach (var coin in InstrumentList.Where(x=>x.rank <= 100))
                {
                    await MarketDataAPI.FetchCoinLogoAsync(coin.Id, false);
                }
                return EnuAPIStatus.Success;             }         }          public static void SaveInstrumentXML()         {             StorageAPI.SavePriceSourceXML(InstrumentList, PriceSourceFile);         }          public static void SaveMyBalanceXML(){              StorageAPI.SaveBalanceXML(Balance, BalanceFile);         }          public static async Task<EnuAPIStatus> LoadTradeListsAsync(string ExchangeCode)
        {
            var exchange = GetExchange(ExchangeCode);             //var apikey = APIKeys.Where(x => x.ExchangeType == extype).First();              if (exchange.APIKeyAvailable())             {                 exchange.AttachTradeList(await ExchangeAPI.FetchTradeListAsync(exchange));                 //PublicExchangeList.Attach(exchange); //do you need?                 return EnuAPIStatus.Success;             }else             {                 return EnuAPIStatus.FailureParameter;             }         }          public static Exchange GetExchange(string Code)
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

        }          public static void RefreshBalance()         {             //if (InstrumentList != null) InstrumentList.AttachCrossRate(USDCrossRate);             Balance.RefreshBalanceData();             CoinStorageList.RecalculateWeights();         }          public static void AttachPosition(Position position, bool DoRefreshBalance = true)         {             Balance.Attach(position);             if (DoRefreshBalance) RefreshBalance();             Task.Run(async () => await FetchCoinLogoAsync(position.Coin));         }          public static void DetachPosition(Position position, bool DoRefreshBalance = true)         {             Balance.Detach(position);             CoinStorageList.DetachPosition(position);             if (DoRefreshBalance) RefreshBalance();         }          public static void DetachPositionByCoin(string InstrumentId, bool DoRefreshBalance = true)         {             Balance.DetachPositionByCoin(InstrumentId);             CoinStorageList.DetachPositionByCoin(InstrumentId);             if (DoRefreshBalance) RefreshBalance();         }          public static void DetachPositionByExchange(Exchange exchange, bool DoRefreshBalance = true)         {             Balance.DetachPositionByExchange(exchange);             CoinStorageList.Detach(exchange);             if (DoRefreshBalance) RefreshBalance();         } 
        public static async Task FetchMarketDataAsync(Instrument coin)
        {
            //await MarketDataAPI.FetchCoinMarketDataAsync(coin);
            var mycoins = new InstrumentList();             if (coin.Symbol1 != "BTC") mycoins.Attach(InstrumentList.First(x => x.Symbol1 == "BTC"));             mycoins.Attach(coin);             await MarketDataAPI.FetchCoinPricesAsync(PublicExchangeList, mycoins, USDCrossRates);
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
            return Reachability.IsHostReachable("http://coinbalance.jpn.org/");
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
        }          public static EnuAPIStatus RemoveAllCache()         {             try
            {                 baseCurrency = EnuBaseFiatCCY.USD;                 PublicExchangeList.ClearAPIKeys();                 return StorageAPI.RemoveAllCache();             }
            catch (Exception)             {                 return EnuAPIStatus.FatalError;             }         }      }      public enum EnuAPIStatus     {         Success,         FailureNetwork,         FailureStorage,         FailureParameter,         NotAvailable,         FatalError,         ParseError     }  } 