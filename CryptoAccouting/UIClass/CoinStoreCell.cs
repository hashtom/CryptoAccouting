using Foundation;
using System;
using UIKit;
using CoreGraphics;
using CryptoAccouting.CoreClass;

namespace CryptoAccouting.UIClass
{
    public class CoinStoreCell: UITableViewCell
    {
        UILabel labelName, labelHoldingBTC, labelValueK, labelWeight;

        public CoinStoreCell(IntPtr handle) : base(handle)
        {
			SelectionStyle = UITableViewCellSelectionStyle.Gray;
			//ContentView.BackgroundColor = UIColor.FromRGB(218, 255, 127);

			labelName = new UILabel()
            {
                Font = UIFont.FromName("Cochin-BoldItalic", 16f),
                TextColor = UIColor.FromRGB(127, 51, 0),
                TextAlignment = UITextAlignment.Center,
                AdjustsFontSizeToFitWidth = true,
				BackgroundColor = UIColor.Clear
			};

			labelHoldingBTC = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 13f),
                TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear
			};

			labelValueK = new UILabel()
			{
				Font = UIFont.FromName("AmericanTypewriter", 13f),
                TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear
			};

            labelWeight = new UILabel()
			{
				Font = UIFont.FromName("AmericanTypewriter", 13f),
                TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear
			};

			ContentView.AddSubviews(new UIView[] { labelName, labelHoldingBTC, labelValueK, labelWeight});

		}

		public void UpdateCell(CoinStorage storage)
		{
			labelName.Text = storage.Name;
			labelHoldingBTC.Text = "฿" + ApplicationCore.NumberFormat(storage.AmountBTC());
			labelValueK.Text = ApplicationCore.NumberFormat(storage.LatestFiatValueBase());
			labelWeight.Text = String.Format("{0:n2}", storage.Weight * 100) + "%";
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			var width = (float)ContentView.Bounds.Width / 4;

			labelName.Frame = new CGRect(0, 10, 110, 20);
			labelHoldingBTC.Frame = new CGRect(width, 10, 100, 20);
			labelValueK.Frame = new CGRect(width * 2, 10, 100, 20);
			labelWeight.Frame = new CGRect(width * 3, 10, 60, 20);
		}
    }
}
