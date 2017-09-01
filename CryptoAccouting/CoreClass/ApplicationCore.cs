﻿using System; using System.Collections.Generic; using System.Linq; using System.Threading.Tasks; using CryptoAccouting.CoreClass.APIClass;  namespace CryptoAccouting.CoreClass {     public static class ApplicationCore     {         public const string AppName = "CryptoAccounting";         public static Balance Balance { get; private set; }         private static EnuCCY baseCurrency;         public static InstrumentList InstrumentList { get; private set; }
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
        {             EnuAPIStatus status;              //Load Instruments Data             status = LoadInstruments(false);              //Load Exchange List
            if (status is EnuAPIStatus.Success) LoadExchangeList();

			//Load Balance Data
			Balance = StorageAPI.LoadBalanceXML("mybalance.xml", InstrumentList);             Balance.ReCalculate();              //Load App Configuration + API keys             if (StorageAPI.LoadAppSettingXML("AppSetting.xml") != EnuAPIStatus.Success)             {                 BaseCurrency = EnuCCY.USD; //Default setting             }              return status;          }          public static async Task<EnuAPIStatus> LoadCoreDataAsync(){

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
        {             return StorageAPI.SaveAppSettingXML("AppSetting.xml", AppName, BaseCurrency, PublicExchangeList);         }          private static EnuAPIStatus LoadExchangeList()
        {
            if (PublicExchangeList is null) PublicExchangeList = new ExchangeList();             return MarketDataAPI.FetchExchangeList(PublicExchangeList);         }          public static EnuAPIStatus LoadInstruments(bool forceRefresh)
        {             if (InstrumentList is null) InstrumentList = new InstrumentList();              if (forceRefresh)
            {                 var status = MarketDataAPI.FetchAllCoinData(InstrumentList);                 if (status == EnuAPIStatus.Success) SaveInstrumentXML();                 if (Balance != null) Balance.AttachInstruments(InstrumentList);                 return status;
            }
            else
            {
                StorageAPI.LoadInstrumentXML("instruments.xml", InstrumentList);                 if (InstrumentList.Count() == 0)
                {
                    return LoadInstruments(true);                 }
                else
                {
                    return EnuAPIStatus.Success;
                }
            }          }          public static void SaveInstrumentXML()
        {             StorageAPI.SaveInstrumentXML(InstrumentList, "instruments.xml");         }          public static void SaveMyBalanceXML(){              StorageAPI.SaveBalanceXML(Balance, "mybalance.xml");         }          //public static Instrument GetInstrumentSymbol1(string symbol)
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
        public static void LoadMarketDataXML()
        {
            InstrumentList = StorageAPI.LoadMarketDataXML("marketdata.xml");
        }

        public static void SaveMarketDataXML()
        {

                StorageAPI.SaveMarketDataXML("marketdata.xml");

        }

        public static async Task FetchMarketDataAsync(Instrument coin)
        {
            await MarketDataAPI.FetchCoinMarketDataAsync(coin);
        }      }      public enum EnuAPIStatus{         Success,         FailureNetwork,         FailureStorage,         FailureParameter,         NotAvailable     }  } 