using System;
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

        public void UpdateCell(Position pos)
		{
            var logo = pos.Coin == null ? null : pos.Coin.LogoFileName;
            imageView.Image = logo == null ? null : UIImage.FromFile(logo);

            labelSymbol.Text = pos.Coin.Symbol;
            labelHoldingBTC.Text = "฿" + String.Format("{0:n4}", pos.AmountBTC());
            labelValue.Text = "฿" + String.Format("{0:n2}", pos.AmountBTC());
            if (pos.Coin.Symbol is "BTC"){
                labelBTCRet.Text = String.Format("{0:n2}", pos.SourceRet1d()) + " %";
				labelPrice.Text = "$" + String.Format("{0:n2}", pos.LatestPrice());
                labelHolding.Text = "";
            }else{
                labelBTCRet.Text = String.Format("{0:n2}", pos.BTCRet1d()) + " %";
                labelPrice.Text = "฿" + String.Format("{0:n8}", pos.LatestPriceBTC());
                labelHolding.Text = String.Format("{0:n2}", pos.Amount);
            }

		}

    }

}
