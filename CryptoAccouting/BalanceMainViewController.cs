using Foundation; using System; using UIKit; using CryptoAccouting.CoreClass; using CryptoAccouting.UIClass; using System.Threading.Tasks;  namespace CryptoAccouting {     public partial class BalanceMainViewController : BalanceTableViewController     {
        //private NavigationDrawer menu;         //private Balance myBalance;          public BalanceMainViewController(IntPtr handle) : base(handle)         {             AppSetting.BaseCurrency = EnuCCY.JPY;
        }           public override void ViewDidLoad()         {             base.ViewDidLoad();              // Instantiate Controllers             AppSetting.balanceMainViewC = this;             AppSetting.transViewC = this.Storyboard.InstantiateViewController("TransactionViewC") as TransactionViewController;             AppSetting.plViewC = this.Storyboard.InstantiateViewController("PLViewC") as PLTableViewController;             AppSetting.perfViewC = this.Storyboard.InstantiateViewController("PerfViewC") as PerfomViewController;             AppSetting.settingViewC = this.Storyboard.InstantiateViewController("SettingTableViewC") as SettingTableViewController;             //menu = ApplicationCore.InitializeSlideMenu(TableView, this, transViewC, plViewC, perfViewC, settingViewC);
             if (ApplicationCore.InitializeCore() != EnuAppStatus.Success){                 this.PopUpWarning("some issue!!");             }                                                        //myBalance = ApplicationCore.Balance;              // Configure Segmented control             ConfigureSegmentButton();              // Configure Table source             TableView.RegisterNibForCellReuse(BalanceViewCell.Nib, "BalanceViewCell");             TableView.Source = new BalanceTableSource(ApplicationCore.Balance, this);
        }          public async override void ViewWillAppear(bool animated)         {
            base.ViewWillAppear(animated);             await ApplicationCore.FetchMarketDataFromBalance();             RefreshBalanceTable();
            labelTotalFiat.Text = "$" + String.Format("{0:n2}", ApplicationCore.Balance.LatestFiatValue());             labelTotalBTC.Text = String.Format("{0:n2}", ApplicationCore.Balance.LatestBTCValue());         }          public override void ViewDidAppear(bool animated)
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
                        TableView.Source = new BalanceTableSource(ApplicationCore.Balance, this);
                        break;
                    case 1:
                        TableView.RegisterNibForCellReuse(ExchangeViewCell.Nib, "ExchangeViewCell");
                        TableView.Source = new ExchangeTableSource(ApplicationCore.Balance, this);
                        break;
                    case 2:
                        TableView.RegisterNibForCellReuse(ExchangeViewCell.Nib, "ExchangeViewCell");
                        TableView.Source = new ExchangeTableSource(ApplicationCore.Balance, this);
                        break;
                    default:
                        break;
                }
                //TableView.ReloadData();
            };          }

        private void RefreshBalanceTable()
        {
            ApplicationCore.Balance.SortPositionByHolding();
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

  //      public void SaveItem(Position pos)
		//{
  //          ApplicationCore.Balance.AttachPosition(pos);   //          ApplicationCore.SaveMyBalanceXML();
		//	TableView.ReloadData();   //          NavigationController.PopToRootViewController(true);
		//}

  //      public override void DeleteItem(Position pos)
		//{
  //          myBalance.DetachPosition(pos);   //          TableView.ReloadData();   //          //ApplicationCore.SaveMyBalance(myBalance);   //          NavigationController.PopToRootViewController(true);
		//}      } }