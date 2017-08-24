using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;
using CryptoAccouting.UIClass;

namespace CryptoAccouting
{
    public abstract class CryptoTableViewController : UITableViewController
    {
		public EnuPopTo popto;

        public CryptoTableViewController(IntPtr handle) : base(handle)
        {
        }


		public void PopUpWarning(string message)
		{

			UIAlertController okAlertController = UIAlertController.Create("Warning", message, UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			this.PresentViewController(okAlertController, true, null);
		}

        //public virtual void DeleteItem(Position pos){}

        public void CellItemUpdated(EnuPopTo PopTo)
        {
            switch (PopTo)
            {
                case EnuPopTo.PopToRoot:
                    NavigationController.PopToRootViewController(true);
                    break;
                case EnuPopTo.OnePop:
                    NavigationController.PopViewController(true);
                    break;
                case EnuPopTo.None:
                    TableView.ReloadData();
                    break;
                default:
                    break;
            }
        }

        //public virtual void RefreshBalanceTable(){}

        public virtual void SetSearchSelectionItem(string searchitem1) //CryptoTableViewController ControllerToBack = null)
        {
        }

        public virtual void ReDrawScreen(){}

    }

    public enum EnuPopTo{
        PopToRoot,
        OnePop,
        None
    }
}
