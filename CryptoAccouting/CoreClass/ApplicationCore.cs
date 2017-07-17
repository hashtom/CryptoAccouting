using System; using System.Collections.Generic; using System.Linq;
//using System.Xml.Serialization;
using System.Xml.Linq; //using System.Threading.Tasks; using System.IO; using UIKit; using CryptoAccouting.UIClass;  namespace CryptoAccouting.CoreClass {     public static class ApplicationCore     {         public const string AppName = "CryptoAccounting";         private static Balance myBalance;         private static List<Instrument> instruments;         public static NavigationDrawer Navigation { get; set; }
        //private static List<Exchange> Exchanges;

        public static NavigationDrawer InitializeSlideMenu(UIView BalanceTableView,                                                            UITableViewController PositionViewC,                                                            UIViewController TransactionViewC,
                                                           UIViewController SettingViewC){             Navigation = new NavigationDrawer(PositionViewC, TransactionViewC, SettingViewC);             Navigation.AddView(BalanceTableView);             return Navigation;         }          public static void LoadInstruments(){             instruments = new List<Instrument>();
            instruments.Add(new Instrument("bitcoin", "BTC") { Type = InstrumentType.Crypto, LogoFileName = "Images/btc.png" });             instruments.Add(new Instrument("ethereum", "ETH") { Type = InstrumentType.Crypto, LogoFileName = "Images/eth.png" });             instruments.Add(new Instrument("augur", "REP") { Type = InstrumentType.Crypto, LogoFileName = "Images/rep.png" });          }          //public static Balance GetTestBalance(){

        //    // Test Data
        //    AppSetting.BaseCurrency = EnuBaseCCY.JPY;         //    Balance mybal;          //    LoadInstruments();          //    mybal = new Balance(EnuExchangeType.Zaif){BalanceName="Tomoaki"};          //    // Test Data Creation         //    var coin1 = instruments.Where(i=>i.Symbol =="BTC").First();         //    var coin2 = instruments.Where(i => i.Symbol == "ETH").First();         //    var coin3 = instruments.Where(i => i.Symbol == "REP").First();         //    var pos1 = new Position(coin1, "1") { Amount = 850 };         //    var pos2 = new Position(coin2, "2") { Amount = 1000 };         //    var pos3 = new Position(coin3, "3") { Amount = 25000 };          //    mybal.AttachPosition(pos1);         //    mybal.AttachPosition(pos2);         //    mybal.AttachPosition(pos3);          //    myBalance = mybal;          //    return myBalance;         //}          public static Balance GetMyBalance(){             return myBalance;         }          public static Instrument GetInstrument(string symbol){             return instruments.Where(i => i.Symbol == symbol).First();         }

        public static List<Instrument> GetInstrumentAll()
		{
			return instruments;
		} 
        public static void AttachMyBalance(Balance bal)         {
            myBalance = bal;         }

        public static void SaveJsonFile(string json, string fileName)
		{

			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(documents, fileName);
			File.WriteAllText(path, json);

		}


        public static string LoadFromJsonFile(string fileName)
        {

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documents, fileName);
            return File.ReadAllText(path);
        } 
		public static DateTime FromEpochSeconds(long EpochSeconds)
		{
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return epoch.AddSeconds(EpochSeconds);

		}          public static Balance LoadBalanceXML(string fileName){
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(documents, fileName);
			var balanceXML = File.ReadAllText(path);              LoadInstruments();              var mybalXE = XElement.Parse(balanceXML).Descendants("position");             var mybal = new Balance(EnuExchangeType.Zaif);

            //var doc = XElement.Parse(xmldoc).Descendants("broker").Where(n => n.Attribute("name").Value == "Zaif");
            //apikey = doc.Descendants("key").Select(x => x.Value).First();
            //apisecret = doc.Descendants("secret").Select(x => x.Value).First();
            foreach (var elem in mybalXE){                 var coin = instruments.Where(i => i.Symbol == elem.Descendants("symbol").Select(x => x.Value).First()).First();                 var pos = new Position(coin, (string)elem.Attribute("id").Value) { Amount = double.Parse(elem.Descendants("amount").Select(x => x.Value).First())};                                                                                                        mybal.AttachPosition(pos);             }              myBalance = mybal;              return myBalance;         }          public static void SaveBalanceXML(Balance myBalance, string fileName){

			var mydocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);             var path = Path.Combine(mydocuments, fileName);              XElement application = new XElement("application",
                                                new XAttribute("name", ApplicationCore.AppName));             XElement balance = new XElement("balance",                                             new XAttribute("exchange", myBalance.ExchangeTraded.ToString()));             application.Add(balance);              foreach (var pos in myBalance){                 XElement position = new XElement("position",                                                  new XAttribute("id", pos.Id.ToString()),                                                  new XElement("symbol", pos.Coin.Symbol),                                                  new XElement("date", pos.BalanceDate),                                                  new XElement("amount", pos.Amount.ToString()),                                                  new XElement("book", pos.BookPrice.ToString())                                                 );                 balance.Add(position);             }
			
            File.WriteAllText(path, application.ToString());          }      }

	public enum EnuBaseCCY
	{
		JPY,
		USD
	}      public enum EnuAppStatus{         Success,         Failure     }  } 