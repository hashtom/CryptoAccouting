using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;
using CryptoAccouting.UIClass;
using System.Collections.Generic;

namespace CryptoAccouting
{
    public partial class SettingTableViewController : CryptoTableViewController
    {
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
            labelBaseCurrency.Text = ApplicationCore.BaseCurrency.ToString();
        }

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			//ApplicationCore.SaveAppSetting();
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);

			if (segue.Identifier == "SettingDetailSegue")
			{
                var navctlr = segue.DestinationViewController as SettingTableViewController;
				if (navctlr != null)
				{

				}
			}
		}

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //base.RowSelected(tableView, indexPath);
            var cell = tableView.CellAt(indexPath);

            switch (cell.ReuseIdentifier)
            {
                case "CurrencyCell":
                    PushSelectionView();
                    tableView.DeselectRow(indexPath, true);
                    break;
                case "RefreshCoinCell":
                    ReloadCoinData();
                    tableView.DeselectRow(indexPath, true);
                    break;

                //case "APIKeyCell":
                    //PushAPISettingView();
                    //tableView.DeselectRow(indexPath, true);
                    //break;
                default:
                    tableView.DeselectRow(indexPath, true);
                    break;
            }
        }

		private void PushSelectionView()
		{
			List<SelectionSearchItem> searchitems = new List<SelectionSearchItem>();
            foreach (var item in Enum.GetValues(typeof(EnuCCY)))
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

        private void PushAPISettingView()
        {
            var APISettingViewC = Storyboard.InstantiateViewController("APISettingTableViewC") as APISettingTableViewController;
			NavigationController.PushViewController(APISettingViewC, true);
        }

        private void ReloadCoinData()
        {
            if (ApplicationCore.LoadInstruments(true) == EnuAppStatus.Success)
            {
                UIAlertController okAlertController = UIAlertController.Create("Update Coin List", "Successfully Updated", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
                ApplicationCore.SaveInstrumentXML();

            }
            else
            {
                UIAlertController okAlertController = UIAlertController.Create("Update COin List", "ERROR!!", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
            }
        }

        public override void SetSearchSelectionItem(string searchitem1, CryptoTableViewController ControllerToBack)
        {
            //base.SetSearchSelectionItem(searchitem1);
            EnuCCY baseccy;
            if (!Enum.TryParse(searchitem1, out baseccy)) baseccy = EnuCCY.USD;
            ApplicationCore.BaseCurrency = baseccy;
            labelBaseCurrency.Text = baseccy.ToString();
            ApplicationCore.SaveAppSetting();
        }
        
    }
}