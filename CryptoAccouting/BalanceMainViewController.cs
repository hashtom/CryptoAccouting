using Foundation; using System; using UIKit; using CryptoAccouting.CoreClass; using CryptoAccouting.UIClass; using System.Threading.Tasks;  namespace CryptoAccouting {     public partial class BalanceMainViewController : BalanceTableViewController     {
        //private NavigationDrawer menu;          public BalanceMainViewController(IntPtr handle) : base(handle)         {             AppSetting.BaseCurrency = EnuCCY.JPY;
        }           public override void ViewDidLoad()         {             base.ViewDidLoad();              // Instantiate Controllers             AppSetting.balanceMainViewC = this;             AppSetting.transViewC = this.Storyboard.InstantiateViewController("TransactionViewC") as TransactionViewController;             AppSetting.plViewC = this.Storyboard.InstantiateViewController("PLViewC") as PLTableViewController;             AppSetting.perfViewC = this.Storyboard.InstantiateViewController("PerfViewC") as PerfomViewController;             AppSetting.settingViewC = this.Storyboard.InstantiateViewController("SettingTableViewC") as SettingTableViewController;             //menu = ApplicationCore.InitializeSlideMenu(TableView, this, transViewC, plViewC, perfViewC, settingViewC);
             if (ApplicationCore.InitializeCore() != EnuAppStatus.Success){                 this.PopUpWarning("some issue!!");             }              // Configure Segmented control             ConfigureSegmentButton();              // Configure Table source             TableView.RegisterNibForCellReuse(BalanceViewCell.Nib, "BalanceViewCell");             TableView.Source = new BalanceTableSource(ApplicationCore.Balance, this);
        }          public override void ViewWillAppear(bool animated)         {
            base.ViewWillAppear(animated);             RefreshBalanceTable();         }          public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);             ApplicationCore.Balance.SortPositionByHolding();
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

            };          }

        private async void RefreshBalanceTable()
        {             await ApplicationCore.FetchMarketDataFromBalance();
            TableView.ReloadData();
			labelTotalFiat.Text = "$" + String.Format("{0:n2}", ApplicationCore.Balance.LatestFiatValue());
			labelTotalBTC.Text = String.Format("{0:n2}", ApplicationCore.Balance.LatestBTCValue());

		} 
        partial void ButtonAddNew_Activated(UIBarButtonItem sender)
        {
            CreateNewPosition();
        }

        partial void ButtonRefresh_Activated(UIBarButtonItem sender)
		{
            RefreshBalanceTable();
		}          private void CreateNewPosition(){
            var SymbolSelectionViewC = Storyboard.InstantiateViewController("SymbolSelectionViewC") as SymbolSelectionViewConroller;
            NavigationController.PushViewController(SymbolSelectionViewC, true);          }      } }