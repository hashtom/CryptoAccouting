﻿using System; using System.Collections.Generic; using System.Linq;
//using System.Xml.Serialization; using System.Threading.Tasks; using UIKit; using CryptoAccouting.UIClass; using CryptoAccouting.CoreClass.APIClass;  namespace CryptoAccouting.CoreClass {     public static class ApplicationCore     {         public const string AppName = "CryptoAccounting";         public static EnuCCY BaseCurrency { get; set; } // AppSetting
        private static List<APIKey> APIKeys; // AppSetting         public static Balance Balance { get; private set; }         private static List<Instrument> myInstruments;
        private static ExchangeList ExchangeList;         //public static NavigationDrawer Navigation { get; set; }

        //public static NavigationDrawer InitializeSlideMenu(UIView BalanceTableView,         //                                                   UITableViewController PositionViewC,         //                                                   UIViewController TransactionViewC,         //                                                   UITableViewController PLViewC,         //                                                   UIViewController PerfViewC,
        //                                                   UIViewController SettingViewC)
        //{         //    Navigation = new NavigationDrawer(BalanceTableView.Frame.Width, BalanceTableView.Frame.Height,         //                                      PositionViewC,
        //                                      TransactionViewC,         //                                      PLViewC,         //                                      PerfViewC,         //                                      SettingViewC);         //    Navigation.AddView(BalanceTableView);         //    return Navigation;         //}          public static EnuAppStatus InitializeCore()
        {             EnuAppStatus status;              //Load App Configuration             if (StorageAPI.LoadAppSettingXML("AppSetting.xml") != EnuAppStatus.Success){                 BaseCurrency = EnuCCY.USD; //Default setting             }              //Load Instruments Data             status = LoadInstruments(false);
            if (status is EnuAppStatus.Success) LoadExchangeList();              //Load Balance Data
			Balance = StorageAPI.LoadBalanceXML("mybalance.xml", myInstruments);              //Load Latest Snapshot price               return status;          }          public static EnuAppStatus SaveAppSetting()
        {             return StorageAPI.SaveAppSettingXML("AppSetting.xml", AppName, BaseCurrency, APIKeys);         }          public static void AttatchAPIKey(APIKey Key)         {             if (APIKeys is null) APIKeys = new List<APIKey>();             APIKeys.Add(Key);          }          public static void LoadExchangeList()
        {
            if (ExchangeList is null) ExchangeList = new ExchangeList();              var btc = GetInstrument("BTC");             var mona = GetInstrument("MONA");             var xem = GetInstrument("XEM");             var eth = GetInstrument("ETH");             var rep = GetInstrument("REP");             var xlm = GetInstrument("XLM");             var gbyte = GetInstrument("GBYTE");
            var bch = GetInstrument("BCH");              //btc.IsActive = true;             //mona.IsActive = true;             //xem.IsActive = true;             //eth.IsActive = true;             //rep.IsActive = true;             //xlm.IsActive = true;             //gbyte.IsActive = true;             //bch.IsActive = true;              var zaif = new Exchange(EnuExchangeType.Zaif) { ExchangeName = "Zaif" };             zaif.AttachListedCoin(btc);             zaif.AttachListedCoin(mona);             zaif.AttachListedCoin(xem);             var kraken = new Exchange(EnuExchangeType.Kraken){ ExchangeName = "Kraken" };
			kraken.AttachListedCoin(btc);             kraken.AttachListedCoin(eth);             kraken.AttachListedCoin(rep);             kraken.AttachListedCoin(xlm);             var coincheck = new Exchange(EnuExchangeType.CoinCheck){ ExchangeName = "CoinCheck" };             coincheck.AttachListedCoin(btc);
            coincheck.AttachListedCoin(rep);                                  var bitflyer = new Exchange(EnuExchangeType.BitFlyer){ ExchangeName = "BitFlyer" };
            bitflyer.AttachListedCoin(btc);
            bitflyer.AttachListedCoin(eth);
            var bitbank = new Exchange(EnuExchangeType.BitBank){ ExchangeName = "BitBank" };
            bitbank.AttachListedCoin(btc);              var poloniex = new Exchange(EnuExchangeType.Poloniex) { ExchangeName = "Poloniex" };             poloniex.AttachListedCoin(xlm);
			poloniex.AttachListedCoin(eth);
			poloniex.AttachListedCoin(rep); 
            var bittrex = new Exchange(EnuExchangeType.Bittrex) { ExchangeName = "Bittrex" };
			bittrex.AttachListedCoin(xlm);             bittrex.AttachListedCoin(eth);             bittrex.AttachListedCoin(mona);
			bittrex.AttachListedCoin(gbyte);             bittrex.AttachListedCoin(bch);              ExchangeList.AttachExchange(zaif);
            ExchangeList.AttachExchange(kraken);
            ExchangeList.AttachExchange(coincheck);
            ExchangeList.AttachExchange(bitbank);
            ExchangeList.AttachExchange(bitflyer);             ExchangeList.AttachExchange(poloniex);             ExchangeList.AttachExchange(bittrex);          }          public static EnuAppStatus LoadInstruments(bool forceRefresh)
        {             if (forceRefresh)
            {                 var status = MarketDataAPI.FetchAllCoinData(myInstruments);                 if ( status == EnuAppStatus.Success)
                {                     SaveInstrumentXML();
                }
                 return status;
            }
            else
            {                 myInstruments = StorageAPI.LoadInstrumentXML("instruments.xml");                 if (myInstruments == null)
                {
                    myInstruments = new List<Instrument>();
                    return LoadInstruments(true);                 }
                else
                {
                    return EnuAppStatus.Success;
                }
            }          }          public static void SaveInstrumentXML()
        {             StorageAPI.SaveInstrumentXML(myInstruments, "instruments.xml");         }          public static void SaveMyBalanceXML(){              StorageAPI.SaveBalanceXML(Balance, "mybalance.xml");         }          public static Instrument GetInstrument(string symbol)
        {
            if (myInstruments.Any(i => i.Symbol == symbol))
            {
                return myInstruments.First(i => i.Symbol == symbol);
            }
            else
            {                 return null;             }
         }

        public static List<Instrument> GetInstrumentAll(bool OnlyActive = true)
		{
            return !OnlyActive ? myInstruments : myInstruments.Where(x => x.IsActive is true).ToList();
		}          public static async Task LoadTradeListsAsync(EnuExchangeType extype, bool isAggregatedDaily = true, bool readfromFile = false)
        {
            var exchange = GetExchange(extype);             var apikey = APIKeys.Where(x => x.ExchangeType == extype).First();              exchange.TradeList = await ExchangeAPI.FetchTradeListAsync(apikey, isAggregatedDaily, readfromFile);             ExchangeList.AttachExchange(exchange);              //return exchange.TradeLists;         }          public static Exchange GetExchange(EnuExchangeType extype)
        {             return ExchangeList.GetExchange(extype);         }

        public static ExchangeList GetExchangesBySymbol(string symbol)
        {
            return ExchangeList.GetExchangesBySymbol(symbol);
        }          public static TradeList GetExchangeTradeList(EnuExchangeType extype, string symbol)
        {             return ExchangeList.GetTradelist(extype,symbol);
        }

		//public static void AttachMyBalance(Balance bal)         //{
        //    myBalance = bal;         //}

		public static DateTime FromEpochSeconds(long EpochSeconds)
		{
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return epoch.AddSeconds(EpochSeconds);

		} 

		public static async Task FetchMarketDataFromBalanceAsync()
		{
			Instrument bitcoin;

            if (Balance != null)
            {
                if (Balance.positions.Any(x => x.Coin.Symbol == "BTC"))
                {
                    bitcoin = Balance.positions.Where(x => x.Coin.Symbol == "BTC").Select(x => x.Coin).First();
                }
                else
                {
                    bitcoin = new Instrument("Bitcoin", "BTC", "Bitcoin");
                }

                await MarketDataAPI.FetchCoinMarketDataAsync(bitcoin);

                foreach (var pos in Balance.positions.Where(x => x.Coin.Symbol != "BTC"))
                {
                    if (pos.Coin != null) await MarketDataAPI.FetchCoinMarketDataAsync(pos.Coin, bitcoin);
                }             }
		}

		public static void LoadMarketDataXML()
		{
            myInstruments = StorageAPI.LoadMarketDataXML("marketdata.xml");
		}

		public static void SaveMarketDataXML()
		{

                StorageAPI.SaveMarketDataXML("marketdata.xml");

		}

		public static async Task FetchMarketDataAsync(Instrument coin)
		{
			await MarketDataAPI.FetchCoinMarketDataAsync(coin);
		}        }

    public enum EnuCCY
    {
        JPY,
        USD,         EUR,         BTC     }
      public enum EnuAppStatus{         Success,         SuccessButOffline,         FailureNetwork,         FailureStorage,         FailureParameter     }  } 