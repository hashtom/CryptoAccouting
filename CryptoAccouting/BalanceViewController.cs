using Foundation; using System; using UIKit; using CryptoAccouting.CoreClass; using CryptoAccouting.UIClass;   namespace CryptoAccouting {     public partial class BalanceViewController : UITableViewController     {         private Balance myBalance;         private NavigationDrawer menu;         static NSString MyCellId = new NSString("BalanceCell");          public BalanceViewController(IntPtr handle) : base(handle)         {             AppSetting.BaseCurrency = EnuBaseCCY.JPY;
            //myBalance = ApplicationCore.GetTestBalance();
            myBalance = ApplicationCore.LoadBalanceXML("mybalance.xml");
        }           public override void ViewDidLoad()         {             base.ViewDidLoad();             TableView.Source = new BalanceTableSource(myBalance, this);             TableView.RegisterClassForCellReuse(typeof(CustomBalanceCell), MyCellId);             TransactionViewController transViewC = this.Storyboard.InstantiateViewController("TransactionViewC") as TransactionViewController;
            SettingTableViewController settingViewC = this.Storyboard.InstantiateViewController("SettingTableViewC") as SettingTableViewController;             menu = ApplicationCore.InitializeSlideMenu(TableView, this, transViewC, settingViewC);
            NavigationItem.RightBarButtonItem = EditButtonItem; //test             //ApplicationCore.SaveBalanceXML(myBalance, "mybalance.xml");
        }          public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
             await MarketDataAPI.FetchCoinMarketData(ApplicationCore.GetInstrumentAll());             TableView.ReloadData();             labelTotalAsset.Text = ApplicationCore.GetInstrument("BTC").MarketPrice.LatestPrice.ToString();         }          public override void ViewDidAppear(bool animated)
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
                    var source = TableView.Source as BalanceTableSource;
                    var rowPath = TableView.IndexPathForSelectedRow;
                    //var rowPath = TableView.IndexPathsForVisibleRows[0];
                    var item = source.GetItem(rowPath.Row);
                    navctlr.SetItem(this, item);
                }             }
        }

        partial void MenuBar_Activated(UIBarButtonItem sender)
        {
            menu.Navigation.ToggleDrawer();
        }

	} }