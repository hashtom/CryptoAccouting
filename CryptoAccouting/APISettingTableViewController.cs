using Foundation;
using System;
using UIKit;
using CoinBalance.CoreModel;
using CoinBalance.CoreAPI;
using CoinBalance.UIModel;
using System.Linq;
using System.Collections.Generic;

namespace CoinBalance
{
    public partial class APISettingTableViewController : CryptoTableViewController
    {

        Exchange thisExchange;
        //LoadingOverlay loadPop;

        public APISettingTableViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            buttonDone.Enabled = false;
            //buttonFetchPosition.Enabled = false;
            textAPIKey.Enabled = false;
            textAPISecret.Enabled = false;
            labelCustomerID.Alpha = 0;
            textCustomerID.Alpha = 0;
        }

        partial void ButtonExchange_TouchUpInside(UIButton sender)
        {
            UIAlertController exchangeAlert = UIAlertController.Create("Exchange", "Choose Exchange", UIAlertControllerStyle.ActionSheet);
            exchangeAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

            foreach (var exc in AppCore.PublicExchangeList.Where(x => (x.HasTradeAPI == true || x.HasBalanceAPI)))
            {
                exchangeAlert.AddAction(UIAlertAction.Create(exc.Name,
                                                                 UIAlertActionStyle.Default,
                                                                 (obj) =>
                                                                 {
                                                                     buttonExchange.SetTitle(exc.Name, UIControlState.Normal);
                                                                     thisExchange = exc;
                                                                     textAPIKey.Text = exc.Key == null ? "" : exc.Key;
                                                                     textAPISecret.Text = exc.Secret == null ? "" : exc.Secret;
                                                                     buttonDone.Enabled = true;
                                                                     textAPIKey.Enabled = true;
                                                                     textAPISecret.Enabled = true;
                                                                     labelCustomerID.Alpha = exc.UseCustomerID() ? 1 : 0;
                                                                     textCustomerID.Alpha = exc.UseCustomerID() ? 1 : 0;
                                                                 }
                                                            ));
            }
            this.PresentViewController(exchangeAlert, true, null);

        }

        private void SetExchange()
        {
            if (thisExchange is null)
            {
                PopUpWarning("Error", "Critical error with Exchange Object.");
            }
            else
            {
                thisExchange.Key = textAPIKey.Text;
                thisExchange.Secret = textAPISecret.Text;
                thisExchange.CustomerID = textCustomerID.Text;
                AppCore.SaveAppSetting();
            }
        }

        private void AddBalance(List<Position> positions)
        {

            //AppCore.DetachPositionByExchange(thisExchange, false);
            AppCore.AttachPositionByStorage(thisExchange, positions, true);
            //AppCore.RefreshBalance();

        }

        partial void ButtonDone_Activated(UIBarButtonItem sender)
        {
            UIAlertController okCancelAlertController = UIAlertController.Create("Warning", "This App only uses API Keys to download positions and transactions."
                                                                                 + "It would be advised to disable your trading an withdrawal API permissions to increase your security.", UIAlertControllerStyle.Alert);
            okCancelAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default,
                                                                   alert =>
                                                                   {
                                                                       SetExchange();
                                                                       buttonDone.Enabled = false;
                                                                   }));
            okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            this.PresentViewController(okCancelAlertController, true, null);

			//this.NavigationController.PopViewController(true);
        }
    }
}