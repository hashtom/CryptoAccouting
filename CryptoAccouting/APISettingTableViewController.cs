using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;
using CryptoAccouting.CoreClass.APIClass;
using CryptoAccouting.UIClass;
using System.Linq;
using System.Collections.Generic;

namespace CryptoAccouting
{
    public partial class APISettingTableViewController : CryptoTableViewController
    {

        Exchange thisExchange;
        LoadingOverlay loadPop;

        public APISettingTableViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            buttonDone.Enabled = false;
            buttonFetchPosition.Enabled = false;
            textAPIKey.Enabled = false;
            textAPISecret.Enabled = false;


            buttonFetchPosition.TouchUpInside += async (object sender, EventArgs e) =>
            {
                List<Position> positions;

                if (thisExchange != null)
                {
                    var bounds = TableView.Bounds;
                    loadPop = new LoadingOverlay(bounds);
                    TableView.Add(loadPop);

                    positions = await ExchangeAPI.FetchPositionAsync(thisExchange);

                    loadPop.Hide();

                    if (positions != null)
                    {
                        if (positions.Count() > 0)
                        {
                            if (AddBalance(positions) is EnuAPIStatus.Success)
                            {
                                UIAlertController okAlertController = UIAlertController.Create("Success", "Successfully Imported.", UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);
                            }
                            else
                            {
                                UIAlertController okAlertController = UIAlertController.Create("Critical", "Couldn't import positoin!", UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);
                            }
                        }else
                        {
                            UIAlertController okAlertController = UIAlertController.Create("Success", "No balance to get imported.", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                    }
                    else
                    {
                        UIAlertController okAlertController = UIAlertController.Create("Critical", "Couldn't get positions from the exchange!", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                    }
                }

            };
        }

        partial void ButtonExchange_TouchUpInside(UIButton sender)
        {
			UIAlertController exchangeAlert = UIAlertController.Create("Exchange", "Choose Exchange", UIAlertControllerStyle.ActionSheet);
			exchangeAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

            foreach (var exc in ApplicationCore.PublicExchangeList.Where(x=>x.APIProvided == true))
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

        private EnuAPIStatus AddBalance(List<Position> positions)
        {
            //try
            //{
                ApplicationCore.DetachPositionByExchange(thisExchange);
                //var storage = ApplicationCore.GetCoinStorage(thisExchange.Code, EnuCoinStorageType.Exchange);
                foreach (var pos in positions)
                {
                    ApplicationCore.AttachCoinStorage(thisExchange.Code, EnuCoinStorageType.Exchange, pos);
                    //pos.AttachCoinStorage(storage);
                    //storage.AttachPosition(pos);
                    ApplicationCore.AttachPosition(pos, false);
                }
                ApplicationCore.RefreshBalance();

            //}catch(Exception)
            //{
            //    return EnuAPIStatus.FatalError;
            //}
            return EnuAPIStatus.Success;
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