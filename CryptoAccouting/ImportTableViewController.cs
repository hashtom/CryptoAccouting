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
    public partial class ImportTableViewController : CryptoTableViewController
    {
        Exchange thisExchange;
        LoadingOverlay loadPop;

        public ImportTableViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            buttonImport.Alpha= 0;


            buttonImport.TouchUpInside += async (sender, e) => 
            {
                List<Position> positions;

                if (thisExchange != null)
                {
                    buttonImport.Enabled = false;
                    var bounds = TableView.Bounds;
                    loadPop = new LoadingOverlay(bounds);
                    TableView.Add(loadPop);

                    try
                    {
                        positions = await ExchangeAPI.FetchPositionAsync(thisExchange);

                        if (positions.Any())
                        {
                            AddBalance(positions);
                            this.PopUpWarning("Import Position", "Successfully Imported.");
                        }
                        else
                        {
                            PopUpWarning("Warning", "There is no balance to get imported.");
                        }
                    }
                    catch (Exception ex)
                    {
                        PopUpWarning("Warning", "Couldn't get positions from the exchange: " + ex.Message);
                    }
                    finally
                    {
                        loadPop.Hide();
                        buttonImport.Enabled = true;
                    }
                }
            };

            buttonExchange.TouchUpInside += (sender, e) =>
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
                                                                         buttonImport.Alpha = 1;
                                                                     }
                                                                ));
                }
                this.PresentViewController(exchangeAlert, true, null);
            };
        }

        private void SetExchange()
        {
            if (thisExchange is null)
            {
                PopUpWarning("Error", "Critical error with Exchange Object.");
            }
            else
            {
                AppCore.SaveAppSetting();
            }
        }

        private void AddBalance(List<Position> positions)
        {

            //AppCore.DetachPositionByExchange(thisExchange, false);
            AppCore.AttachPositionByStorage(thisExchange, positions, true);
            //AppCore.RefreshBalance();

        }
    }
}