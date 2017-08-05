using Foundation; using System; using UIKit; using CryptoAccouting.CoreClass; using CryptoAccouting.UIClass; using System.Collections.Generic; using System.Threading.Tasks; using System.Linq;  namespace CryptoAccouting {     public partial class BalanceMainViewController : CryptoTableViewController     {
        //private NavigationDrawer menu;          public BalanceMainViewController(IntPtr handle) : base(handle)         {             AppSetting.BaseCurrency = EnuCCY.JPY;
        }           public async override void ViewDidLoad()         {             base.ViewDidLoad();              // Instantiate Controllers             AppSetting.balanceMainViewC = this;             AppSetting.transViewC = this.Storyboard.InstantiateViewController("TransactionViewC") as TransactionViewController;             AppSetting.plViewC = this.Storyboard.InstantiateViewController("PLViewC") as PLTableViewController;             AppSetting.perfViewC = this.Storyboard.InstantiateViewController("PerfViewC") as PerfomViewController;             AppSetting.settingViewC = this.Storyboard.InstantiateViewController("SettingTableViewC") as SettingTableViewController;             //menu = ApplicationCore.InitializeSlideMenu(TableView, this, transViewC, plViewC, perfViewC, settingViewC);
             if (ApplicationCore.InitializeCore() != EnuAppStatus.Success){                 this.PopUpWarning("some issue!!");             }              // Configure Segmented control             ConfigureSegmentButton();              // Configure Table source             TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "CoinViewCell");             TableView.Source = new CoinTableSource(this);             await ApplicationCore.FetchMarketDataFromBalanceAsync();
        }          public override void ViewWillAppear(bool animated)         {
            base.ViewWillAppear(animated);             //await ApplicationCore.FetchMarketDataFromBalance();              if (ApplicationCore.Balance != null){
			labelTotalFiat.Text = "$" + String.Format("{0:n2}", ApplicationCore.Balance.LatestFiatValue());
			labelTotalBTC.Text = String.Format("{0:n2}", ApplicationCore.Balance.AmountBTC());                 ApplicationCore.Balance.SortPositionByHolding();
            }             TableView.ReloadData();
		}          public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
         public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier == "PositionDetailsSegue")
            {
                var navctlr = segue.DestinationViewController as BalanceDetailViewConrtoller;
                if (navctlr != null)
                {
                    var source = TableView.Source as CoinTableSource;
                    var rowPath = TableView.IndexPathForSelectedRow;
                    var item = source.GetItem(rowPath.Row);
                    navctlr.SetSymbol(item.Coin.Symbol);
                }              }else if (segue.Identifier == "ExchangeDetailsSegue")             {
				//var navctlr = segue.DestinationViewController as BalanceDetailViewConrtoller;
				//if (navctlr != null)
				//{
    //                var source = TableView.Source as ExchangeTableSource;
				//	var rowPath = TableView.IndexPathForSelectedRow;
				//	var item = source.GetItem(rowPath.Row);
				//	navctlr.SetPosition(item);
				//}             }
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
                        //TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "BookingTestViewCell");
                        //TableView.Source = new BookingTableSource("ETH",
                        //                                          ApplicationCore.Balance.positions.Where(x => x.Coin.Symbol == "ETH").ToList(),                         //                                          this);
                        break;
                    default:
                        break;
                }
                TableView.ReloadData();
            };          } 
        partial void ButtonAddNew_Activated(UIBarButtonItem sender)
        {
            PushSelectionView();
        }

        partial void ButtonRefresh_Activated(UIBarButtonItem sender)
		{
           //ApplicationCore.FetchMarketDataFromBalance();
		}          private void PushSelectionView()
        {
            List<SelectionSearchItem> searchitems = new List<SelectionSearchItem>();             foreach ( var item in ApplicationCore.GetInstrumentAll()){                 SelectionSearchItem searchitem = new SelectionSearchItem()
                {                     SearchItem1 = item.Symbol,                     SearchItem2 = item.Name,                     ImageFile = item.Id + ".png",                     SortOrder = item.rank                 };                 searchitems.Add(searchitem);             } 
			var SymbolSelectionViewC = Storyboard.InstantiateViewController("SymbolSelectionViewC") as SymbolSelectionViewConroller;
			SymbolSelectionViewC.SelectionItems = searchitems;             SymbolSelectionViewC.DestinationID = "BalanceEditViewC";             NavigationController.PushViewController(SymbolSelectionViewC, true);         }      } }