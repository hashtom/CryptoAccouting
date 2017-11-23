using Foundation;
using System;
using UIKit;
using CoinBalance.UIClass;
using CoinBalance.CoreClass;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using System.Threading.Tasks;

namespace CoinBalance
{
    public partial class PLTableViewController : UITableViewController
    {
        TradeList myTradeList;

        public PLTableViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Task.Run(async () =>
            //{
                //if (!AppDelegate.IsInDesignerView)
                //{
                //await AppCore.LoadTradeListsAsync("Zaif");
                    //myTradeList = AppCore.GetExchangeTradeList("Zaif");
                    //myTradeList.CalculateTotalValue(DateTime.Now.Year);
                    DrawScreen();
                //}
            //});
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
		}

        partial void ButtonPickExchange_TouchUpInside(UIButton sender)
        {
            //this.myTradeList.SwitchTrdeYear(2016);
            DrawScreen();
        }

        partial void ButtonFiatCurrency_TouchUpInside(UIButton sender)
        {
			//this.myTradeList.SwitchTrdeYear(2015);
			DrawScreen();
        }

        partial void ButtonCryptCurrency_TouchUpInside(UIButton sender)
        {
			//this.myTradeList.SwitchTrdeYear(2017);
			DrawScreen();
        }

        private void DrawScreen()
        {
			//this.TextPeriod.Text = myTradeList.TradeYear.ToString();
			this.LabelTxsBuy.Text = String.Format("{0:n0}", myTradeList.NumOrdersBuy);
			this.LabelTxsSell.Text = String.Format("{0:n0}", myTradeList.NumOrdersSell);
			this.LabelTxsTotal.Text = String.Format("{0:n0}", myTradeList.NumOrdersBuy + myTradeList.NumOrdersSell);
			this.LabelQtySell.Text = String.Format("{0:n0}", myTradeList.NumOrdersSell);
			this.LabelGrossQty.Text = String.Format("{0:n0}", myTradeList.NumOrdersSell + myTradeList.NumOrdersBuy);

            this.LabelQtySell.Text = String.Format("{0:n}", myTradeList.TotalBTCTradeValueSell);
			this.LabelQtySell2.Text = this.LabelQtySell.Text;
			this.LabelQtyBuy.Text = String.Format("{0:n}", myTradeList.TotalBTCTradeValueBuy);
			this.LabelGrossQty.Text = String.Format("{0:n}", (myTradeList.TotalBTCTradeValueBuy + myTradeList.TotalBTCTradeValueSell));

			//this.LabelBuyValue.Text = String.Format("{0:n0}", myTradeList.TotalOtherTradeValueBuy);
			//this.LabelSellValue.Text = String.Format("{0:n0}", myTradeList.TotalOtherTradeValueSell);
			//this.LabelSellValue2.Text = String.Format("{0:n0}", myTradeList.TotalOtherTradeValueSell);
			//this.LabelTotalValue.Text = String.Format("{0:n0}", (myTradeList.TotalOtherTradeValueBuy + myTradeList.TotalOtherTradeValueSell));

            //this.LabelTradedCoins.Text = AppCore.GetExchangeTradeList("Zaif").TradedCoinString;
			//foreach (var coinname in ApplicationCore.GetExchange(EnuExchangeType.Zaif).TradeLists.Select(x => x.TradedCoin.Name))
			//{
			//	this.LabelTradedCoins.Text += coinname + " ";
			//}

			//this.LabelSoldValue.Text = Math.Round(myTradeList., 0).ToString();
            this.LabelRealizedCost.Text = String.Format("{0:n0}", myTradeList.RealizedBookValue);
			this.LabelRealizedPL.Text = String.Format("{0:n0}", myTradeList.RealizedPL());
			this.LabelAvgBookPrice.Text = String.Format("{0:n0}", myTradeList.AverageBookPrice);
			//this.LabelEstTaxValue
		}
    }

}