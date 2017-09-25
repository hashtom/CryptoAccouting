using Foundation; using System; using UIKit; using CryptoAccouting.CoreClass; using CryptoAccouting.UIClass; using System.Collections.Generic; using System.Threading.Tasks; using System.Linq;  namespace CryptoAccouting {     public partial class BalanceMainViewController : CryptoTableViewController     {
        //private NavigationDrawer menu;
        Balance mybalance;          public BalanceMainViewController(IntPtr handle) : base(handle)         {
        }          public override void ViewDidLoad()         {             base.ViewDidLoad();              // Instantiate Controllers             AppSetting.balanceMainViewC = this;             //AppSetting.transViewC = this.Storyboard.InstantiateViewController("TransactionViewC") as TransactionViewController;             AppSetting.plViewC = this.Storyboard.InstantiateViewController("PLViewC") as PLTableViewController;             AppSetting.settingViewC = this.Storyboard.InstantiateViewController("SettingTableViewC") as SettingTableViewController;             //menu = ApplicationCore.InitializeSlideMenu(TableView, this, transViewC, plViewC, perfViewC, settingViewC);              //BalanceTopView.               if (ApplicationCore.InitializeCore() != EnuAPIStatus.Success)             {                 this.PopUpWarning("some issue!!");                 this.mybalance = new Balance();             }             else             {                 this.mybalance = ApplicationCore.Balance;             }              // Configure Segmented control             ConfigureSegmentButton();              // Configure Table source             TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "CoinViewCell");             TableView.Source = new CoinTableSource(mybalance, this);             //await ApplicationCore.LoadCoreDataAsync();             //await ApplicationCore.FetchMarketDataFromBalanceAsync();
        }          public async override void ViewWillAppear(bool animated)         {
            base.ViewWillAppear(animated);             await ApplicationCore.LoadCoreDataAsync();             await ApplicationCore.FetchMarketDataFromBalanceAsync();              ReDrawScreen();             TableView.ReloadData();
		}          public override void ViewDidAppear(bool animated)
        {
            //base.ViewDidAppear(animated);
        }          public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
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
                    navctlr.SetInstrument(item.Coin.Id);
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
                        TableView.Source = new CoinTableSource(mybalance, this);
                        break;
                    case 1:
                        TableView.RegisterNibForCellReuse(CoinStorageCell.Nib, "CoinStorageCell");
                        TableView.Source = new CoinStorageTableSource(mybalance.CoinStorageList, this);
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
            };          }          public override void ReDrawScreen()         {
            if (ApplicationCore.Balance != null)             {                 labelCcy.Text = ApplicationCore.BaseCurrency.ToString();                  //var digit = (int)Math.Log10(mybalance.LatestFiatValueBase()) + 1;                 labelTotalFiat.Text = ApplicationCore.NumberFormat(mybalance.LatestFiatValueBase());                 labelTotalBTC.Text = ApplicationCore.NumberFormat(mybalance.AmountBTC());                 label1dPctBTC.Text = String.Format("{0:n2}", mybalance.BaseRet1d()) + "%";                 label1dPctBTC.TextColor = mybalance.BaseRet1d() > 0 ? UIColor.Green : UIColor.Red;
                mybalance.BalanceByCoin.SortPositionByHolding();
            }         } 
        partial void ButtonAddNew_Activated(UIBarButtonItem sender)
        {
            PushSelectionView();
        }

        async partial void ButtonRefresh_Activated(UIBarButtonItem sender)
		{
            await ApplicationCore.FetchMarketDataFromBalanceAsync();             //mybalance.BalanceByCoin.SortPositionByHolding();             ReDrawScreen();             TableView.ReloadData();
		}          private void PushSelectionView()
        {
            List<SelectionSearchItem> searchitems = new List<SelectionSearchItem>();             foreach (var item in ApplicationCore.InstrumentList)
            {                 SelectionSearchItem searchitem = new SelectionSearchItem()
                {                     SearchItem1 = item.Id,                     SearchItem2 = item.Symbol1,                     ImageFile = item.Id + ".png",                     SortOrder = item.rank                 };                 searchitems.Add(searchitem);             } 
			var SymbolSelectionViewC = Storyboard.InstantiateViewController("SymbolSelectionViewC") as SymbolSelectionViewConroller;
			SymbolSelectionViewC.SelectionItems = searchitems;             SymbolSelectionViewC.DestinationID = "BalanceEditViewC";             NavigationController.PushViewController(SymbolSelectionViewC, true);         }      } }