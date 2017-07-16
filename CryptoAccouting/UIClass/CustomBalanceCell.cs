using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace CryptoAccouting.UIClass
{

    public class CustomBalanceCell : UITableViewCell
    {
        UILabel codeLabel, amountLabel, priceLabel, pctLabel;
        UIImageView imageView;

        //public CustomBalanceCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId) // Old way
        public CustomBalanceCell(IntPtr p) : base(p)
        {
            //SelectionStyle = UITableViewCellSelectionStyle.Gray;
            //ContentView.BackgroundColor = UIColor.FromRGB(218, 255, 127);
            //this.UserInteractionEnabled = true; // test to Delete

            imageView = new UIImageView();

            codeLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 16f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Left,
                BackgroundColor = UIColor.Clear
            };

            amountLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Left,
                BackgroundColor = UIColor.Clear
            };

            priceLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Left,
                BackgroundColor = UIColor.Clear
            };

            pctLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Left,
                BackgroundColor = UIColor.Clear
            };

            ContentView.AddSubviews(new UIView[] { imageView, codeLabel, amountLabel, priceLabel, pctLabel });

        }

        public void UpdateCell(string code, string amount, string price, string volume, UIImage image)
        {
            imageView.Image = image;
            codeLabel.Text = code;
            amountLabel.Text = amount;
            priceLabel.Text = price;
            pctLabel.Text = volume;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            //imageView.Frame = new CGRect(ContentView.Bounds.Width - 63, 5, 33, 33);
            imageView.Frame = new CGRect(5, 5, 33, 33);
            codeLabel.Frame = new CGRect(50, 10, 100, 25);
            amountLabel.Frame = new CGRect(100, 10, 100, 25);
            priceLabel.Frame = new CGRect(200, 10, 100, 25);
            pctLabel.Frame = new CGRect(300, 10, 100, 25);
        }
    }

}