using Foundation;
using System;
using UIKit;
using CoreGraphics;
using CryptoAccouting.CoreClass;

namespace CryptoAccouting.UIClass
{
    public class CoinBookingCell : UITableViewCell
    {

        UILabel labelSymbol, labelHolding, labelBook, labelExchange, labelStorage;

        //public CoinBookingCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
        public CoinBookingCell(IntPtr handle) : base(handle)
        {
            SelectionStyle = UITableViewCellSelectionStyle.Gray;
            ContentView.BackgroundColor = UIColor.FromRGB(218, 255, 127);

            labelSymbol = new UILabel()
            {
                Font = UIFont.FromName("Cochin-BoldItalic", 22f),
                TextColor = UIColor.FromRGB(127, 51, 0),
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };

            labelHolding = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextColor = UIColor.FromRGB(38, 127, 0),
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };

            labelBook = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextColor = UIColor.FromRGB(38, 127, 0),
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };

            labelExchange = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextColor = UIColor.FromRGB(38, 127, 0),
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };

            labelStorage = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextColor = UIColor.FromRGB(38, 127, 0),
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };

            ContentView.AddSubviews(new UIView[] { labelSymbol, labelHolding, labelBook, labelExchange, labelStorage });

        }

        public void UpdateCell(Position pos)
        {
            labelSymbol.Text = pos.Coin.Symbol1;
            labelHolding.Text = ApplicationCore.NumberFormat(pos.Amount);
            labelBook.Text = ApplicationCore.NumberFormat(pos.BookPriceUSD);
            labelExchange.Text = pos.BookedExchange == null ? "N/A" : pos.BookedExchange.Name;
            labelStorage.Text = pos.CoinStorage == null ? "N/A" : pos.CoinStorage.Name;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var width = (float)ContentView.Bounds.Width / 5;

            labelSymbol.Frame = new CGRect(20, 10, 50, 20);
            labelHolding.Frame = new CGRect(width, 10, 100, 20);
            labelBook.Frame = new CGRect(width * 2, 10, 100, 20);
            labelExchange.Frame = new CGRect(width * 3, 10, 60, 20);
            labelStorage.Frame = new CGRect(width * 4, 10, 60, 20);
        }
    }

}
