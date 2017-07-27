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
		BalanceViewController AppDel { get; set; }
        ExchangeList exchangesListed;

		Instrument thisCoin;
        DateTime thisBalanceDate;
        EnuExchangeType thisExchangeType;

		public BalanceEditViewController(IntPtr handle) : base(handle)
		{
		}

        public void SetPosition(BalanceViewController d, Position pos)
		{
			AppDel = d;
			PositionDetail = pos;
            thisCoin = pos.Coin;
			exchangesListed = ApplicationCore.GetExchangesBySymbol(pos.Coin.Symbol);
		}

        public void CoinSelected(string symbol)
        {
            thisCoin = ApplicationCore.GetInstrument(symbol);
            exchangesListed = ApplicationCore.GetExchangesBySymbol(symbol);
            this.NavigationItem.HidesBackButton = true;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            PrepareDatePicker();
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
                HeaderBackgroundColor = UIColor.Red,
                HeaderTextColor = UIColor.White,
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

        public void ReDrawScreen(){

            if (PositionDetail is null) // new balance
            {
                labelCoinSymbol.Text = thisCoin.Symbol;
                labelCurrentPrice.Text = String.Format("{0:n0}", thisCoin.MarketPrice.LatestMainPrice());
                //textBookPrice.Text = PositionDetail.MarketPrice().ToString();
				textBalanceDate.Text = DateTime.Now.Date.ToShortDateString();

			}
            else
            {
                labelCoinSymbol.Text = PositionDetail.Coin.Symbol;
                var imagelogo = PositionDetail.Coin.LogoFileName;
                imageCoin.Image = imagelogo == null ? null : UIImage.FromFile(imagelogo);

                labelCurrentPrice.Text = String.Format("{0:n0}", PositionDetail.LatestMainPrice());
                textQuantity.Text = PositionDetail.Amount.ToString();
				textBookPrice.Text = PositionDetail.BookPrice < 0 ? textBookPrice.Text = PositionDetail.MarketPrice().ToString() : PositionDetail.BookPrice.ToString();
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
        }

        partial void ButtonExchange_TouchUpInside(UIButton sender)
        {

            UIAlertController exchangeAlert = UIAlertController.Create("Exchange","Choose Exchange", UIAlertControllerStyle.ActionSheet);
            exchangeAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

            //exchangeAlert.AddChildViewController();

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

        partial void ButtonSave_Activated(UIBarButtonItem sender)
        {
            CreatePosition();
            AppSetting.balanceViewC.SaveItem(PositionDetail);

        }

        partial void ButtonCancel_Activated(UIBarButtonItem sender)
        {
            NavigationController.PopToRootViewController(true);
        }
    }

}