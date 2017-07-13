using Foundation; using System; using UIKit; //using System.Collections.Generic; //using System.Threading.Tasks; using CryptoAccouting.CoreClass; using CryptoAccouting.UIClass; //using Syncfusion.SfNavigationDrawer.iOS; //using CoreGraphics;  namespace CryptoAccouting {     public partial class BalanceViewController : UITableViewController     {         private Balance myBalance;          public BalanceViewController(IntPtr handle) : base(handle)         {             AppSetting.BaseCurrency = EnuBaseCCY.JPY;
			myBalance = ApplicationCore.GetTestBalance();
		}           public override void ViewDidLoad()         {             var menu = ApplicationCore.InitializeSlideMenu(BalanceTableView);             menu.Navigation.ToggleDrawer(); // Test             BalanceTableView.Source = new BalanceTableSource(myBalance);         }          public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated); 
            //test
            //var exg = new ExchangeAPI();             //var p = await exg.FetchBTCPriceAsyncTest(EnuExchangeType.Zaif);
            //labelTotalAsset.Text = p.LatestPrice.ToString();             await MarketDataAPI.FetchCoinMarketData(ApplicationCore.GetInstrumentAll());             BalanceTableView.ReloadData();          }          public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }     } }