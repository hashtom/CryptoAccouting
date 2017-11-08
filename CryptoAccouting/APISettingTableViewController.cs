using Foundation;
using System;
using UIKit;
using CoinBalance.CoreClass;
using CoinBalance.CoreClass.APIClass;
using CoinBalance.UIClass;
using System.Linq;
using System.Collections.Generic;

namespace CoinBalance
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

                    try
                    {
                        positions = await ExchangeAPI.FetchPositionAsync(thisExchange);

                        if (positions.Any())
                        {
                            try
                            {
                                AddBalance(positions);
                                this.PopUpWarning("Successfully Imported.");
                                //UIAlertController okAlertController = UIAlertController.Create("Success", "Successfully Imported.", UIAlertControllerStyle.Alert);
                                //okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                                //this.PresentViewController(okAlertController, true, null);
                            }
                            catch (Exception ex)
                            {
                                this.PopUpWarning("Couldn't import positoin: " + ex.Message);
                                //UIAlertController okAlertController = UIAlertController.Create("Critical", "Couldn't import positoin!", UIAlertControllerStyle.Alert);
                                //okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                                //this.PresentViewController(okAlertController, true, null);
                            }
                        }
                        else
                        {
                            PopUpWarning("There is no balance to get imported.");
                            //UIAlertController okAlertController = UIAlertController.Create("Success", "No balance to get imported.", UIAlertControllerStyle.Alert);
                            //okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                            //this.PresentViewController(okAlertController, true, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        PopUpWarning("Couldn't get positions from the exchange: " + ex.Message);

                    }
                    finally
                    {
                        loadPop.Hide();
                    }
                }
            };
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
                AppCore.SaveAppSetting();
            }
        }

        private void AddBalance(List<Position> positions)
        {

            AppCore.DetachPositionByExchange(thisExchange);
            //var storage = ApplicationCore.GetCoinStorage(thisExchange.Code, EnuCoinStorageType.Exchange);
            foreach (var pos in positions)
            {
                AppCore.AttachCoinStorage(thisExchange.Code, EnuCoinStorageType.Exchange, pos);
                //pos.AttachCoinStorage(storage);
                //storage.AttachPosition(pos);
                AppCore.AttachPosition(pos, false);
            }
            AppCore.RefreshBalance();

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