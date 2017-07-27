using System;
using CoreGraphics;
using Foundation;
using UIKit;
using CryptoAccouting.CoreClass;

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

        public void UpdateCell(Position pos)
		{
            var logo = pos.Coin.LogoFileName;

            imageView.Image = logo == null ? null : UIImage.FromFile(logo);
            labelSymbol.Text = pos.Coin.Symbol;
			labelHoldings.Text = String.Format("{0:n2}", pos.Amount);
            labelHoldingBTC.Text = "B" + String.Format("{0:n4}", pos.AmountBTC());
            labelFiatValue.Text = "$" + String.Format("{0:n0}", pos.LatestFiatValue());
			labelPct.Text = String.Format("{0:n2}", pos.Pct1d()) + " %";
            if (pos.Coin.Symbol is "BTC"){
				labelPrice.Text = "$" + String.Format("{0:n2}", pos.MarketPrice());
                labelHoldings.Text = "";
            }else{
                labelPrice.Text = String.Format("{0:n8}", pos.MarketPriceBTC());
            }

		}

    }

}
