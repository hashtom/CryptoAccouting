using Foundation;
using System;
using UIKit;
using CoinBalance.CoreClass;
using CoinBalance.UIClass;

namespace CoinBalance
{
    public abstract class CryptoTableViewController : UITableViewController
    {
		public EnuPopTo popto;

        public CryptoTableViewController(IntPtr handle) : base(handle)
        {
        }

        internal void PopUpWarning(string title, string message, Action lamda = null)
        {
            UIAlertController okAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
            PresentViewController(okAlertController, true, lamda);
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
