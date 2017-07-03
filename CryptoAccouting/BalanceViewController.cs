using Foundation; using System; using UIKit; using System.Collections.Generic; using System.Threading.Tasks;  namespace CryptoAccouting {     public partial class BalanceViewController : UITableViewController     {         private Balance myBalance;          public BalanceViewController(IntPtr handle) : base(handle)         {
            myBalance = CryptoAccoutingCore.FetchMyBalance();             AppConfig.BaseCurrency = "JPY";         }          public override void ViewDidLoad()         {                      }          public async override void ViewWillAppear(bool animated) 		{ 			base.ViewWillAppear(animated);             //TableView.Source = new HeaderTableSource();             TableView.Source = new BalanceTableSource(myBalance);

            //var testzaifprice = new ExchangeAPI(); //test

            //test
            var exg = new ExchangeAPI();              var p = await exg.FetchPriceAsync(EnuExchangeType.Zaif);             labelTotalAsset.Text = p.LatestPrice.ToString();//test              var txs = await exg.FetchTransaction((EnuExchangeType.Zaif));              labelCurrency.Text = AppConfig.BaseCurrency;   		}     } }