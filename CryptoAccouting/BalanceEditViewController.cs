using Foundation;
using System;
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
		//BalanceViewController AppDel { get; set; }
        ExchangeList exchangesListed;

        bool editmode = true;
		Instrument thisCoin;
        DateTime thisBalanceDate;
        EnuExchangeType thisExchangeType;

		public BalanceEditViewController(IntPtr handle) : base(handle)
		{
		}

        public void SetPosition(Position pos)
		{
			//AppDel = d;
			PositionDetail = pos;
            thisCoin = pos.Coin;
			exchangesListed = ApplicationCore.GetExchangesBySymbol(pos.Coin.Symbol);
            editmode = false;
            //buttonCancel.Enabled = false;
            //this.NavigationItem.HidesBackButton = false;

		}

        public void NewCoinSelected(string symbol)
        {
            thisCoin = ApplicationCore.GetInstrument(symbol);
            exchangesListed = ApplicationCore.GetExchangesBySymbol(symbol);
            //this.NavigationItem.HidesBackButton = true;
            editmode = true;
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
            await ApplicationCore.RefreshMarketDataAsync(thisCoin);
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
                buttonExchange.SetTitleColor(UIColor.Black, UIControlState.Disabled);
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
                "B " + String.Format("{0:n8}", thisCoin.MarketPrice.LatestPriceBTC);
            labelFiatPrice.Text = thisCoin.Symbol == "BTC" ?
                "$" + String.Format("{0:n2}", thisCoin.MarketPrice.LatestMainPrice()) :
                "$" + String.Format("{0:n6}", thisCoin.MarketPrice.LatestMainPrice());
            labelFiat1dRet.Text = String.Format("{0:n2}", thisCoin.MarketPrice.FiatPct1d) + "%";
            //labelBTCRet.Text
            labelVolume.Text = String.Format("{0:n0}", thisCoin.MarketPrice.DayVolume);
            labelMarketCap.Text = "$" + String.Format("{0:n0}", thisCoin.MarketPrice.MarketCap);

            if (PositionDetail is null) // new balance
            {
				textBalanceDate.Text = DateTime.Now.Date.ToShortDateString();
			}
            else
            {
                //labelCoinSymbol.Text = PositionDetail.Coin.Symbol;
                var imagelogo = PositionDetail.Coin.LogoFileName;
                imageCoin.Image = imagelogo == null ? null : UIImage.FromFile(imagelogo);

                //labelFiatPrice.Text = String.Format("{0:n0}", PositionDetail.LatestMainPrice());
                textQuantity.Text = String.Format("{0:n0}", PositionDetail.Amount);
                textBookPrice.Text = PositionDetail.BookPrice < 0 ? String.Format("{0:n2}", PositionDetail.MarketPrice()) : String.Format("{0:n2}", PositionDetail.BookPrice);
				textBalanceDate.Text = PositionDetail.BalanceDate.Date.ToShortDateString();

			}

            if (buttonExchange.TitleLabel.Text is null)
            {
                buttonExchange.SetTitle(exchangesListed.First().ExchangeName,UIControlState.Normal);
            }
        }

        private void CreatePosition(){

            if (PositionDetail is null) PositionDetail = new Position(thisCoin);

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
    }

}