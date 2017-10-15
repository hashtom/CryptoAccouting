using Foundation; using System; using UIKit; using CryptoAccouting.CoreClass; using CryptoAccouting.UIClass; using System.Collections.Generic; using CoreGraphics; using CoreAnimation; using System.Threading.Tasks;  namespace CryptoAccouting {     public partial class BalanceMainViewController : CryptoTableViewController     {
        //private NavigationDrawer menu;
        Balance mybalance;         CAGradientLayer gradient;         LoadingOverlay loadPop;          public BalanceMainViewController(IntPtr handle) : base(handle)         {         }          public override void ViewDidLoad()         {             base.ViewDidLoad();              // Instantiate Controllers             AppSetting.balanceMainViewC = this;             //AppSetting.transViewC = this.Storyboard.InstantiateViewController("TransactionViewC") as TransactionViewController;             AppSetting.plViewC = this.Storyboard.InstantiateViewController("PLViewC") as PLTableViewController;             AppSetting.settingViewC = this.Storyboard.InstantiateViewController("SettingTableViewC") as SettingTableViewController;             //menu = ApplicationCore.InitializeSlideMenu(TableView, this, transViewC, plViewC, perfViewC, settingViewC);  
            if (ApplicationCore.InitializeCore() != EnuAPIStatus.Success)             {                 this.PopUpWarning("some issue!!");                 this.mybalance = new Balance();             }             else             {                 this.mybalance = ApplicationCore.Balance;             }              // Configure Table source             TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "CoinViewCell");             TableView.Source = new CoinTableSource(mybalance, this);
            ReDrawScreen(); 
            if (!ApplicationCore.IsInternetReachable())
			{
				UIAlertController okAlertController = UIAlertController.Create("Warning", "Unable to Connect Internet!", UIAlertControllerStyle.Alert);
				okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				this.PresentViewController(okAlertController, true, null);
			}
             //Color Design             gradient = new CAGradientLayer();             gradient.Frame = BalanceTopView.Bounds;             gradient.NeedsDisplayOnBoundsChange = true;             gradient.MasksToBounds = true;             gradient.Colors = new CGColor[] { UIColor.FromRGB(178, 200, 198).CGColor, UIColor.FromRGB(242, 243, 242).CGColor };             BalanceTopView.Layer.InsertSublayer(gradient, 0); 
			// Configure Segmented control
			ConfigureSegmentButton();              Task.Run(async () =>             {
                //if (!AppDelegate.IsInDesignerView)
                //{
                    await ApplicationCore.FetchCoinLogoAsync();
                //}             });              RefreshControl = new UIRefreshControl();             RefreshControl.ValueChanged += async (sender, e) =>              {                 await RefreshPriceAsync();                 RefreshControl.EndRefreshing();             };
        }          public async override void ViewWillAppear(bool animated)         {
            base.ViewWillAppear(animated);

            //Task.Run(async () =>
            //{
            if (!AppDelegate.IsInDesignerView)
            {
                if (await ApplicationCore.LoadCrossRateAsync() != EnuAPIStatus.Success)                 {                     //can't get fxrate even saved file                     UIAlertController okAlertController = UIAlertController.Create("critical", "Critical FX rates data error!", UIAlertControllerStyle.Alert);                     okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));                     this.PresentViewController(okAlertController, true, null);                 } 
                if(await ApplicationCore.FetchMarketDataFromBalanceAsync() != EnuAPIStatus.Success)                 {                     //Ignore Network error                     //UIAlertController okAlertController = UIAlertController.Create("Critical", "Critical Balance data error.", UIAlertControllerStyle.Alert);                     //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));                     //this.PresentViewController(okAlertController, true, null);                 }
            }

            ReDrawScreen();
            TableView.ReloadData();             
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
                        TableView.RegisterClassForCellReuse(typeof(CoinStoreCell), "CoinStorageCell");                         //TableView.RegisterNibForCellReuse(CoinStorageCell.Nib, "CoinStorageCell");
                        TableView.Source = new CoinStoreTableSource(ApplicationCore.CoinStorageList, this);
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
            if (ApplicationCore.Balance != null)             {                 labelCcy.Text = ApplicationCore.BaseCurrency.ToString();                 labelTotalFiat.Text = ApplicationCore.NumberFormat(mybalance.LatestFiatValueBase());                 labelTotalBTC.Text = ApplicationCore.NumberFormat(mybalance.AmountBTC());                 label1dPctBTC.Text = ApplicationCore.NumberFormat(mybalance.BaseRet1d(), true, false) + "%";                 label1dPctBTC.TextColor = mybalance.BaseRet1d() > 0 ? UIColor.FromRGB(18,104,114) : UIColor.Red;                 labelLastUpdate.Text = ApplicationCore.Bitcoin().MarketPrice != null ? "Last Update: " + ApplicationCore.Bitcoin().MarketPrice.PriceDate.ToShortTimeString() : "";
                //mybalance.SortPositionByHolding();
            }         } 
        partial void ButtonAddNew_Activated(UIBarButtonItem sender)
        {
            PushSelectionView();
        }

        async partial void ButtonRefresh_Activated(UIBarButtonItem sender)
        {             var bounds = TableView.Bounds;             loadPop = new LoadingOverlay(bounds);             TableView.Add(loadPop);              await RefreshPriceAsync();              loadPop.Hide();
        }          private void PushSelectionView()
        {
            List<SelectionSearchItem> searchitems = new List<SelectionSearchItem>();             foreach (var item in ApplicationCore.InstrumentList)
            {                 SelectionSearchItem searchitem = new SelectionSearchItem()
                {                     SearchItem1 = item.Id,                     SearchItem2 = item.Symbol1,                     ImageFile = item.Id + ".png",                     SortOrder = item.rank                 };                 searchitems.Add(searchitem);             } 
			var SymbolSelectionViewC = Storyboard.InstantiateViewController("SymbolSelectionViewC") as SymbolSelectionViewConroller;
			SymbolSelectionViewC.SelectionItems = searchitems;             SymbolSelectionViewC.DestinationID = "BalanceEditViewC";             NavigationController.PushViewController(SymbolSelectionViewC, true);         }          async private Task RefreshPriceAsync()         {             buttonRefresh.Enabled = false;              await ApplicationCore.FetchMarketDataFromBalanceAsync();             ReDrawScreen();             TableView.ReloadData();              buttonRefresh.Enabled = true;         }     } }