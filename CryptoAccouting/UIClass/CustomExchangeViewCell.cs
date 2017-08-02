﻿using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace CryptoAccouting
{

	public class CustomExchangeViewCell : UITableViewCell
	{
		UILabel codeLabel, amountLabel, priceLabel, volumeLabel;
		UIImageView imageView;

		public CustomExchangeViewCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
		{
			SelectionStyle = UITableViewCellSelectionStyle.Gray;
			ContentView.BackgroundColor = UIColor.FromRGB(218, 255, 127);
			imageView = new UIImageView();

			codeLabel = new UILabel()
			{
				Font = UIFont.FromName("Cochin-BoldItalic", 22f),
				TextColor = UIColor.FromRGB(127, 51, 0),
				BackgroundColor = UIColor.Clear
			};

			amountLabel = new UILabel()
			{
				Font = UIFont.FromName("AmericanTypewriter", 12f),
				TextColor = UIColor.FromRGB(38, 127, 0),
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear
			};

			priceLabel = new UILabel()
			{
				Font = UIFont.FromName("AmericanTypewriter", 12f),
				TextColor = UIColor.FromRGB(38, 127, 0),
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear
			};

			volumeLabel = new UILabel()
			{
				Font = UIFont.FromName("AmericanTypewriter", 12f),
				TextColor = UIColor.FromRGB(38, 127, 0),
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear
			};

			ContentView.AddSubviews(new UIView[] { codeLabel, amountLabel, imageView });

		}
		public void UpdateCell(string code, string amount, string price, string volume, UIImage image)
		{
			imageView.Image = image;
			codeLabel.Text = code;
			amountLabel.Text = amount;
			priceLabel.Text = price;
			volumeLabel.Text = volume;
		}
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			imageView.Frame = new CGRect(ContentView.Bounds.Width - 63, 5, 33, 33);
			codeLabel.Frame = new CGRect(5, 4, ContentView.Bounds.Width - 63, 25);
			amountLabel.Frame = new CGRect(100, 18, 100, 20);
		}
	}

}