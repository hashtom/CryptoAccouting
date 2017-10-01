using Foundation;
using System;
using UIKit;
using CoreGraphics;
using CryptoAccouting.CoreClass;

namespace CryptoAccouting.UIClass
{
    public class CoinBookingCell : UITableViewCell
    {
        UILabel labelSymbol, labelHolding, labelTD, labelExchange, labelStorage;
        bool showTD;
        float width;

        //public CoinBookingCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
        public CoinBookingCell(IntPtr handle) : base(handle)
        {
            SelectionStyle = UITableViewCellSelectionStyle.Gray;
			//ContentView.BackgroundColor = UIColor.FromRGB(218, 255, 127);

			showTD = (float)ContentView.Bounds.Width > 320 ? true : false;
			width = showTD ? (float)ContentView.Bounds.Width / 5 : (float)ContentView.Bounds.Width / 4;

            labelSymbol = new UILabel()
            {
                Font = UIFont.FromName("Cochin-BoldItalic", 16f),
                TextColor = UIColor.FromRGB(127, 51, 0),
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };

            labelHolding = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };

            labelTD = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextAlignment = UITextAlignment.Right,
                AdjustsFontSizeToFitWidth = true,
                BackgroundColor = UIColor.Clear
            };

            labelExchange = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };

            labelStorage = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };

            if (showTD)
            {
                ContentView.AddSubviews(new UIView[] { labelSymbol, labelHolding, labelTD, labelExchange, labelStorage });
            }
            else
            {
                ContentView.AddSubviews(new UIView[] { labelSymbol, labelHolding, labelExchange, labelStorage });
            }
        }

        public void UpdateCell(Position pos)
        {
            labelSymbol.Text = pos.Coin.Symbol1;
            labelHolding.Text = ApplicationCore.NumberFormat(pos.Amount);
            labelTD.Text = pos.BalanceDate.ToShortDateString();
            labelExchange.Text = pos.BookedExchange == null ? "N/A" : pos.BookedExchange.Name;
            labelStorage.Text = pos.CoinStorage == null ? "N/A" : pos.CoinStorage.StorageType.ToString();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            labelSymbol.Frame = new CGRect(0, 10, 90, 20);
            labelHolding.Frame = new CGRect(width, 10, 90, 20);
            if (showTD)
            {
                labelTD.Frame = new CGRect((width * 2) + 10, 10, 60, 20);
                labelExchange.Frame = new CGRect(width * 3, 10, 70, 20);
                labelStorage.Frame = new CGRect(width * 4, 10, 70, 20);
            }
            else
            {
                labelExchange.Frame = new CGRect(width * 2, 10, 70, 20);
                labelStorage.Frame = new CGRect(width * 3 - 10, 10, 70, 20);
            }
        }
    }

}
