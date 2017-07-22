using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace CryptoAccouting.UIClass
{
    public partial class BalanceViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("BalanceViewCell");
        public static readonly UINib Nib;
		//UIImageView imageView;
		//UILabel codeLabel, amountLabel, priceLabel, pctLabel;

        static BalanceViewCell()
        {
            Nib = UINib.FromName("BalanceViewCell", NSBundle.MainBundle);
        }

        protected BalanceViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

		public void UpdateCell(string code, string amount, string price, string pct1d, UIImage image)
		{
			imageView.Image = image;
            labelSymbol.Text = code;
            labelHoldings.Text = amount;
            labelPrice.Text = price;
            labelPct.Text = pct1d;
		}

		//public override void LayoutSubviews()
		//{
		//	base.LayoutSubviews();
		//	//imageView.Frame = new CGRect(ContentView.Bounds.Width - 63, 5, 33, 33);
		//	imageView.Frame = new CGRect(5, 5, 33, 33);
		//	codeLabel.Frame = new CGRect(50, 10, 100, 25);
		//	amountLabel.Frame = new CGRect(100, 10, 100, 25);
		//	priceLabel.Frame = new CGRect(200, 10, 100, 25);
		//	pctLabel.Frame = new CGRect(300, 10, 100, 25);
		//}
    }
}
