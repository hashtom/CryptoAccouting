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
            if (ExchangeList is null) ExchangeList = new ExchangeList();              var zaif = new Exchange(EnuExchangeType.Zaif) { ExchangeName = "Zaif" };             zaif.AttachListedCoin(GetInstrument("BTC"));             zaif.AttachListedCoin(GetInstrument("MONA"));             zaif.AttachListedCoin(GetInstrument("XEM"));             var kraken = new Exchange(EnuExchangeType.Kraken){ ExchangeName = "Kraken" };
			kraken.AttachListedCoin(GetInstrument("BTC"));             kraken.AttachListedCoin(GetInstrument("ETH"));             kraken.AttachListedCoin(GetInstrument("REP"));             var coincheck = new Exchange(EnuExchangeType.CoinCheck){ ExchangeName = "CoinCheck" };             coincheck.AttachListedCoin(GetInstrument("BTC"));
            coincheck.AttachListedCoin(GetInstrument("REP"));                                  var bitflyer = new Exchange(EnuExchangeType.BitFlyer){ ExchangeName = "BitFlyer" };
            bitflyer.AttachListedCoin(GetInstrument("BTC"));
            bitflyer.AttachListedCoin(GetInstrument("ETH"));
            var bitbank = new Exchange(EnuExchangeType.BitBank){ ExchangeName = "BitBank" };
            bitbank.AttachListedCoin(GetInstrument("BTC"));             ExchangeList.AttachExchange(zaif);
            ExchangeList.AttachExchange(kraken);
            ExchangeList.AttachExchange(coincheck);
            ExchangeList.AttachExchange(bitbank);
            ExchangeList.AttachExchange(bitflyer);          }          public static void LoadInstruments(bool forceRefresh = true)
        {             if (forceRefresh)
            {                 myInstruments = new List<Instrument>();
                MarketDataAPI.FetchAllCoinData(myInstruments);                 myInstruments.Where(i => i.Symbol == "BTC").First().LogoFileName = "Images/btc.png";                 myInstruments.Where(i => i.Symbol == "ETH").First().LogoFileName = "Images/eth.png";                 myInstruments.Where(i => i.Symbol == "REP").First().LogoFileName = "Images/rep.png";                 StorageAPI.SaveInstrumentXML(myInstruments, "instruments.xml"); 
            }
            else
            {                 myInstruments = StorageAPI.LoadInstrumentXML("instruments.xml");             }          }          //public static Balance GetTestBalance(){

        //    // Test Data
        //    AppSetting.BaseCurrency = EnuBaseCCY.JPY;         //    Balance mybal;          //    LoadInstruments();          //    mybal = new Balance(EnuExchangeType.Zaif){BalanceName="Tomoaki"};          //    // Test Data Creation         //    var coin1 = instruments.Where(i=>i.Symbol =="BTC").First();         //    var coin2 = instruments.Where(i => i.Symbol == "ETH").First();         //    var coin3 = instruments.Where(i => i.Symbol == "REP").First();         //    var pos1 = new Position(coin1, "1") { Amount = 850 };         //    var pos2 = new Position(coin2, "2") { Amount = 1000 };         //    var pos3 = new Position(coin3, "3") { Amount = 25000 };          //    mybal.AttachPosition(pos1);         //    mybal.AttachPosition(pos2);         //    mybal.AttachPosition(pos3);          //    myBalance = mybal;          //    return myBalance;         //}          public static Balance GetMyBalance(){              if (myBalance is null)
            {
                myBalance = StorageAPI.LoadBalanceXML("mybalance.xml", myInstruments);             }              return myBalance;         }          public static void SaveMyBalance(Balance myBalance){              StorageAPI.SaveBalanceXML(myBalance, "mybalance.xml");         }          public static Instrument GetInstrument(string symbol, bool ForcePriceRefresh = false)
        { 
            if (myInstruments.Where(i => i.Symbol == symbol).Any())
            {
                var coin = myInstruments.Where(i => i.Symbol == symbol).First();
				if (ForcePriceRefresh)
				{
					MarketDataAPI.FetchCoinMarketDataAsync(coin).Wait();
				}                 return coin;
            }else{                 return null;             }
         }

        public static List<Instrument> GetInstrumentAll()
		{
			return myInstruments;
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

		}          public static async Task FetchMarketData()
        {             await MarketDataAPI.FetchMarketDataFromBalance(myBalance);         }      }

    public enum EnuCCY
    {         //Fiat Only at the moment
        JPY,
        USD,         EUR     }
      public enum EnuAppStatus{         Success,         Failure     }  } 