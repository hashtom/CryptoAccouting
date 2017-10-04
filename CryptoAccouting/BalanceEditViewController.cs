using Foundation;
using System;
using System.IO;
using UIKit;
using CryptoAccouting.CoreClass;
using CryptoAccouting.UIClass;
using System.Linq;

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

            TableView.SectionFooterHeight = 0;

            //thisPriceSource = thisCoin.Symbol1 == "BTC" ? "Bitstamp" : "Bittrex";

            PrepareDatePicker();
            InitializeUserInteractionStates();
            ReDrawScreen();

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
                    if (ApplicationCore.Balance.CoinContains(thisCoin))
                    {
                        UIAlertController okAlertController = UIAlertController.Create("Warning", "You got this coin in your balance.", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (obj) => switchWatchOnly.SetState(false, false)));
                        this.PresentViewController(okAlertController, true, null);
                    }
                    else
                    {
                        WatchOnlyScreen();
                    }
                }else
                {
                    editmode = true;
                    InitializeUserInteractionStates();
                    ReDrawScreen();
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
            if (editmode)
            {
                switchWatchOnly.Enabled = true;
                buttonDone.Enabled = true;
                buttonEdit.Enabled = false;
                buttonExchange.SetTitleColor(UIColor.Blue, UIControlState.Normal);
                buttonExchange.Enabled = true;
                buttonTradeDate.SetTitleColor(UIColor.Blue, UIControlState.Normal);
                buttonTradeDate.Enabled = true;
                buttonWallet.SetTitleColor(UIColor.Blue, UIControlState.Normal);
                buttonWallet.Enabled = true;
                textQuantity.Enabled = true;
                textQuantity.TextColor = UIColor.Blue;
            }
            else
            {
                switchWatchOnly.Enabled = false;
                buttonDone.Enabled = false;
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

        private void WatchOnlyScreen()
        {
            buttonExchange.Enabled = false;
            buttonExchange.SetTitle("",UIControlState.Disabled);
            buttonTradeDate.Enabled = false;
            buttonTradeDate.SetTitle("",UIControlState.Disabled);
            buttonWallet.Enabled = false;
            buttonWallet.SetTitle("", UIControlState.Disabled);
            textQuantity.Enabled = false;
            textQuantity.Text = "";
            textQuantity.Placeholder = "Watch Only";
        }

        public override void ReDrawScreen()
        {

            labelCoinSymbol.Text = thisCoin.Symbol1;
            labelCoinName.Text = thisCoin.Name;
            //buttonPriceSource.SetTitle(thisCoin.PriceSourceCode, UIControlState.Normal);

            if (thisCoin.MarketPrice != null)
            {
            }

            if (PositionDetail is null) // new balance
            {
                thisBalanceDate = DateTime.Now.Date;
                buttonTradeDate.SetTitle(thisBalanceDate.ToShortDateString(), UIControlState.Normal);
            }
            else
            {
                //labelCoinSymbol.Text = PositionDetail.Coin.Symbol;
                var logo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                    "Images", thisCoin.Id + ".png");
                imageCoin.Image = logo == null ? null : UIImage.FromFile(logo);

                //labelFiatPrice.Text = String.Format("{0:n0}", PositionDetail.LatestMainPrice());
                textQuantity.Text = (Math.Abs(PositionDetail.Amount) < 0.00000001) ? "" : String.Format("{0:n6}", PositionDetail.Amount);
                //textBookPrice.Text = (Math.Abs(PositionDetail.BookPriceUSD) < 0.00000001) ? String.Format("{0:n2}", PositionDetail.LatestPriceUSD()) : String.Format("{0:n2}", PositionDetail.BookPriceUSD);
                //textBookPrice.Text = String.Format("{0:n2}", PositionDetail.BookPriceUSD);
                thisBalanceDate = PositionDetail.BalanceDate;
                buttonTradeDate.SetTitle(PositionDetail.BalanceDate.Date.ToShortDateString(), UIControlState.Normal);
            }

            var exname = thisExchange is null ? "Not Specified" : PositionDetail.BookedExchange.Code;
            buttonExchange.SetTitle(exname, UIControlState.Normal);

            var storagename = thisStorage is null ? "Not Specified" : thisStorage.StorageType.ToString();
            buttonWallet.SetTitle(storagename, UIControlState.Normal);
        }

        private void CreatePosition()
        {
            if (PositionDetail is null) PositionDetail = new Position(thisCoin);

            PositionDetail.Amount = switchWatchOnly.On ? 0 : double.Parse(textQuantity.Text);
            //PositionDetail.BookPriceUSD = textBookPrice.Text is "" ? 0 : double.Parse(textBookPrice.Text);
            PositionDetail.BalanceDate = thisBalanceDate;

            if (thisExchange != null) PositionDetail.BookedExchange = thisExchange;

            //if (thisStorage != null)
            //{
                CoinStorage storage;
                if (thisStorage.StorageType == EnuCoinStorageType.Exchange)
                {
                    storage = thisExchange;
                }
                else
                {
                    storage = ApplicationCore.Balance.GetCoinStorage(thisStorage.Code, thisStorage.StorageType);
                }

                if (storage != null)
                {
                    PositionDetail.AttachCoinStorage(storage);
                }
                else
                {
                    PositionDetail.AttachNewStorage(thisStorage.Code, thisStorage.StorageType);
                }
            //}


            ApplicationCore.Balance.Attach(PositionDetail);
            ApplicationCore.Balance.ReCalculate();
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