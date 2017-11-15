using Foundation;
using System;
using UIKit;
using CoinBalance.CoreClass;
using CoinBalance.UIClass;

namespace CoinBalance
{
    public abstract class CryptoViewController : UIViewController
    {
        
        public CryptoViewController(IntPtr handle) : base(handle)
        {
        }


        internal void PopUpWarning(string title, string message, Action lamda = null)
        {
            UIAlertController okAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
            PresentViewController(okAlertController, true, lamda);
        }

        public virtual void ReDrawScreen() { }

    }
}
