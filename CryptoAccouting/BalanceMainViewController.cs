using Foundation; using System; using UIKit; using CryptoAccouting.CoreClass; using CryptoAccouting.UIClass; using System.Threading.Tasks; using System.Linq;  namespace CryptoAccouting {     public partial class BalanceMainViewController : BalanceTableViewController     {
        //private NavigationDrawer menu;          public BalanceMainViewController(IntPtr handle) : base(handle)         {             AppSetting.BaseCurrency = EnuCCY.JPY;
        }           public override void ViewDidLoad()         {             base.ViewDidLoad();              // Instantiate Controllers             AppSetting.balanceMainViewC = this;             AppSetting.transViewC = this.Storyboard.InstantiateViewController("TransactionViewC") as TransactionViewController;             AppSetting.plViewC = this.Storyboard.InstantiateViewController("PLViewC") as PLTableViewController;             AppSetting.perfViewC = this.Storyboard.InstantiateViewController("PerfViewC") as PerfomViewController;             AppSetting.settingViewC = this.Storyboard.InstantiateViewController("SettingTableViewC") as SettingTableViewController;             //menu = ApplicationCore.InitializeSlideMenu(TableView, this, transViewC, plViewC, perfViewC, settingViewC);
             if (ApplicationCore.InitializeCore() != EnuAppStatus.Success){                 this.PopUpWarning("some issue!!");             }              // Configure Segmented control             ConfigureSegmentButton();              // Configure Table source             TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "CoinViewCell");             TableView.Source = new CoinTableSource(this);
        }          public async override void ViewWillAppear(bool animated)         {
            base.ViewWillAppear(animated);             await ApplicationCore.FetchMarketDataFromBalance();             RefreshBalanceTable();         }          public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);             ApplicationCore.Balance.SortPositionByHolding();
        }
         public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier == "PositionSegue")
            {
                var navctlr = segue.DestinationViewController as BalanceDetailViewConrtoller;
                if (navctlr != null)
                {
                    var source = TableView.Source as CoinTableSource;
                    var rowPath = TableView.IndexPathForSelectedRow;
                    var item = source.GetItem(rowPath.Row);
                    navctlr.SetSymbol(item.Coin.Symbol);
                }             }else if (segue.Identifier == "PositionEditSegue")             {
				var navctlr = segue.DestinationViewController as BalanceEditViewController;
				if (navctlr != null)
				{
                    var source = TableView.Source as BookingTableSource;
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
                        TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "CoinViewCell");
                        TableView.Source = new CoinTableSource(this);
                        break;
                    case 1:
                        TableView.RegisterNibForCellReuse(ExchangeViewCell.Nib, "ExchangeViewCell");
                        TableView.Source = new ExchangeTableSource(this);
                        break;
                    case 2:
                        TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "BookingViewCell");
                        TableView.Source = new BookingTableSource("ETH",
                                                                  ApplicationCore.Balance.positions.Where(x => x.Coin.Symbol == "ETH").ToList(),                                                                   this);
                        break;
                    default:
                        break;
                }
                RefreshBalanceTable();
            };          }

        private void RefreshBalanceTable()
        {
            TableView.ReloadData();
			labelTotalFiat.Text = "$" + String.Format("{0:n2}", ApplicationCore.Balance.LatestFiatValue());
            labelTotalBTC.Text = String.Format("{0:n2}", ApplicationCore.Balance.AmountBTC());
		} 
        partial void ButtonAddNew_Activated(UIBarButtonItem sender)
        {
            CreateNewPosition();
        }

        partial void ButtonRefresh_Activated(UIBarButtonItem sender)
		{
            RefreshBalanceTable();
		}          private void CreateNewPosition()
        {
            var SymbolSelectionViewC = Storyboard.InstantiateViewController("SymbolSelectionViewC") as SymbolSelectionViewConroller;
            NavigationController.PushViewController(SymbolSelectionViewC, true);         }      } }