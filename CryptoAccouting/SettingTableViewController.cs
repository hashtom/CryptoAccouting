using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;

namespace CryptoAccouting
{
    public partial class SettingTableViewController : UITableViewController
    {
        public SettingTableViewController (IntPtr handle) : base (handle)
        {
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
                case "RefreshCoinCell":
                    if (ApplicationCore.LoadInstruments(true) == EnuAppStatus.Success)
                    {
                        UIAlertController okAlertController = UIAlertController.Create("Update Coin List", "Successfully Updated", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                        ApplicationCore.SaveInstrumentXML();

                    }else{
						UIAlertController okAlertController = UIAlertController.Create("Update COin List", "ERROR!!", UIAlertControllerStyle.Alert);
						okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
						this.PresentViewController(okAlertController, true, null); 
                    }
                    tableView.DeselectRow(indexPath, true);
					break;
                default:
                    tableView.DeselectRow(indexPath, true);
                    break;
            }
        }
        
    }
}