﻿using Foundation; using System; using UIKit; using CryptoAccouting.CoreClass; using CryptoAccouting.UIClass; using System.Collections.Generic; using System.Threading.Tasks; using System.Linq;  namespace CryptoAccouting {     public partial class BalanceMainViewController : CryptoTableViewController     {
        //private NavigationDrawer menu;
        Balance mybalance;          public BalanceMainViewController(IntPtr handle) : base(handle)         {
        }          public override void ViewDidLoad()         {             base.ViewDidLoad();              // Instantiate Controllers             AppSetting.balanceMainViewC = this;             AppSetting.transViewC = this.Storyboard.InstantiateViewController("TransactionViewC") as TransactionViewController;             AppSetting.plViewC = this.Storyboard.InstantiateViewController("PLViewC") as PLTableViewController;             AppSetting.settingViewC = this.Storyboard.InstantiateViewController("SettingTableViewC") as SettingTableViewController;             //menu = ApplicationCore.InitializeSlideMenu(TableView, this, transViewC, plViewC, perfViewC, settingViewC);              //BalanceTopView.               if (ApplicationCore.InitializeCore() != EnuAppStatus.Success)             {                 this.PopUpWarning("some issue!!");                 this.mybalance = new Balance();             }             else             {                 this.mybalance = ApplicationCore.Balance;             }              // Configure Segmented control             ConfigureSegmentButton();              // Configure Table source             TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "CoinViewCell");             TableView.Source = new CoinTableSource(mybalance, this);             //await ApplicationCore.LoadCoreDataAsync();             //await ApplicationCore.FetchMarketDataFromBalanceAsync();
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
                        TableView.Source = new CoinTableSource(mybalance, this);
                        break;
                    case 1:
                        TableView.RegisterNibForCellReuse(CoinStorageViewCell.Nib, "CoinStorageViewCell");
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
            if (ApplicationCore.Balance != null)             {                 labelCcy.Text = ApplicationCore.BaseCurrency.ToString();                 labelTotalFiat.Text = String.Format("{0:n0}", mybalance.LatestFiatValueBase());                 labelTotalBTC.Text = String.Format("{0:n2}", mybalance.AmountBTC());
                mybalance.BalanceByCoin.SortPositionByHolding();
            }         } 
        partial void ButtonAddNew_Activated(UIBarButtonItem sender)
        {
            PushSelectionView();
        }

        async partial void ButtonRefresh_Activated(UIBarButtonItem sender)
		{
            await ApplicationCore.FetchMarketDataFromBalanceAsync();             //mybalance.BalanceByCoin.SortPositionByHolding();             TableView.ReloadData();
		}          private void PushSelectionView()
        {
            List<SelectionSearchItem> searchitems = new List<SelectionSearchItem>();             foreach (var item in ApplicationCore.InstrumentList)
            {                 SelectionSearchItem searchitem = new SelectionSearchItem()
                {                     SearchItem1 = item.Symbol,                     SearchItem2 = item.Name,                     ImageFile = item.Id + ".png",                     SortOrder = item.rank                 };                 searchitems.Add(searchitem);             } 
			var SymbolSelectionViewC = Storyboard.InstantiateViewController("SymbolSelectionViewC") as SymbolSelectionViewConroller;
			SymbolSelectionViewC.SelectionItems = searchitems;             SymbolSelectionViewC.DestinationID = "BalanceEditViewC";             NavigationController.PushViewController(SymbolSelectionViewC, true);         }      } }