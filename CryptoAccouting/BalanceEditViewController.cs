using Foundation;
using System;
using System.IO;
using UIKit;
using CoinBalance.CoreClass;
using CoinBalance.UIClass;
using System.Linq;
//using CoreGraphics;
//using CoreAnimation;

namespace CoinBalance
{
    public partial class BalanceEditViewController : CryptoTableViewController
    {
        Position PositionDetail;
        ExchangeList exchangesListed;
        //CryptoTableViewController owner;

        bool editmode = true;
        Instrument thisCoin;
        DateTime thisBalanceDate;
        Exchange thisExchange;
        CoinStorage thisStorage;

        public BalanceEditViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            PrepareDatePicker();
            ReDrawScreen();
            InitializeUserInteractionStates();

            buttonExchange.TouchUpInside += (sender, e) =>
            {

                UIAlertController exchangeAlert = UIAlertController.Create("Exchange", "Choose Exchange", UIAlertControllerStyle.ActionSheet);
                exchangeAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

                foreach (var exc in exchangesListed)
                {
                    exchangeAlert.AddAction(UIAlertAction.Create(exc.Name, UIAlertActionStyle.Default,
                                                                     (obj) =>
                                                                     {
                                                                         buttonExchange.SetTitle(exc.Name, UIControlState.Normal);
                                                                         thisExchange = exc;
                                                                     }
                                                                ));
                }
                this.PresentViewController(exchangeAlert, true, null);
            };

            buttonWallet.TouchUpInside += (sender, e) =>
            {
                UIAlertController exchangeAlert = UIAlertController.Create("Storage", "Choose Storage", UIAlertControllerStyle.ActionSheet);
                exchangeAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

                foreach (var st in CoinStorageList.GetStorageListSelection())
                {
                    if (st.StorageType == EnuCoinStorageType.Exchange && (thisExchange == null || thisExchange.Code == "OTC")) continue;
                    exchangeAlert.AddAction(UIAlertAction.Create(st.Code,
                                                                         UIAlertActionStyle.Default,
                                                                         (obj) =>
                                                                         {
                                                                             //if (st.StorageType == EnuCoinStorageType.Exchange && (thisExchange == null || thisExchange.Code == "OTC"))
                                                                             //{
                                                                             //    UIAlertController okAlertController = UIAlertController.Create("Alert", "Please setup booked exchange in advance.", UIAlertControllerStyle.Alert);
                                                                             //    okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                                                                             //    this.PresentViewController(okAlertController, true, null);
                                                                             //}
                                                                             //else
                                                                             //{
                                                                                 buttonWallet.SetTitle(st.Code, UIControlState.Normal);
                                                                                 thisStorage = st;
                                                                             //}
                                                                         }
                                                                ));
                }
                this.PresentViewController(exchangeAlert, true, null);
            };

            switchWatchOnly.ValueChanged += (sender, e) => 
            {
                if (switchWatchOnly.On)
                {
                    if (AppCore.Balance.HasBalance(thisCoin))
                    {
                        this.PopUpWarning("Warning", "You have already got this coin in your balance. You can only add single Watch-only coin.");
                    }
                    else
                    {
                        InitializeUserInteractionStates();
                    }
                }
                else
                {
                    editmode = true;
                    InitializeUserInteractionStates();
                }
            };

