using Foundation; using System; using UIKit; using System.Collections.Generic; using System.Threading.Tasks; using CryptoAccouting.CoreClass;  namespace CryptoAccouting {     public partial class BalanceViewController : UITableViewController     {         private Balance myBalance;          public BalanceViewController(IntPtr handle) : base(handle)         {             AppConfig.BaseCurrency = EnuBaseCCY.JPY;
			myBalance = ApplicationCore.GetTestBalance();
		}          public override void ViewDidLoad()         {
			BalanceTableView.Source = new BalanceTableSource(myBalance);         }          public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated); 
            //test
            //var exg = new ExchangeAPI();             //var p = await exg.FetchBTCPriceAsyncTest(EnuExchangeType.Zaif);
            //labelTotalAsset.Text = p.LatestPrice.ToString();             await MarketDataAPI.FetchCoinMarketData(ApplicationCore.GetInstrumentAll());             BalanceTableView.ReloadData();          }          public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }     } }