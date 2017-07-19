using Foundation;
using System;
using UIKit;
using CryptoAccouting.UIClass;
using CryptoAccouting.CoreClass;
using System.Collections.Generic;
using CoreGraphics;

namespace CryptoAccouting
{
    public partial class PLTableViewController : UITableViewController
    {

        TradeList myTradeList;

        public PLTableViewController(IntPtr handle) : base(handle)
        {
        }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            myTradeList = await ApplicationCore.GetTradeListAsync(EnuExchangeType.Zaif, true, true);

			myTradeList.ReEvaluate();
            this.LabelSoldAmount.Text = Math.Round(myTradeList.TotalAmountSold, 0).ToString();
            //this.LabelSoldValue.Text = Math.Round(myTradeList., 0).ToString();
            this.LabelAvgBookPrice.Text = Math.Round(myTradeList.BookPrice, 0).ToString();
            //this.LabelAvgBookValue = Math.Round(myTradeList., 0).ToString();
            this.LabelRealizedPL.Text = Math.Round(myTradeList.RealizedPL()).ToString();
            //this.LabelEstTaxValue
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
		}

        partial void ButtonPickExchange_TouchUpInside(UIButton sender)
        {

        }

        partial void ButtonFiatCurrency_TouchUpInside(UIButton sender)
        {
            
        }

        partial void ButtonCryptCurrency_TouchUpInside(UIButton sender)
        {
           
        }
    }

}