            buttonDelete.TouchUpInside += (sender, e) => 
            {
                if (PositionDetail != null)
                {
                    UIAlertController okCancelAlertController = UIAlertController.Create("Warning", "Are you sure you want to delete this position?", UIAlertControllerStyle.Alert);
                    okCancelAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default,
                                                                           alert =>
                                                                           {
                                                                               AppCore.DetachPosition(PositionDetail);
                                                                               CellItemUpdated(Popto);
                                                                           }));
                    okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    this.PresentViewController(okCancelAlertController, true, null);
                }
            };
        }


        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!AppDelegate.IsInDesignerView)
            {
                try
                {
                    await AppCore.FetchMarketDataAsync(thisCoin);
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": ViewWillAppear: FetchMarketDataAsync: " + e.GetType() + ": " + e.Message);
                }
            }
        }

        private void PrepareDatePicker()
        {

            var modalPicker = new ModalPickerViewController(ModalPickerType.Date, "Select A Date", this)
            {
                HeaderBackgroundColor = UIColor.Gray,
                HeaderTextColor = UIColor.Blue,
                TransitioningDelegate = new ModalPickerTransitionDelegate(),
                ModalPresentationStyle = UIModalPresentationStyle.Custom
            };

            modalPicker.DatePicker.Mode = UIDatePickerMode.Date;

            modalPicker.OnModalPickerDismissed += (s, ea) =>
            {
                var dateFormatter = new NSDateFormatter()
                {
                    DateStyle = NSDateFormatterStyle.Medium
                };

                buttonTradeDate.SetTitle(dateFormatter.ToString(modalPicker.DatePicker.Date), UIControlState.Normal);
                thisBalanceDate = (DateTime)modalPicker.DatePicker.Date;
            };


            //Events description
            buttonTradeDate.TouchUpInside += (sender, e) => 
            {
                PresentViewController(modalPicker, true, null);
            };
        }

        private void InitializeUserInteractionStates()
        {
            if (switchWatchOnly.On)
            {
                buttonExchange.Enabled = false;
                buttonExchange.SetTitle("", UIControlState.Disabled);
                buttonTradeDate.Enabled = false;
                buttonTradeDate.SetTitle("", UIControlState.Disabled);
                buttonWallet.Enabled = false;
                buttonWallet.SetTitle("", UIControlState.Disabled);
                textQuantity.Enabled = false;
                textQuantity.Text = "";
                textQuantity.Placeholder = "Watch Only";
            }

            if (editmode)
            {
                buttonDone.Enabled = true;
                buttonEdit.Enabled = false;
                switchWatchOnly.Enabled = true;

                if (!switchWatchOnly.On)
                {
                    buttonExchange.Enabled = true;
                    buttonExchange.SetTitleColor(UIColor.Blue, UIControlState.Normal);
                    buttonTradeDate.Enabled = true;
                    buttonTradeDate.SetTitleColor(UIColor.Blue, UIControlState.Normal);
                    buttonWallet.Enabled = true;
                    buttonWallet.SetTitleColor(UIColor.Blue, UIControlState.Normal);
                    textQuantity.Enabled = true;
                    textQuantity.TextColor = UIColor.Blue;
                    textQuantity.Placeholder = "Input Quantity";
                }

                buttonDelete.Alpha = PositionDetail != null ? 1 : 0;
            }
            else
            {
                buttonDone.Enabled = false;
                buttonEdit.Enabled = true;
                switchWatchOnly.Enabled = false;
                buttonDelete.Alpha = 0;

                if (!switchWatchOnly.On)
                {
                    buttonExchange.Enabled = false;
                    buttonExchange.SetTitleColor(UIColor.Black, UIControlState.Normal);
                    buttonWallet.Enabled = false;
                    buttonWallet.SetTitleColor(UIColor.Black, UIControlState.Normal);
                    buttonTradeDate.Enabled = false;
                    buttonTradeDate.SetTitleColor(UIColor.Black, UIControlState.Normal);
                    textQuantity.Enabled = false;
                    textQuantity.TextColor = UIColor.Black;
                }
            }
        }

        public override void ReDrawScreen()
        {
            labelCoinSymbol.Text = thisCoin.Symbol1;
            labelCoinName.Text = thisCoin.Name;
            var logo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Images", thisCoin.Id + ".png");
            imageCoin.Image = logo == null ? null : UIImage.FromFile(logo);

            if (PositionDetail != null)
            {
                if (PositionDetail.WatchOnly)
                {
                    switchWatchOnly.SetState(true, false);
                    //WatchOnlyScreen();
                }
                else
                {
                    switchWatchOnly.SetState(false, false);
                    textQuantity.Text = (Math.Abs(PositionDetail.Amount) < 0.00000001) ? "" : String.Format("{0:n6}", PositionDetail.Amount);
                    thisBalanceDate = PositionDetail.BalanceDate;
                    buttonTradeDate.SetTitle(PositionDetail.BalanceDate.Date.ToShortDateString(), UIControlState.Normal);
                    var exname = thisExchange is null ? "Not Specified" : thisExchange.Name;
                    buttonExchange.SetTitle(exname, UIControlState.Normal);
                }
            }
            else
            {
                thisBalanceDate = DateTime.Now.Date;
                buttonTradeDate.SetTitle(thisBalanceDate.ToShortDateString(), UIControlState.Normal);
                buttonExchange.SetTitle("Not Specified", UIControlState.Normal);
            }

            buttonWallet.SetTitle(thisStorage.StorageType.ToString(), UIControlState.Normal);
        }

        private void CreatePosition()
        {
            if (PositionDetail is null) PositionDetail = new Position(thisCoin);

            if (switchWatchOnly.On)
            {
                PositionDetail.WatchOnly = true;
                PositionDetail.ClearAttributes();
            }
            else
            {
                PositionDetail.WatchOnly = false;
                PositionDetail.Amount = double.Parse(textQuantity.Text);
                PositionDetail.BalanceDate = thisBalanceDate;

                if (thisExchange != null) PositionDetail.BookedExchange = thisExchange;

                AppCore.AttachCoinStorage(thisStorage.Code, thisStorage.StorageType, PositionDetail);
                //var storage = ApplicationCore.GetCoinStorage(thisStorage.Code, thisStorage.StorageType);
                //PositionDetail.AttachCoinStorage(storage);
            }

            AppCore.AttachPosition(PositionDetail);
            //ApplicationCore.Balance.ReCalculate();
            AppCore.SaveMyBalanceXML();
        }

        partial void ButtonEdit_Activated(UIBarButtonItem sender)
        {
            editmode = true;
            InitializeUserInteractionStates();
        }

        partial void ButtonDone_Activated(UIBarButtonItem sender)
        {
            double amount;
            if ((textQuantity.Text != "" && double.TryParse(textQuantity.Text, out amount)) || switchWatchOnly.On)
            {
                CreatePosition();
                CellItemUpdated(Popto);

            }
            else
            {
                PopUpWarning("Warning", "Please input correct holding quantity.");
                //UIAlertController okAlertController = UIAlertController.Create("PositionDetail", "Please input correct holding quantity.", UIAlertControllerStyle.Alert);
                //okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                //this.PresentViewController(okAlertController, true, null);
            }
        }

        public void SetPosition(Position pos, EnuPopTo popto, bool editmode)
        {
            PositionDetail = pos;
            thisCoin = pos.Coin;
            exchangesListed = AppCore.GetExchangeListByInstrument(pos.Coin.Id);
            thisExchange = PositionDetail.BookedExchange;
            thisStorage = PositionDetail.CoinStorage != null ? PositionDetail.CoinStorage : CoinStorageList.GetStorageListSelection().First(x => x.StorageType == EnuCoinStorageType.TBA);
            this.editmode = editmode;
            this.Popto = popto;
        }


        public override void SetSearchSelectionItem(string searchitem1)
        {
            thisCoin = AppCore.InstrumentList.GetByInstrumentId(searchitem1);
            exchangesListed = AppCore.GetExchangeListByInstrument(searchitem1);
            thisStorage = CoinStorageList.GetStorageListSelection().First(x => x.StorageType == EnuCoinStorageType.TBA);
            editmode = true;
        }

    }

}