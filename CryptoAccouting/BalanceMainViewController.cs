﻿using Foundation; using System; //using System.ComponentModel; using UIKit; using CryptoAccouting.CoreClass; using CryptoAccouting.UIClass; using System.Collections.Generic; using CoreGraphics; using CoreAnimation;  namespace CryptoAccouting {     public partial class BalanceMainViewController : CryptoTableViewController     {
        //private NavigationDrawer menu;
        Balance mybalance;         public CAGradientLayer gradient;          public BalanceMainViewController(IntPtr handle) : base(handle)         {         }          public async override void ViewDidLoad()         {             base.ViewDidLoad();              // Instantiate Controllers             AppSetting.balanceMainViewC = this;             //AppSetting.transViewC = this.Storyboard.InstantiateViewController("TransactionViewC") as TransactionViewController;             AppSetting.plViewC = this.Storyboard.InstantiateViewController("PLViewC") as PLTableViewController;             AppSetting.settingViewC = this.Storyboard.InstantiateViewController("SettingTableViewC") as SettingTableViewController;             //menu = ApplicationCore.InitializeSlideMenu(TableView, this, transViewC, plViewC, perfViewC, settingViewC);  
            if (ApplicationCore.InitializeCore() != EnuAPIStatus.Success)             {                 this.PopUpWarning("some issue!!");                 this.mybalance = new Balance();             }             else             {                 this.mybalance = ApplicationCore.Balance;             }              // Configure Table source             TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "CoinViewCell");             TableView.Source = new CoinTableSource(mybalance, this);

            if (!ApplicationCore.IsInternetReachable())
			{
				UIAlertController okAlertController = UIAlertController.Create("Warning", "Unable to Connect Internet!", UIAlertControllerStyle.Alert);
				okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				this.PresentViewController(okAlertController, true, null);
			}
             //Color Design             gradient = new CAGradientLayer();             gradient.Frame = BalanceTopView.Bounds;             gradient.NeedsDisplayOnBoundsChange = true;             gradient.MasksToBounds = true;             gradient.Colors = new CGColor[] { UIColor.FromRGB(178, 200, 198).CGColor, UIColor.FromRGB(242, 243, 242).CGColor };             BalanceTopView.Layer.InsertSublayer(gradient, 0); 
			// Configure Segmented control
			ConfigureSegmentButton(); 
			if (!AppDelegate.IsInDesignerView)
			{
				await ApplicationCore.FetchCoinLogoAsync();
			}
        }          public async override void ViewWillAppear(bool animated)         {
            base.ViewWillAppear(animated);

            if (!AppDelegate.IsInDesignerView)
            {
                await ApplicationCore.LoadCoreDataAsync();
                await ApplicationCore.FetchMarketDataFromBalanceAsync();
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
                        TableView.Source = new CoinStoreTableSource(mybalance.CoinStorageList, this);
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
            if (ApplicationCore.Balance != null)             {                 labelCcy.Text = ApplicationCore.BaseCurrency.ToString();                  //var digit = (int)Math.Log10(mybalance.LatestFiatValueBase()) + 1;                 labelTotalFiat.Text = ApplicationCore.NumberFormat(mybalance.LatestFiatValueBase());                 labelTotalBTC.Text = ApplicationCore.NumberFormat(mybalance.AmountBTC());                 label1dPctBTC.Text = String.Format("{0:n2}", mybalance.BaseRet1d()) + "%";                 label1dPctBTC.Text = mybalance.BaseRet1d() > 0 ? "+" + label1dPctBTC.Text : label1dPctBTC.Text;                 label1dPctBTC.TextColor = mybalance.BaseRet1d() > 0 ? UIColor.FromRGB(18,104,114) : UIColor.Red; 
                mybalance.BalanceByCoin.SortPositionByHolding();
            }         } 
        partial void ButtonAddNew_Activated(UIBarButtonItem sender)
        {
            PushSelectionView();
        }

        async partial void ButtonRefresh_Activated(UIBarButtonItem sender)
        {
            await ApplicationCore.FetchMarketDataFromBalanceAsync();
            //mybalance.BalanceByCoin.SortPositionByHolding();
            ReDrawScreen();
            TableView.ReloadData();
        }          private void PushSelectionView()
        {
            List<SelectionSearchItem> searchitems = new List<SelectionSearchItem>();             foreach (var item in ApplicationCore.InstrumentList)
            {                 SelectionSearchItem searchitem = new SelectionSearchItem()
                {                     SearchItem1 = item.Id,                     SearchItem2 = item.Symbol1,                     ImageFile = item.Id + ".png",                     SortOrder = item.rank                 };                 searchitems.Add(searchitem);             } 
			var SymbolSelectionViewC = Storyboard.InstantiateViewController("SymbolSelectionViewC") as SymbolSelectionViewConroller;
			SymbolSelectionViewC.SelectionItems = searchitems;             SymbolSelectionViewC.DestinationID = "BalanceEditViewC";             NavigationController.PushViewController(SymbolSelectionViewC, true);         }      } }