using Foundation; using System; using UIKit; using CryptoAccouting.CoreClass; using CryptoAccouting.UIClass; using System.Threading.Tasks;  namespace CryptoAccouting {     public partial class BalanceViewController : UITableViewController     {
        //private NavigationDrawer menu;         private Balance myBalance;          public BalanceViewController(IntPtr handle) : base(handle)         {             AppSetting.BaseCurrency = EnuCCY.JPY;
        }           public override void ViewDidLoad()         {             base.ViewDidLoad();              // Instantiate Controllers             AppSetting.balanceViewC = this;             AppSetting.transViewC = this.Storyboard.InstantiateViewController("TransactionViewC") as TransactionViewController;             AppSetting.plViewC = this.Storyboard.InstantiateViewController("PLViewC") as PLTableViewController;             AppSetting.perfViewC = this.Storyboard.InstantiateViewController("PerfViewC") as PerfomViewController;             AppSetting.settingViewC = this.Storyboard.InstantiateViewController("SettingTableViewC") as SettingTableViewController;             //menu = ApplicationCore.InitializeSlideMenu(TableView, this, transViewC, plViewC, perfViewC, settingViewC);

            ApplicationCore.InitializeCore();             myBalance = ApplicationCore.GetMyBalance();              // Configure Segmented control             ConfigureSegmentButton();              // Configure Table source             TableView.RegisterNibForCellReuse(BalanceViewCell.Nib, "BalanceViewCell");             TableView.Source = new BalanceTableSource(myBalance, this);
        }          public async override void ViewWillAppear(bool animated)         {
            base.ViewWillAppear(animated);             await ApplicationCore.FetchMarketDataFromBalance();             RefreshBalanceTable();
            labelTotalFiat.Text = "$" + String.Format("{0:n2}", myBalance.LatestFiatValue());             labelTotalBTC.Text = String.Format("{0:n2}", myBalance.LatestBTCValue());         }          public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
         public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier == "PositionSegue")
            {
                var navctlr = segue.DestinationViewController as BalanceEditViewController;
                if (navctlr != null)
                {
                    var source = TableView.Source as BalanceTableSource;
                    var rowPath = TableView.IndexPathForSelectedRow;
                    var item = source.GetItem(rowPath.Row);
                    navctlr.SetPosition(item);
                }             }
        }
         private void ConfigureSegmentButton()
        {

            SegmentBalance.ValueChanged += (sender, e) =>
            {
                var selectedSegmentId = (sender as UISegmentedControl).SelectedSegment;

                switch (selectedSegmentId)
                {
                    case 0:
                        TableView.RegisterNibForCellReuse(BalanceViewCell.Nib, "BalanceViewCell");
                        TableView.Source = new BalanceTableSource(ApplicationCore.GetMyBalance(), this);
                        break;
                    case 1:
                        TableView.RegisterNibForCellReuse(ExchangeViewCell.Nib, "ExchangeViewCell");
                        TableView.Source = new ExchangeTableSource(ApplicationCore.GetMyBalance(), this);
                        break;
                    case 2:
                        TableView.RegisterNibForCellReuse(ExchangeViewCell.Nib, "ExchangeViewCell");
                        TableView.Source = new ExchangeTableSource(ApplicationCore.GetMyBalance(), this);
                        break;
                    default:
                        break;
                }
                TableView.ReloadData();
            };          }

        //    partial void ButtonSwitch_TouchUpInside(UIButton sender)
        //    {
        //        if (viewposition is 0)
        //        {
        //            TableView.RegisterNibForCellReuse(ExchangeViewCell.Nib, "ExchangeViewCell" );
        //            TableView.Source = new ExchangeTableSource(ApplicationCore.GetMyBalance(), this);
        //            //labelViewTitle.Text = "By Exchange";
        //            viewposition = 1;
        //        }else{
        //TableView.RegisterNibForCellReuse(BalanceViewCell.Nib, "BalanceViewCell");
        //TableView.Source = new BalanceTableSource(ApplicationCore.GetMyBalance(), this);
        //            //labelViewTitle.Text = "Total Balance";
        //viewposition = 0;
        //    }
        //    TableView.ReloadData();
        //}

        private void RefreshBalanceTable()
        {
            myBalance.SortPositionByHolding();
            TableView.ReloadData();         } 
        partial void ButtonAddNew_Activated(UIBarButtonItem sender)
        {
            CreateNewPosition();
        }

        partial void ButtonRefresh_Activated(UIBarButtonItem sender)
		{
            RefreshBalanceTable();
		}          private void CreateNewPosition(){
            var SymbolSelectionViewC = Storyboard.InstantiateViewController("SymbolSelectionViewC") as SymbolSelectionViewConroller;
            NavigationController.PushViewController(SymbolSelectionViewC, true);          }

        public void SaveItem(Position pos)
		{
            myBalance.AttachPosition(pos);             TableView.ReloadData();             ApplicationCore.SaveMyBalance(myBalance);
			//NavigationController.PopViewController(true);             NavigationController.PopToRootViewController(true);
		}

		public void DeleteItem(Position pos)
		{
            myBalance.DetachPosition(pos);             TableView.ReloadData();             ApplicationCore.SaveMyBalance(myBalance);
			//NavigationController.PopViewController(true);             //NavigationController.PopToRootViewController(true);
		}     } }