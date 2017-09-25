using System;
using System.IO;
using CoreGraphics;
using Foundation;
using UIKit;
using CryptoAccouting.CoreClass;

namespace CryptoAccouting.UIClass
{
    public partial class CoinViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("CoinViewCell");
        public static readonly UINib Nib;

        static CoinViewCell()
        {
            Nib = UINib.FromName("CoinViewCell", NSBundle.MainBundle);
        }

        protected CoinViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public void UpdateCell(Position pos, bool ShowBookingValue)
		{
            labelSymbol.Text = pos.Coin.Symbol1;

            if (pos.Coin.Symbol1 is "BTC")
            {
                labelPrice.Text = "$" + ApplicationCore.NumberFormat(pos.LatestPriceUSD());
                labelHolding.Text = "";
                labelHoldingBTC.Text = "฿" + ApplicationCore.NumberFormat(pos.LatestAmountBTC());
                labelMemo.Text = String.Format("{0:n2}", pos.LatestSourceRet1d()) + " %";
                labelMemo.TextColor = pos.LatestSourceRet1d() > 0 ? UIColor.Green : UIColor.Red;
            }
            else
            {
                labelPrice.Text = "฿" + ApplicationCore.NumberFormat(pos.LatestPriceBTC());
                labelHolding.Text = ApplicationCore.NumberFormat(pos.Amount);
                labelHoldingBTC.Text = "฿" + ApplicationCore.NumberFormat(pos.LatestAmountBTC());
                labelMemo.Text = String.Format("{0:n2}", pos.BTCRet1d()) + " %";
                labelMemo.TextColor = pos.BTCRet1d() > 0 ? UIColor.Green : UIColor.Red;
            }

            if (ShowBookingValue)
            {
                labelPrice.Text = ApplicationCore.NumberFormat(pos.BookPriceUSD);
                labelValue.Text = ApplicationCore.NumberFormat(pos.BookValue());
                labelMemo.Text = pos.BookedExchange == null ? "" : pos.BookedExchange.Name;
            }
            else
            {
                var logo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Images", pos.Coin.Id + ".png");
                ImageView.Image = logo == null ? null : UIImage.FromFile(logo);
                labelValue.Text = ApplicationCore.NumberFormat(pos.LatestFiatValueBase());
            }

		}

    }

}
