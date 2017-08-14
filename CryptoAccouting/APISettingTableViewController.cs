using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;
using CryptoAccouting.UIClass;
using System.Linq;

namespace CryptoAccouting
{
    public partial class APISettingTableViewController : CryptoTableViewController
    {

        Exchange thisExchange;

        public APISettingTableViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationItem.HidesBackButton = true;
        }

        partial void ButtonExchange_TouchUpInside(UIButton sender)
        {
			UIAlertController exchangeAlert = UIAlertController.Create("Exchange", "Choose Exchange", UIAlertControllerStyle.ActionSheet);
			exchangeAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

            foreach (var exc in ApplicationCore.ExchangeList.Where(x=>x.APIReady == true))
			{
                exchangeAlert.AddAction(UIAlertAction.Create(exc.ExchangeName,
                                                                 UIAlertActionStyle.Default,
                                                                 (obj) =>
                                                                 {
                                                                     buttonExchange.SetTitle(exc.ExchangeName, UIControlState.Normal);
                                                                     thisExchange = exc;
                                                                     textAPIKey.Text = exc.Key == null ? "" : exc.Key;
                                                                     textAPISecret.Text = exc.Secret == null ? "" : exc.Secret;
                }
                                                            ));
			}
			this.PresentViewController(exchangeAlert, true, null);

        }

        private void SetExchange()
        {
            thisExchange.Key = textAPIKey.Text;
            thisExchange.Secret = textAPISecret.Text;
            ApplicationCore.SaveAppSetting();
        }

        partial void ButtonCancel_Activated(UIBarButtonItem sender)
        {
            this.NavigationController.PopViewController(true);
        }

        partial void ButtonDone_Activated(UIBarButtonItem sender)
        {
			SetExchange();
			this.NavigationController.PopViewController(true);
        }
    }
}