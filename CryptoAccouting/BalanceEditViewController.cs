using Foundation;
using System;
using System.IO;
using UIKit;
using CryptoAccouting.CoreClass;
using CryptoAccouting.UIClass;
using System.Collections.Generic;
//using CoreGraphics;
//using System.Drawing;
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
            PrepareDatePicker();
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

                foreach (var st in ApplicationCore.GetStorageList())
                {
                    exchangeAlert.AddAction(UIAlertAction.Create(st.Code,
                                                                     UIAlertActionStyle.Default,
                                                                     (obj) =>
                                                                     {
                                                                         buttonWallet.SetTitle(st.Code, UIControlState.Normal);
                                                                         thisStorage = st;
                                                                     }
                                                                ));
                }
                this.PresentViewController(exchangeAlert, true, null);
            };
        }

        public async override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
            await ApplicationCore.FetchMarketDataAsync(thisCoin);
            ReDrawScreen();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
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

                textBalanceDate.Text = dateFormatter.ToString(modalPicker.DatePicker.Date);
                thisBalanceDate = (DateTime)modalPicker.DatePicker.Date;
            };


            //Events description
            textBalanceDate.EditingDidBegin += (sender, e) =>
            {
                PresentViewController(modalPicker, true, null);
            };
        }

        private void InitializeUserInteractionStates()
        {
            if (editmode)
            {
                buttonDone.Enabled = true;
                buttonEdit.Enabled = false;
                //buttonDone.SetTitleTextAttributes(new UITextAttributes() { TextColor = UIColor.Blue }, UIControlState.Normal);
                buttonExchange.SetTitleColor(UIColor.Blue, UIControlState.Normal);
                buttonExchange.UserInteractionEnabled = true;
                buttonWallet.SetTitleColor(UIColor.Blue, UIControlState.Normal);
				buttonWallet.UserInteractionEnabled = true;
                textQuantity.UserInteractionEnabled = true;
                textBookPrice.UserInteractionEnabled = true;
                textBalanceDate.UserInteractionEnabled = true;
                textQuantity.TextColor = UIColor.Blue;
                textBookPrice.TextColor = UIColor.Blue;
                textBalanceDate.TextColor = UIColor.Blue;
            }
            else
            {
                buttonDone.Enabled = false;
                buttonExchange.UserInteractionEnabled = false;
                //buttonDone.SetTitleTextAttributes(new UITextAttributes() { TextColor = UIColor.Black }, UIControlState.Disabled);
                buttonExchange.SetTitleColor(UIColor.Black, UIControlState.Normal);
				buttonWallet.UserInteractionEnabled = false;
                buttonWallet.SetTitleColor(UIColor.Black, UIControlState.Normal);
                textQuantity.UserInteractionEnabled = false;
                textBookPrice.UserInteractionEnabled = false;
                textBalanceDate.UserInteractionEnabled = false;
                textQuantity.TextColor = UIColor.Black;
                textBookPrice.TextColor = UIColor.Black;
                textBalanceDate.TextColor = UIColor.Black;
            }
        }

        public override void ReDrawScreen(){

            labelCoinSymbol.Text = thisCoin.Symbol;
            labelCoinName.Text = thisCoin.Name;

            if (thisCoin.MarketPrice != null)
            {
                labelBTCPrice.Text = thisCoin.Symbol == "BTC" ?
                    "" :
                    "฿" + String.Format("{0:n8}", thisCoin.MarketPrice.LatestPriceBTC);
                labelFiatPrice.Text = thisCoin.Symbol == "BTC" ?
                    "$" + String.Format("{0:n2}", thisCoin.MarketPrice.LatestPriceUSD) :
                    "$" + String.Format("{0:n6}", thisCoin.MarketPrice.LatestPriceBTC);
                labelFiat1dRet.Text = String.Format("{0:n2}", thisCoin.MarketPrice.SourceRet1d()) + "%";
                labelBTCRet.Text = String.Format("{0:n2}", thisCoin.MarketPrice.BTCRet1d()) + "%";
                //labelVolume.Text = String.Format("{0:n0}", thisCoin.MarketPrice.DayVolume);
                //labelMarketCap.Text = "$" + String.Format("{0:n0}", thisCoin.MarketPrice.MarketCap);
            }

            if (PositionDetail is null) // new balance
            {
                textBookPrice.Text = "0";
                thisBalanceDate = DateTime.Now.Date;
				textBalanceDate.Text = thisBalanceDate.ToShortDateString();
                //buttonExchange.SetTitle("Not Selected", UIControlState.Normal);
			}
            else
            {
                //labelCoinSymbol.Text = PositionDetail.Coin.Symbol;
				var logo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
									"Images", thisCoin.Id + ".png");
                imageCoin.Image = logo == null ? null : UIImage.FromFile(logo);

                //labelFiatPrice.Text = String.Format("{0:n0}", PositionDetail.LatestMainPrice());
                textQuantity.Text = String.Format("{0:n6}", PositionDetail.Amount);
                textBookPrice.Text = PositionDetail.BookPriceUSD < 0 ? String.Format("{0:n2}", PositionDetail.LatestPriceUSD()) : String.Format("{0:n2}", PositionDetail.BookPriceUSD);
                thisBalanceDate = PositionDetail.BalanceDate;
                textBalanceDate.Text = PositionDetail.BalanceDate.Date.ToShortDateString();
                var exname = PositionDetail.BookedExchange is null ?
                                           "Not Specified" :
                                           PositionDetail.BookedExchange.Code;
                buttonExchange.SetTitle(exname, UIControlState.Normal);

                var storagename = PositionDetail.CoinStorage is null ?
                                           "Not Specified" :
                                                PositionDetail.CoinStorage.Code;
                buttonWallet.SetTitle(storagename, UIControlState.Normal);
			}

        }

        private void CreatePosition()
        {
            if (PositionDetail is null) PositionDetail = new Position(thisCoin);

            PositionDetail.Amount = double.Parse(textQuantity.Text);
            PositionDetail.BookPriceUSD = textBookPrice.Text is "" ? 0 : double.Parse(textBookPrice.Text);
            PositionDetail.BookedExchange = thisExchange;
            PositionDetail.BalanceDate = thisBalanceDate;
            PositionDetail.AttachCoinStorage(thisStorage);

            ApplicationCore.Balance.Attach(PositionDetail);
            ApplicationCore.SaveMyBalanceXML();
        }

        partial void ButtonEdit_Activated(UIBarButtonItem sender)
        {
            editmode = true;
            InitializeUserInteractionStates();
        }


        partial void ButtonDone_Activated(UIBarButtonItem sender)
        {
            if (textQuantity.Text != "")
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
			exchangesListed = ApplicationCore.GetExchangesBySymbol(pos.Coin.Symbol);
			this.editmode = editmode;
            this.popto = popto;
		}


        public override void SetSearchSelectionItem(string searchitem1)
        {
            thisCoin = ApplicationCore.GetInstrument(searchitem1);
            exchangesListed = ApplicationCore.GetExchangesBySymbol(searchitem1);
            //thisExchange = exchangesListed.First();
            editmode = true;

            //this.owner = ControllerToBack == null ? null : ControllerToBack;
        }

    }

}