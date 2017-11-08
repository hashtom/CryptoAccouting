using Foundation;
using System;
using UIKit;
using CoreGraphics;
using CoinBalance.CoreClass;

namespace CoinBalance.UIClass
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
                Font = UIFont.FromName("Apple SD Gothic Neo", 14f),
                TextColor = UIColor.FromRGB(0,0,128),
                TextAlignment = UITextAlignment.Center,
                AdjustsFontSizeToFitWidth = true,
                BackgroundColor = UIColor.Clear
            };

            labelHoldingBTC = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };

            labelValueK = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };

            labelWeight = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };

            ContentView.AddSubviews(new UIView[] { labelName, labelHoldingBTC, labelValueK, labelWeight});

        }

        public void UpdateCell(CoinStorage storage)
        {
            labelName.Text = storage.Name;
            labelHoldingBTC.Text = "฿" + AppCore.NumberFormat(storage.AmountBTC());
            labelValueK.Text = AppCore.NumberFormat(storage.LatestFiatValueBase());
            labelWeight.Text = String.Format("{0:n2}", storage.Weight * 100) + "%";
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var width = (float)ContentView.Bounds.Width / 4;

            labelName.Frame = new CGRect(5, 10, 95, 20);
            labelHoldingBTC.Frame = new CGRect(width, 10, 100, 20);
            labelValueK.Frame = new CGRect(width * 2, 10, 100, 20);
            labelWeight.Frame = new CGRect(width * 3, 10, 60, 20);
        }
    }
}
