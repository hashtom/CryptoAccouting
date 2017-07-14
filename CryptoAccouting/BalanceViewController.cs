using Foundation; using System; using UIKit; //using System.Collections.Generic; //using System.Threading.Tasks; using CryptoAccouting.CoreClass; using CryptoAccouting.UIClass; //using Syncfusion.SfNavigationDrawer.iOS; //using CoreGraphics;  namespace CryptoAccouting {     public partial class BalanceViewController : UITableViewController     {         private Balance myBalance;         private NavigationDrawer menu;          public BalanceViewController(IntPtr handle) : base(handle)         {             AppSetting.BaseCurrency = EnuBaseCCY.JPY;
            myBalance = ApplicationCore.GetTestBalance();
        }           public override void ViewDidLoad()         {             base.ViewDidLoad();
            menu = ApplicationCore.InitializeSlideMenu(BalanceTableView);             NavigationItem.RightBarButtonItem = EditButtonItem; //test             BalanceTableView.Source = new BalanceTableSource(myBalance);         }          public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated); 
            //test
            //var exg = new ExchangeAPI();             //var p = await exg.FetchBTCPriceAsyncTest(EnuExchangeType.Zaif);
            //labelTotalAsset.Text = p.LatestPrice.ToString();             await MarketDataAPI.FetchCoinMarketData(ApplicationCore.GetInstrumentAll());             BalanceTableView.ReloadData();          }          public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
         public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier == "PositionSegue")
            {
                var navctlr = segue.DestinationViewController as PositionDetailViewController;
                if (navctlr != null)
                {
                    var source = BalanceTableView.Source as BalanceTableSource;
                    var rowPath = BalanceTableView.IndexPathForSelectedRow;
                    var item = source.GetItem(rowPath.Row);
                    navctlr.SetItem(this, item);
                }
            } 
        }          public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //base.RowSelected(tableView, indexPath);             //var cell = tableView.CellAt(indexPath);             //BalanceTableView.DeselectRow(indexPath, animated: true);              PerformSegue("PositionSegue", this); 
        }

        partial void MenuBar_Activated(UIBarButtonItem sender)
        {
            menu.Navigation.ToggleDrawer();
        }         } }