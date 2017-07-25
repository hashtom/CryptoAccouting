using System;
using Foundation;
using UIKit;
using CryptoAccouting.CoreClass;

namespace CryptoAccouting.UIClass
{
    public partial class ExchangeViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("ExchangeViewCell");
        public static readonly UINib Nib;

        static ExchangeViewCell()
        {
            Nib = UINib.FromName("ExchangeViewCell", NSBundle.MainBundle);
        }

        protected ExchangeViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

		public void UpdateCell(Position pos)
		{
			var logo = pos.Coin.LogoFileName;

            labelExchange.Text = pos.Coin.Symbol;
			//imageView.Image = logo == null ? null : UIImage.FromFile(logo);
			//labelSymbol.Text = pos.Coin.Symbol;
			//labelHoldings.Text = String.Format("{0:n0}", pos.Amount);
			//labelHoldingBTC.Text = String.Format("{0:n0}", pos.AmountBTC());
			//labelFiatValue.Text = String.Format("{0:n0}", pos.LatestFiatValue());
			//labelPct.Text = String.Format("{0:n2}", pos.Pct1d()) + " %";
			//if (pos.Coin.Symbol is "BTC")
			//{
			//	labelPrice.Text = String.Format("{0:n0}", pos.MarketPrice());
			//	labelHoldings.Text = "";
			//}
			//else
			//{
			//	labelPrice.Text = String.Format("{0:n6}", pos.MarketPriceBTC());
			//}

		}
    }
}
