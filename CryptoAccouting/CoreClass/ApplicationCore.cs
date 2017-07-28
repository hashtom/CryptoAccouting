﻿using System; using System.Collections.Generic; using System.Linq;
//using System.Xml.Serialization; using System.Threading.Tasks; using UIKit; using CryptoAccouting.UIClass; using CryptoAccouting.CoreClass.APIClass;  namespace CryptoAccouting.CoreClass {     public static class ApplicationCore     {         public const string AppName = "CryptoAccounting";         public static EnuCCY BaseCurrency { get; set; }  // Fiats or BTC         private static Balance myBalance;         private static List<Instrument> myInstruments;         //private static TradeList myTradeList;
        private static ExchangeList ExchangeList;         //public static NavigationDrawer Navigation { get; set; }

        //public static NavigationDrawer InitializeSlideMenu(UIView BalanceTableView,         //                                                   UITableViewController PositionViewC,         //                                                   UIViewController TransactionViewC,         //                                                   UITableViewController PLViewC,         //                                                   UIViewController PerfViewC,
        //                                                   UIViewController SettingViewC)
        //{         //    Navigation = new NavigationDrawer(BalanceTableView.Frame.Width, BalanceTableView.Frame.Height,         //                                      PositionViewC,
        //                                      TransactionViewC,         //                                      PLViewC,         //                                      PerfViewC,         //                                      SettingViewC);         //    Navigation.AddView(BalanceTableView);         //    return Navigation;         //}          public static void InitializeCore(bool forceRefresh = true)
        {
			BaseCurrency = EnuCCY.JPY;             LoadInstruments(forceRefresh);             LoadExchangeList();         }          public static void LoadExchangeList()
        {
            if (ExchangeList is null) ExchangeList = new ExchangeList();              var btc = GetInstrument("BTC");             var mona = GetInstrument("MONA");             var xem = GetInstrument("XEM");             var eth = GetInstrument("ETH");             var rep = GetInstrument("REP");             var xlm = GetInstrument("XLM");             var gbyte = GetInstrument("GBYTE");               btc.IsActive = true;             mona.IsActive = true;             xem.IsActive = true;             eth.IsActive = true;             rep.IsActive = true;             xlm.IsActive = true;             gbyte.IsActive = true;              var zaif = new Exchange(EnuExchangeType.Zaif) { ExchangeName = "Zaif" };             zaif.AttachListedCoin(btc);             zaif.AttachListedCoin(mona);             zaif.AttachListedCoin(xem);             var kraken = new Exchange(EnuExchangeType.Kraken){ ExchangeName = "Kraken" };
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
			bittrex.AttachListedCoin(gbyte);              ExchangeList.AttachExchange(zaif);
            ExchangeList.AttachExchange(kraken);
            ExchangeList.AttachExchange(coincheck);
            ExchangeList.AttachExchange(bitbank);
            ExchangeList.AttachExchange(bitflyer);             ExchangeList.AttachExchange(poloniex);             ExchangeList.AttachExchange(bittrex);          }          public static void LoadInstruments(bool forceRefresh = true)
        {             if (forceRefresh)
            {                 myInstruments = new List<Instrument>();
                MarketDataAPI.FetchAllCoinData(myInstruments);                 StorageAPI.SaveInstrumentXML(myInstruments, "instruments.xml"); 
            }
            else
            {                 myInstruments = StorageAPI.LoadInstrumentXML("instruments.xml");             }          }          //public static Balance GetTestBalance(){

        //    // Test Data
        //    AppSetting.BaseCurrency = EnuBaseCCY.JPY;         //    Balance mybal;          //    LoadInstruments();          //    mybal = new Balance(EnuExchangeType.Zaif){BalanceName="Tomoaki"};          //    // Test Data Creation         //    var coin1 = instruments.Where(i=>i.Symbol =="BTC").First();         //    var coin2 = instruments.Where(i => i.Symbol == "ETH").First();         //    var coin3 = instruments.Where(i => i.Symbol == "REP").First();         //    var pos1 = new Position(coin1, "1") { Amount = 850 };         //    var pos2 = new Position(coin2, "2") { Amount = 1000 };         //    var pos3 = new Position(coin3, "3") { Amount = 25000 };          //    mybal.AttachPosition(pos1);         //    mybal.AttachPosition(pos2);         //    mybal.AttachPosition(pos3);          //    myBalance = mybal;          //    return myBalance;         //}          public static Balance GetMyBalance(){              if (myBalance is null)
            {
                myBalance = StorageAPI.LoadBalanceXML("mybalance.xml", myInstruments);             }              return myBalance;         }          public static void SaveMyBalance(Balance myBalance){              StorageAPI.SaveBalanceXML(myBalance, "mybalance.xml");         }          public static Instrument GetInstrument(string symbol)
        {
            if (myInstruments.Any(i => i.Symbol == symbol))
            {
                return myInstruments.First(i => i.Symbol == symbol);
            }
            else
            {                 return null;             }
         }

        public static async Task RefreshMarketDataAsync(Instrument coin)
        {
            await MarketDataAPI.FetchCoinMarketDataAsync(coin);
        }

        public static List<Instrument> GetInstrumentAll(bool OnlyActive = true)
		{
            return !OnlyActive ? myInstruments : myInstruments.Where(x => x.IsActive is true).ToList();
		}          public static async Task LoadTradeListsAsync(EnuExchangeType extype, bool isAggregatedDaily = true, bool readfromFile = false)
        {
            var exchange = GetExchange(extype);             exchange.TradeList = await ExchangeAPI.FetchTradeListAsync(exchange.ExchangeType, isAggregatedDaily, readfromFile);             ExchangeList.AttachExchange(exchange);              //return exchange.TradeLists;         }          public static Exchange GetExchange(EnuExchangeType extype)
        {             return ExchangeList.GetExchange(extype);         }

        public static ExchangeList GetExchangesBySymbol(string symbol)
        {
            return ExchangeList.GetExchangesBySymbol(symbol);
        }          public static TradeList GetExchangeTradeList(EnuExchangeType extype, string symbol)
        {             return ExchangeList.GetTradelist(extype,symbol);
        }

		public static void AttachMyBalance(Balance bal)         {
            myBalance = bal;         }

		public static DateTime FromEpochSeconds(long EpochSeconds)
		{
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return epoch.AddSeconds(EpochSeconds);

		}          public static async Task FetchMarketDataFromBalance()         {             await MarketDataAPI.FetchMarketDataFromBalance(myBalance);         }      }

    public enum EnuCCY
    {         //Fiat Only at the moment
        JPY,
        USD,         EUR     }
      public enum EnuAppStatus{         Success,         Failure     }  } 