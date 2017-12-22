using Foundation; using System; using UIKit; using CoinBalance.CoreModel; using CoinBalance.UIModel; using System.Collections.Generic; using CoreGraphics; using CoreAnimation; using System.Threading.Tasks;  namespace CoinBalance {     public partial class BalanceMainViewController : CryptoTableViewController     {
        //private NavigationDrawer menu;
        Balance mybalance;         int position_count;          public BalanceMainViewController(IntPtr handle) : base(handle)         {             var lang = AppSetting.Lang;         }          public override void ViewDidLoad()         {             base.ViewDidLoad();              // Instantiate Controllers             AppSetting.balanceMainViewC = this;             AppSetting.transViewC = this.Storyboard.InstantiateViewController("TransactionViewC") as TransactionViewController;             AppSetting.plViewC = this.Storyboard.InstantiateViewController("PLViewC") as PLTableViewController;             AppSetting.settingViewC = this.Storyboard.InstantiateViewController("SettingTableViewC") as SettingTableViewController;             //menu = ApplicationCore.InitializeSlideMenu(TableView, this, transViewC, plViewC, perfViewC, settingViewC);              try             {
                AppCore.InitializeCore();
                this.mybalance = AppCore.Balance;                  // Configure Table source                 TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "CoinViewCell");                 TableView.Source = new CoinTableSource(mybalance, this);                 position_count = mybalance.BalanceByCoin.Count;

                //Color Design                 var gradient = new CAGradientLayer();                 gradient.Frame = this.BalanceTopView.Bounds;                 gradient.NeedsDisplayOnBoundsChange = true;                 gradient.MasksToBounds = true;                 gradient.Colors = new CGColor[] { UIColor.FromRGB(0, 126, 167).CGColor, UIColor.FromRGB(0, 168, 232).CGColor };                 //NavigationController.NavigationBar.Layer.InsertSublayer(gradient, 0);                 this.BalanceTopView.Layer.InsertSublayer(gradient, 0); 
    			// Configure Segmented control
    			//ConfigureSegmentButton();                  RefreshControl = new UIRefreshControl();                 RefreshControl.ValueChanged += async (sender, e) =>
                {
                    try                     {
                        await RefreshPriceAsync();                     }
                    catch (Exception ex)                     {                         if (!AppCore.IsInternetReachable())                         {                             this.PopUpWarning("Warning", "Unable to Connect Internet!", () => RefreshControl.EndRefreshing());                         }                         else                         {
                            this.PopUpWarning("Warning", "Unable to obtain latest prices. " + ex.Message, () => RefreshControl.EndRefreshing());                         }                     }                     finally                     {                         RefreshControl.EndRefreshing();                         ReDrawScreen();                     }                 };                  ReDrawScreen();                  if (!AppCore.IsInternetReachable())                 {                     this.PopUpWarning("Warning", "Unable to Connect Internet!");
                }
                else                 {
                    //run the last
                    Task.Run(async () => await AppCore.FetchCoinLogoTop100Async());                 }              }             catch (Exception e)             {
                this.PopUpWarning("Error", "Critical issue: " + e.GetType() + ": " + e.Message);                 this.mybalance = new Balance();             } 
        }          public async override void ViewWillAppear(bool animated)         {
            base.ViewWillAppear(animated);              if (position_count != mybalance.BalanceByCoin.Count)             {                 TableView.Source = new CoinTableSource(mybalance, this);                 position_count = mybalance.BalanceByCoin.Count;             }              ReDrawScreen(); //need this              var updatetime = AppCore.Balance.PriceDateTime.AddSeconds(60);             if (DateTime.Now > updatetime)             {
                try
                {
                    await AppCore.LoadCrossRateAsync();
                    await AppCore.FetchMarketDataFromBalanceAsync();
                    //await AppCore.FetchCoinLogoFromBalanceAsync();
                }
                catch (Exception e)
                {
                    //this.PopUpWarning("Unable to obtain latest prices: " + e.GetType() + ": " + e.Message);
                    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": ViewWillAppear: Cound't update price: " + e.GetType() + ": " + e.Message);
                }
                finally
                {
                    ReDrawScreen();
                }             }

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
         //private void ConfigureSegmentButton()
        //{

        //    SegmentBalance.ValueChanged += (sender, e) =>
        //    {
        //        var selectedSegmentId = (sender as UISegmentedControl).SelectedSegment;

        //        switch (selectedSegmentId)
        //        {
        //            case 0:
        //                TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "CoinViewCell");
        //                TableView.Source = new CoinTableSource(mybalance, this);
        //                break;
        //            case 1:
        //                TableView.RegisterClassForCellReuse(typeof(CoinStoreCell), "CoinStorageCell");         //                //TableView.RegisterNibForCellReuse(CoinStorageCell.Nib, "CoinStorageCell");
        //                TableView.Source = new CoinStoreTableSource(AppCore.CoinStorageList, this);
        //                break;
        //            default:
        //                break;
        //        }         //        ReDrawScreen();
        //    };          //}          public override void ReDrawScreen()         {
            if (AppCore.Balance != null)             {                 labelCcy.Text = AppCore.BaseCurrency.ToString();                 labelTotalFiat.Text = AppCore.NumberFormat(mybalance.LatestFiatValueBase());                 labelTotalBTC.Text = String.Format("{0:n2}", mybalance.AmountBTC());                 label1dPct.Text = AppCore.NumberFormat(mybalance.BaseRet1d(), true, false) + "%";                 label1dPct.TextColor = mybalance.BaseRet1d() >= 0 ? UIColor.FromRGB(247, 255, 247) : UIColor.FromRGB(128, 0, 0);                 labelLastUpdate.Text = mybalance.PriceDateTime !=                      DateTime.MinValue ? "Price Updated: " + mybalance.PriceDateTime.ToShortDateString() + " " + mybalance.PriceDateTime.ToShortTimeString()                     : "";                 TableView.ReloadData();
            }          } 
        partial void ButtonAddNew_Activated(UIBarButtonItem sender)
        {
            PushSelectionView();
        }          private void PushSelectionView()
        {
            List<SelectionSearchItem> searchitems = new List<SelectionSearchItem>();             foreach (var item in AppCore.InstrumentList)
            {                 SelectionSearchItem searchitem = new SelectionSearchItem()
                {                     SearchItem1 = item.Id,                     SearchItem2 = item.Symbol1,                     ImageFile = item.Id + ".png",                     SortOrder = item.rank                 };                 searchitems.Add(searchitem);             } 
			var SymbolSelectionViewC = Storyboard.InstantiateViewController("SymbolSelectionViewC") as SymbolSelectionViewConroller;
			SymbolSelectionViewC.SelectionItems = searchitems;             SymbolSelectionViewC.DestinationID = "BalanceEditViewC";             NavigationController.PushViewController(SymbolSelectionViewC, true);         }          async private Task RefreshPriceAsync()         {             try             {
                //await ApplicationCore.LoadCrossRateAsync();
                await AppCore.FetchMarketDataFromBalanceAsync();                 await AppCore.FetchCoinLogoFromBalanceAsync();             }             catch (Exception e)             {                 System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": RefreshPriceAsync: Cound't update price: " + e.GetType() + ": " + e.Message);                 throw;             }         }     } }