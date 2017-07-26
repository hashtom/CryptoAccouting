using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;
using System.Collections.Generic;
//using CoreGraphics;
//using System.Drawing;
using System.Linq;

namespace CryptoAccouting
{
	public partial class BalanceEditViewController : UITableViewController
	{
		public Position PositionDetail;
		public BalanceViewController AppDel { get; set; }
        string symbol;
        public ExchangeList exchangesListed;

		public BalanceEditViewController(IntPtr handle) : base(handle)
		{
		}

        public void SetPosition(BalanceViewController d, Position pos)
		{
			AppDel = d;
			PositionDetail = pos;
            exchangesListed = ApplicationCore.GetExchangesBySymbol(pos.Coin.Symbol);
		}

        public void CoinSelected(string symbol)
        {
            this.symbol = symbol;
            exchangesListed = ApplicationCore.GetExchangesBySymbol(symbol);
            this.NavigationItem.HidesBackButton = true;
        }

		public override void ViewDidLoad()
        {
			base.ViewDidLoad();

		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
            ReDrawScreen();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
		}

        public void ReDrawScreen(){

            if (PositionDetail is null) // new add
            {
                labelCoinSymbol.Text = symbol;
            }
            else
            {
                labelCoinSymbol.Text = PositionDetail.Coin.Symbol;
                var imagelogo = PositionDetail.Coin.LogoFileName;
                imageCoin.Image = imagelogo == null ? null : UIImage.FromFile(imagelogo);

                labelCurrentPrice.Text = PositionDetail.MarketPrice().ToString();
                textQuantity.Text = PositionDetail.Amount.ToString();
                textTradePrice.Text = PositionDetail.MarketPrice().ToString();
            }

            if (buttonExchange.TitleLabel.Text is null)
            {
                buttonExchange.SetTitle(exchangesListed.First().ExchangeName,UIControlState.Normal);
            }
        }

        private void CreatePosition(){

            if (PositionDetail is null) PositionDetail = new Position(ApplicationCore.GetInstrument(symbol));

            PositionDetail.Amount = double.Parse(textQuantity.Text);
            //PositionDetail.TradedExchange = (EnuExchangeType)int.Parse(buttonExchange.TitleLabel.Text);

        }

        partial void ButtonExchange_TouchUpInside(UIButton sender)
        {

            UIAlertController okAlertController = UIAlertController.Create("Exchange","Choose Exchange", UIAlertControllerStyle.ActionSheet);
            okAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

            foreach (var exc in exchangesListed)
            {
                okAlertController.AddAction(UIAlertAction.Create(exc.ExchangeName,
                                                                 UIAlertActionStyle.Default,
                                                                 (obj) =>
                                                                 {
                                                                     buttonExchange.SetTitle(exc.ExchangeType.ToString(), UIControlState.Normal);
                                                                 }
                                                                ));
            }
            this.PresentViewController(okAlertController, true, null);
		}

        partial void ButtonSave_Activated(UIBarButtonItem sender)
        {
            CreatePosition();
            AppDel.SaveItem(PositionDetail);
        }

    }

}