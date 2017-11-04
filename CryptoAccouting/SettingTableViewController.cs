using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;
using System.Collections.Generic;
using CryptoAccouting.UIClass;

namespace CryptoAccouting
{
    public partial class SettingTableViewController : CryptoTableViewController
    {
        LoadingOverlay loadPop;

        public SettingTableViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ReDrawScreen();
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //base.RowSelected(tableView, indexPath);
            var cell = tableView.CellAt(indexPath);
            switch (cell.ReuseIdentifier)
            {
                case "CurrencyCell":
                    PushSelectionView();
                    break;
                case "RefreshCoinCell":
                    ReloadCoinData();
                    break;

                case "RemoveCacheCell":
                    UIAlertController okAlertController = UIAlertController.Create("Warning", "Are you sure you want to clear all cache data? API keys will also be removed.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, null));
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (obj) => RemoveCache()));
                    this.PresentViewController(okAlertController, true, null);
                    break;
                default:
                    break;
            }

            tableView.DeselectRow(indexPath, true);
        }

		private void PushSelectionView()
		{
			List<SelectionSearchItem> searchitems = new List<SelectionSearchItem>();
            foreach (var item in Enum.GetValues(typeof(EnuBaseFiatCCY)))
			{
				SelectionSearchItem searchitem = new SelectionSearchItem()
				{
                    SearchItem1 = item.ToString(),
					SearchItem2 = "",
					SortOrder = (int)item
				};
				searchitems.Add(searchitem);
			}

			var SymbolSelectionViewC = Storyboard.InstantiateViewController("SymbolSelectionViewC") as SymbolSelectionViewConroller;
			SymbolSelectionViewC.SelectionItems = searchitems;
			NavigationController.PushViewController(SymbolSelectionViewC, true);
		}

        private void RemoveCache()
        {
			if (AppCore.RemoveAllCache() is EnuAPIStatus.Success)
			{
				UIAlertController okAlertController = UIAlertController.Create("Success", "Cache cleared.", UIAlertControllerStyle.Alert);
				okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				this.PresentViewController(okAlertController, true, null);
                ReDrawScreen();
			}
			else
			{
				UIAlertController okAlertController = UIAlertController.Create("Critical Error", "Failed to remove cache data!", UIAlertControllerStyle.Alert);
				okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
				this.PresentViewController(okAlertController, true, null);
			}
        }

        private void ReloadCoinData()
        {
            try
            {
                AppCore.SyncLatestCoins();

                UIAlertController okAlertController = UIAlertController.Create("Success", "Successfully Updated.", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                //ApplicationCore.SaveInstrumentXML();

            }
            catch(Exception e)
            {
                UIAlertController okAlertController = UIAlertController.Create("Critical Error", "Unable to update coin data: " + e.Message, UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
            }
        }

        public async override void SetSearchSelectionItem(string searchitem1) //CryptoTableViewController ControllerToBack)
        {
            //base.SetSearchSelectionItem(searchitem1);
            EnuBaseFiatCCY baseccy;
            if (!Enum.TryParse(searchitem1, out baseccy)) baseccy = EnuBaseFiatCCY.USD;
            labelBaseCurrency.Text = baseccy.ToString();

            AppCore.BaseCurrency = baseccy;
            AppCore.SaveAppSetting();

            var bounds = SettingTableView.Bounds;
            loadPop = new LoadingOverlay(bounds);
            SettingTableView.Add(loadPop);
            await AppCore.LoadCrossRateAsync();
            await AppCore.FetchMarketDataFromBalanceAsync();
            loadPop.Hide();
        }

        public override void ReDrawScreen()
        {
            base.ReDrawScreen();
            labelBaseCurrency.Text = AppCore.BaseCurrency.ToString();
        }
        
    }
}