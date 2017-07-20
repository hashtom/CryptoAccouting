using Foundation; using System; using UIKit; using CryptoAccouting.CoreClass; using CryptoAccouting.UIClass; using System.Threading.Tasks;  namespace CryptoAccouting {     public partial class BalanceViewController : UITableViewController     {         //private Balance myBalance;         private NavigationDrawer menu;         static NSString MyCellId = new NSString("BalanceCell");          public BalanceViewController(IntPtr handle) : base(handle)         {             AppSetting.BaseCurrency = EnuCCY.JPY;             NavigationItem.RightBarButtonItem = EditButtonItem; //test
        }           public override void ViewDidLoad()         {             base.ViewDidLoad();                                      // Instantiate Controllers             TransactionViewController transViewC = this.Storyboard.InstantiateViewController("TransactionViewC") as TransactionViewController;             PLTableViewController plViewC = this.Storyboard.InstantiateViewController("PLViewC") as PLTableViewController;             PerfomViewController perfViewC = this.Storyboard.InstantiateViewController("PerfViewC") as PerfomViewController;             SettingTableViewController settingViewC = this.Storyboard.InstantiateViewController("SettingTableViewC") as SettingTableViewController;             menu = ApplicationCore.InitializeSlideMenu(TableView, this, transViewC, plViewC, perfViewC, settingViewC);
    
            ApplicationCore.LoadInstruments();
            TableView.Source = new BalanceTableSource(ApplicationCore.GetMyBalance(), this);
            TableView.RegisterClassForCellReuse(typeof(CustomBalanceCell), MyCellId);
        }          public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);             await ApplicationCore.FetchMarketData();
			TableView.ReloadData();
			labelTotalAsset.Text = ApplicationCore.GetInstrument("BTC").MarketPrice.LatestPrice.ToString();         }          public override void ViewDidAppear(bool animated)
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
                    var item = source.GetItem(rowPath.Row);
                    navctlr.SetItem(this, item);
                }             }
        }

        partial void MenuBar_Activated(UIBarButtonItem sender)
        {
            menu.Navigation.ToggleDrawer();
        }

	} }