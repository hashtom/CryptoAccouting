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
	public partial class BalanceEditViewController : UITableViewController
	{
		Position PositionDetail;
        ExchangeList exchangesListed;

        bool editmode = true;
		Instrument thisCoin;
        DateTime thisBalanceDate;
        EnuExchangeType thisExchangeType;

		public BalanceEditViewController(IntPtr handle) : base(handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            PrepareDatePicker();
			InitializeUserInteractionStates();
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
                textQuantity.UserInteractionEnabled = false;
                textBookPrice.UserInteractionEnabled = false;
                textBalanceDate.UserInteractionEnabled = false;
                textQuantity.TextColor = UIColor.Black;
                textBookPrice.TextColor = UIColor.Black;
                textBalanceDate.TextColor = UIColor.Black;
            }
        }

        private void ReDrawScreen(){

            labelCoinSymbol.Text = thisCoin.Symbol;
            labelCoinName.Text = thisCoin.Name;

            labelBTCPrice.Text = thisCoin.Symbol == "BTC" ?
                "" :
                "฿" + String.Format("{0:n8}", thisCoin.MarketPrice.LatestPriceBTC);
            labelFiatPrice.Text = thisCoin.Symbol == "BTC" ?
                "$" + String.Format("{0:n2}", thisCoin.MarketPrice.LatestPrice) :
                "$" + String.Format("{0:n6}", thisCoin.MarketPrice.LatestPriceBTC);
            labelFiat1dRet.Text = String.Format("{0:n2}", thisCoin.MarketPrice.SourceRet1d()) + "%";
            labelBTCRet.Text = String.Format("{0:n2}", thisCoin.MarketPrice.BTCRet1d()) + "%";
            //labelVolume.Text = String.Format("{0:n0}", thisCoin.MarketPrice.DayVolume);
            //labelMarketCap.Text = "$" + String.Format("{0:n0}", thisCoin.MarketPrice.MarketCap);

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
                textQuantity.Text = String.Format("{0:n0}", PositionDetail.Amount);
                textBookPrice.Text = PositionDetail.BookPrice < 0 ? String.Format("{0:n2}", PositionDetail.LatestPrice()) : String.Format("{0:n2}", PositionDetail.BookPrice);
                thisBalanceDate = PositionDetail.BalanceDate;
                textBalanceDate.Text = PositionDetail.BalanceDate.Date.ToShortDateString();
                var exname = PositionDetail.TradedExchange is 0 ?
                                           "Not Selected" :
                                           PositionDetail.TradedExchange.ToString();
                buttonExchange.SetTitle(exname, UIControlState.Normal);
			}

        }

        private void CreatePosition(){

            if (PositionDetail is null) PositionDetail = new Position(thisCoin,EnuPositionType.Detail);

            PositionDetail.Amount = double.Parse(textQuantity.Text);
            PositionDetail.BookPrice = textBookPrice.Text is "" ? 0 : double.Parse(textBookPrice.Text);
            PositionDetail.TradedExchange = thisExchangeType;
            PositionDetail.BalanceDate = thisBalanceDate;

            ApplicationCore.Balance.AttachPosition(PositionDetail);
			ApplicationCore.SaveMyBalanceXML();
        }

        partial void ButtonExchange_TouchUpInside(UIButton sender)
        {

            UIAlertController exchangeAlert = UIAlertController.Create("Exchange","Choose Exchange", UIAlertControllerStyle.ActionSheet);
            exchangeAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

            foreach (var exc in exchangesListed)
            {
                exchangeAlert.AddAction(UIAlertAction.Create(exc.ExchangeName,
                                                                 UIAlertActionStyle.Default,
                                                                 (obj) =>
                                                                 {
                                                                     buttonExchange.SetTitle(exc.ExchangeType.ToString(), UIControlState.Normal);
                                                                     thisExchangeType = exc.ExchangeType;
                                                                 }
                                                                ));
            }
            this.PresentViewController(exchangeAlert, true, null);
		}

        partial void ButtonEdit_Activated(UIBarButtonItem sender)
        {
            editmode = true;
            InitializeUserInteractionStates();
        }


        partial void ButtonDone_Activated(UIBarButtonItem sender)
        {
			CreatePosition();
            AppSetting.balanceMainViewC.CellItemUpdated();
        }

		public void SetPosition(Position pos)
		{
			PositionDetail = pos;
			thisCoin = pos.Coin;
			exchangesListed = ApplicationCore.GetExchangesBySymbol(pos.Coin.Symbol);
			editmode = false;
		}


        public void SetPositionForNewCoin(string symbol)
        {
            thisCoin = ApplicationCore.GetInstrument(symbol);
            exchangesListed = ApplicationCore.GetExchangesBySymbol(symbol);
            thisExchangeType = exchangesListed.First().ExchangeType;
            editmode = true;
        }
    }

}