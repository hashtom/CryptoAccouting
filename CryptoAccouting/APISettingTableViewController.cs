using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;
using CryptoAccouting.CoreClass.APIClass;
using System.Linq;
using System.Collections.Generic;

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
            //NavigationItem.HidesBackButton = true;
            buttonDone.Enabled = false;
            buttonFetchPosition.Enabled = false;
            textAPIKey.Enabled = false;
            textAPISecret.Enabled = false;


            buttonFetchPosition.TouchUpInside += async (object sender, EventArgs e) =>
            {
                List<Position> positions;

                if (thisExchange != null)
                {
                    positions = await ExchangeAPI.FetchPositionAsync(thisExchange);
                    if (positions != null) positions.ForEach(x => ApplicationCore.Balance.Attach(x));
                }
                else
                {
                    UIAlertController okAlertController = UIAlertController.Create("Warning", "Couldn't get positions from the exchange!", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);
                }

            };
        }

        partial void ButtonExchange_TouchUpInside(UIButton sender)
        {
			UIAlertController exchangeAlert = UIAlertController.Create("Exchange", "Choose Exchange", UIAlertControllerStyle.ActionSheet);
			exchangeAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

            foreach (var exc in ApplicationCore.PublicExchangeList.Where(x=>x.APIReady == true))
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
                                                                     buttonFetchPosition.Enabled = true;
                                                                     textAPIKey.Enabled = true;
                                                                     textAPISecret.Enabled = true;
                }
                                                            ));
			}
			this.PresentViewController(exchangeAlert, true, null);

        }

        private void SetExchange()
        {
            if (thisExchange is null)
            {
                UIAlertController okAlertController = UIAlertController.Create("Critical", "Critical Error!", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
            }
            else
            {
                thisExchange.Key = textAPIKey.Text;
                thisExchange.Secret = textAPISecret.Text;
                ApplicationCore.SaveAppSetting();
            }
        }

        partial void ButtonDone_Activated(UIBarButtonItem sender)
        {
            UIAlertController okCancelAlertController = UIAlertController.Create("Warning", "This App only uses API Keys to download for positions and transactions."
                                                                                 + "Please disable trading an withdrawal permissions to increase your security.", UIAlertControllerStyle.Alert);
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