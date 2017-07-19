﻿using System; using System.Collections.Generic; using System.Linq;
//using System.Xml.Serialization; using System.Threading.Tasks; using UIKit; using CryptoAccouting.UIClass; using CryptoAccouting.CoreClass.APIClass;  namespace CryptoAccouting.CoreClass {     public static class ApplicationCore     {         public const string AppName = "CryptoAccounting";         private static Balance myBalance;         private static List<Instrument> myInstruments;         private static TradeList myTradeList;         public static NavigationDrawer Navigation { get; set; }
        //private static List<Exchange> Exchanges;

        public static NavigationDrawer InitializeSlideMenu(UIView BalanceTableView,                                                            UITableViewController PositionViewC,                                                            UIViewController TransactionViewC,                                                            UITableViewController PLViewC,                                                            UIViewController PerfViewC,
                                                           UIViewController SettingViewC)
        {             Navigation = new NavigationDrawer(BalanceTableView.Frame.Width, BalanceTableView.Frame.Height,                                               PositionViewC,
                                              TransactionViewC,                                               PLViewC,                                               PerfViewC,                                               SettingViewC);             Navigation.AddView(BalanceTableView);             return Navigation;         }          public static void LoadInstruments(bool dorefresh = true)
        {             if (dorefresh)
            {                 myInstruments = new List<Instrument>();
                MarketDataAPI.FetchAllCoinData(myInstruments);                 myInstruments.Where(i => i.Symbol == "BTC").First().LogoFileName = "Images/btc.png";                 myInstruments.Where(i => i.Symbol == "ETH").First().LogoFileName = "Images/eth.png";                 myInstruments.Where(i => i.Symbol == "REP").First().LogoFileName = "Images/rep.png";                 StorageAPI.SaveInstrumentXML(myInstruments, "instruments.xml"); 
            }
            else
            {                 myInstruments = StorageAPI.LoadInstrumentXML("instruments.xml");             }          }          //public static Balance GetTestBalance(){

        //    // Test Data
        //    AppSetting.BaseCurrency = EnuBaseCCY.JPY;         //    Balance mybal;          //    LoadInstruments();          //    mybal = new Balance(EnuExchangeType.Zaif){BalanceName="Tomoaki"};          //    // Test Data Creation         //    var coin1 = instruments.Where(i=>i.Symbol =="BTC").First();         //    var coin2 = instruments.Where(i => i.Symbol == "ETH").First();         //    var coin3 = instruments.Where(i => i.Symbol == "REP").First();         //    var pos1 = new Position(coin1, "1") { Amount = 850 };         //    var pos2 = new Position(coin2, "2") { Amount = 1000 };         //    var pos3 = new Position(coin3, "3") { Amount = 25000 };          //    mybal.AttachPosition(pos1);         //    mybal.AttachPosition(pos2);         //    mybal.AttachPosition(pos3);          //    myBalance = mybal;          //    return myBalance;         //}          public static Balance GetMyBalance(){              if (myBalance is null)
            {
                myBalance = StorageAPI.LoadBalanceXML("mybalance.xml", myInstruments);             }              return myBalance;         }          public static Instrument GetInstrument(string symbol){             return myInstruments.Count == 0 ? null : myInstruments.Where(i => i.Symbol == symbol).First();         }

        public static List<Instrument> GetInstrumentAll()
		{
			return myInstruments;
		}          public static async Task<TradeList> GetTradeListAsync(EnuExchangeType extype, bool isAggregatedDaily, bool readfromFile)
        {             if (myTradeList is null)
            {
                myTradeList = await ExchangeAPI.FetchTransactionAsync(extype, true, readfromFile);             }              return myTradeList;         } 
        public static void AttachMyBalance(Balance bal)         {
            myBalance = bal;         }

		public static DateTime FromEpochSeconds(long EpochSeconds)
		{
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return epoch.AddSeconds(EpochSeconds);

		}          public static async Task FetchMarketData()
        {             await MarketDataAPI.FetchCoinMarketData(myBalance);         }      }

	public enum EnuBaseCCY
	{
		JPY,
		USD
	}      public enum EnuAppStatus{         Success,         Failure     }  } 