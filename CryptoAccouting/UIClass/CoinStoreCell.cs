using Foundation;
using System;
using UIKit;
using CoreGraphics;
using CryptoAccouting.CoreClass;

namespace CryptoAccouting.UIClass
{
    public class CoinStoreCell: UITableViewCell
    {
        UILabel labelName, labelHoldingBTC, labelValueK, labelRet;

        public CoinStoreCell(IntPtr handle) : base(handle)
        {
			SelectionStyle = UITableViewCellSelectionStyle.Gray;
			ContentView.BackgroundColor = UIColor.FromRGB(218, 255, 127);

			labelName = new UILabel()
			{
				Font = UIFont.FromName("Cochin-BoldItalic", 22f),
				TextColor = UIColor.FromRGB(127, 51, 0),
				BackgroundColor = UIColor.Clear
			};

			labelHoldingBTC = new UILabel()
			{
				Font = UIFont.FromName("AmericanTypewriter", 12f),
				TextColor = UIColor.FromRGB(38, 127, 0),
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear
			};

			labelValueK = new UILabel()
			{
				Font = UIFont.FromName("AmericanTypewriter", 12f),
				TextColor = UIColor.FromRGB(38, 127, 0),
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear
			};

			labelRet = new UILabel()
			{
				Font = UIFont.FromName("AmericanTypewriter", 12f),
				TextColor = UIColor.FromRGB(38, 127, 0),
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear
			};

			ContentView.AddSubviews(new UIView[] { labelName, labelHoldingBTC, labelValueK, labelRet});

		}

		public void UpdateCell(CoinStorage storage)
		{
			labelName.Text = storage.Name;
			labelHoldingBTC.Text = "฿" + ApplicationCore.NumberFormat(storage.AmountBTC());
			labelValueK.Text = ApplicationCore.NumberFormat(storage.LatestFiatValueBase());
			labelRet.Text = String.Format("{0:n2}", storage.Weight * 100) + "%";
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			var width = (float)ContentView.Bounds.Width / 4;

			labelName.Frame = new CGRect(20, 10, 50, 20);
			labelHoldingBTC.Frame = new CGRect(width, 10, 100, 20);
			labelValueK.Frame = new CGRect(width * 2, 10, 100, 20);
			labelRet.Frame = new CGRect(width * 3, 10, 60, 20);
		}
    }
}
