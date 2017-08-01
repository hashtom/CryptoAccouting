using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;
using CryptoAccouting.UIClass;

namespace CryptoAccouting
{
    public abstract class BalanceTableViewController : UITableViewController
    {
        
        public BalanceTableViewController(IntPtr handle) : base(handle)
        {
        }


		public void PopUpWarning(string message)
		{

			UIAlertController okAlertController = UIAlertController.Create("Warning", message, UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			this.PresentViewController(okAlertController, true, null);
		}

        //public virtual void DeleteItem(Position pos){}

        public void CellItemUpdated()
        {
            TableView.ReloadData();
			NavigationController.PopToRootViewController(true);
        }

        //public virtual void RefreshBalanceTable(){}
    }
}
