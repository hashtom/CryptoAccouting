﻿using System; using System.Collections.Generic; using System.Linq;
//using System.Xml.Serialization; using System.Threading.Tasks; using UIKit; using CryptoAccouting.UIClass; using CryptoAccouting.CoreClass.APIClass;  namespace CryptoAccouting.CoreClass {     public static class ApplicationCore     {         public const string AppName = "CryptoAccounting";         public static Balance Balance { get; private set; }         private static EnuCCY baseCurrency;         private static InstrumentList InstrumentList;
        public static ExchangeList ExchangeList;         public static CrossRate USDCrossRate { get; private set; }         private static bool HasCrossRateUpdated = false;          public static EnuCCY BaseCurrency
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
        //                                      TransactionViewC,         //                                      PLViewC,         //                                      PerfViewC,         //                                      SettingViewC);         //    Navigation.AddView(BalanceTableView);         //    return Navigation;         //}          public static EnuAppStatus InitializeCore()
        {             EnuAppStatus status;              //Load Instruments Data             status = LoadInstruments(false);              //Load Exchange List
            if (status is EnuAppStatus.Success) LoadExchangeList();

			//Load Balance Data
			Balance = StorageAPI.LoadBalanceXML("mybalance.xml", InstrumentList);              //Load App Configuration + API keys             if (StorageAPI.LoadAppSettingXML("AppSetting.xml") != EnuAppStatus.Success)             {                 BaseCurrency = EnuCCY.USD; //Default setting             }              //Load Latest Snapshot price              return status;          }          public static async Task<EnuAppStatus> LoadCoreDataAsync(){

            //Load FX
            if (!HasCrossRateUpdated)
            {
                USDCrossRate = await MarketDataAPI.FetchUSDCrossRateAsync(BaseCurrency);                 HasCrossRateUpdated = true;             }              return EnuAppStatus.Success;         } 
        public static async Task<EnuAppStatus> FetchMarketDataFromBalanceAsync()
        { 
            if (Balance != null)
            {                 var mycoins = new InstrumentList();
                Balance.Select(x => x.Coin).Distinct().ToList().ForEach(x => mycoins.Attach(x));
                return await MarketDataAPI.FetchCoinMarketDataAsync(mycoins);
            }
            else
            {                 return EnuAppStatus.NotAvailable;             }
        }          public static EnuAppStatus SaveAppSetting()
        {             return StorageAPI.SaveAppSettingXML("AppSetting.xml", AppName, BaseCurrency, ExchangeList);         }          private static EnuAppStatus LoadExchangeList()
        {
            if (ExchangeList is null) ExchangeList = new ExchangeList();             return MarketDataAPI.FetchExchangeList(ExchangeList);         }          public static EnuAppStatus LoadInstruments(bool forceRefresh)
        {             InstrumentList = new InstrumentList();             if (forceRefresh)
            {                 var status = MarketDataAPI.FetchAllCoinData(InstrumentList);                 if (status == EnuAppStatus.Success) SaveInstrumentXML();                 return status;
            }
            else
            {                 InstrumentList = StorageAPI.LoadInstrumentXML("instruments.xml");                 if (InstrumentList == null)
                {
                    return LoadInstruments(true);                 }
                else
                {
                    return EnuAppStatus.Success;
                }
            }          }          public static void SaveInstrumentXML()
        {             StorageAPI.SaveInstrumentXML(InstrumentList, "instruments.xml");         }          public static void SaveMyBalanceXML(){              StorageAPI.SaveBalanceXML(Balance, "mybalance.xml");         }          public static Instrument GetInstrument(string symbol)
        {
            if (InstrumentList.Any(i => i.Symbol == symbol))
            {
                return InstrumentList.First(i => i.Symbol == symbol);
            }
            else
            {                 return null;             }
         }

        public static InstrumentList GetInstrumentAll(bool OnlyActive = true)
        {
            if (!OnlyActive)
            {                 return InstrumentList;             }
            else
            {                 var list = new InstrumentList();                 foreach (var coin in InstrumentList.Where(x => x.IsActive is true))                 {                     list.Attach(coin);                 }
                return list;             }
        }          public static async Task LoadTradeListsAsync(string ExchangeCode, bool isAggregatedDaily = true, bool readfromFile = false)
        {
            var exchange = GetExchange(ExchangeCode);             //var apikey = APIKeys.Where(x => x.ExchangeType == extype).First();              exchange.TradeList = await ExchangeAPI.FetchTradeListAsync(exchange, isAggregatedDaily, readfromFile);             ExchangeList.Attach(exchange);              //return exchange.TradeLists;         }          public static Exchange GetExchange(string Code)
        {             return ExchangeList.GetExchange(Code);         }

        public static ExchangeList GetExchangesBySymbol(string symbol)
        {
            return ExchangeList.GetExchangesBySymbol(symbol);
        }          public static TradeList GetExchangeTradeList(string exchangeCode)
        {             return ExchangeList.GetTradelist(exchangeCode);
        }

        //public static void AttachMyBalance(Balance bal)         //{
        //    myBalance = bal;         //}

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
        }      }      public enum EnuAppStatus{         Success,         FailureNetwork,         FailureStorage,         FailureParameter,         NotAvailable     }  } 