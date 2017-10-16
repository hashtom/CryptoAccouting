using Foundation;
using System;
using System.IO;
using UIKit;
using CryptoAccouting.CoreClass;
using CryptoAccouting.UIClass;
using System.Linq;
//using CoreGraphics;
//using CoreAnimation;

namespace CryptoAccouting
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

            //TableView.SectionFooterHeight = 0;

            //thisPriceSource = thisCoin.Symbol1 == "BTC" ? "Bitstamp" : "Bittrex";

            PrepareDatePicker();
            ReDrawScreen();
            InitializeUserInteractionStates();

            buttonExchange.TouchUpInside += (sender, e) =>
            {

                UIAlertController exchangeAlert = UIAlertController.Create("Exchange", "Choose Exchange", UIAlertControllerStyle.ActionSheet);
                exchangeAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

                foreach (var exc in exchangesListed)
                {
                    exchangeAlert.AddAction(UIAlertAction.Create(exc.Code, UIAlertActionStyle.Default,
                                                                     (obj) =>
                                                                     {
                                                                         buttonExchange.SetTitle(exc.Code, UIControlState.Normal);
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
                    if (ApplicationCore.Balance.HasBalance(thisCoin))
                    {
                        UIAlertController okAlertController = UIAlertController.Create("Warning", "You got this coin in your balance.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (obj) => switchWatchOnly.SetState(false, false)));
                        this.PresentViewController(okAlertController, true, null);
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
                                                                               ApplicationCore.DetachPosition(PositionDetail);
                                                                               CellItemUpdated(popto);
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
                await ApplicationCore.FetchMarketDataAsync(thisCoin);
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
                //textBalanceDate.Text = dateFormatter.ToString(modalPicker.DatePicker.Date);
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
                }
            }
            else
            {
                buttonDone.Enabled = false;
                buttonEdit.Enabled = true;
                switchWatchOnly.Enabled = false;

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

            if (PositionDetail != null)
            {
                buttonDelete.Enabled = true;
            }
            else
            {
                buttonDelete.Enabled = false;
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
                    var exname = thisExchange is null ? "Not Specified" : thisExchange.Code;
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
            }
            else
            {
                PositionDetail.WatchOnly = false;
                PositionDetail.Amount = double.Parse(textQuantity.Text);
                PositionDetail.BalanceDate = thisBalanceDate;

                if (thisExchange != null) PositionDetail.BookedExchange = thisExchange;

                ApplicationCore.AttachCoinStorage(thisStorage.Code, thisStorage.StorageType, PositionDetail);
                //var storage = ApplicationCore.GetCoinStorage(thisStorage.Code, thisStorage.StorageType);
                //PositionDetail.AttachCoinStorage(storage);
            }

            ApplicationCore.AttachPosition(PositionDetail);
            //ApplicationCore.Balance.ReCalculate();
            ApplicationCore.SaveMyBalanceXML();
        }

        partial void ButtonEdit_Activated(UIBarButtonItem sender)
        {
            editmode = true;
            InitializeUserInteractionStates();
        }

        partial void ButtonDone_Activated(UIBarButtonItem sender)
        {
            if (textQuantity.Text != "" || switchWatchOnly.On)
            {
                CreatePosition();
                CellItemUpdated(popto);

            }
            else
            {
                UIAlertController okAlertController = UIAlertController.Create("PositionDetail", "Please input at least holding quantity.", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                this.PresentViewController(okAlertController, true, null);
            }
        }

        public void SetPosition(Position pos, EnuPopTo popto, bool editmode)
        {
            PositionDetail = pos;
            thisCoin = pos.Coin;
            exchangesListed = ApplicationCore.GetExchangeListByInstrument(pos.Coin.Id);
            thisExchange = PositionDetail.BookedExchange;
            thisStorage = PositionDetail.CoinStorage != null ? PositionDetail.CoinStorage : CoinStorageList.GetStorageListSelection().First(x => x.StorageType == EnuCoinStorageType.TBA);
            this.editmode = editmode;
            this.popto = popto;
        }


        public override void SetSearchSelectionItem(string searchitem1)
        {
            thisCoin = ApplicationCore.InstrumentList.GetByInstrumentId(searchitem1);
            exchangesListed = ApplicationCore.GetExchangeListByInstrument(searchitem1);
            thisStorage = CoinStorageList.GetStorageListSelection().First(x => x.StorageType == EnuCoinStorageType.TBA);
            editmode = true;
        }

    }

}