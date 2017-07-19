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

            this.LabelTxsBuy.Text = myTradeList.TxCountBuy.ToString();
            this.LabelQtySell.Text = myTradeList.TxCountSell.ToString();
            this.LabelGrossQty.Text = (myTradeList.TxCountSell + myTradeList.TxCountBuy).ToString();

            this.LabelQtySell.Text = myTradeList.TotalQtySell.ToString();
            this.LabelQtySell2.Text = this.LabelQtySell.Text;
            this.LabelQtyBuy.Text = myTradeList.TotalQtyBuy.ToString();
            this.LabelGrossQty.Text = (myTradeList.TotalQtyBuy + myTradeList.TotalQtySell).ToString();

            this.LabelBuyValue.Text = Math.Round(myTradeList.TotalValueBuy, 0).ToString();
            this.LabelSellValue.Text = Math.Round(myTradeList.TotalValueSell, 0).ToString();
            this.LabelSellValue2.Text = this.LabelSellValue.Text;
            this.LabelTotalValue.Text = Math.Round(myTradeList.TotalValueBuy + myTradeList.TotalValueSell / 1000, 0).ToString();

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