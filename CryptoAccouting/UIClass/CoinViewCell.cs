﻿using System;
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
            labelSymbol.Text = pos.Coin.Symbol;

            if (pos.Coin.Symbol is "BTC")
            {
                labelPrice.Text = "$" + String.Format("{0:n}", pos.LatestPriceUSD());
                labelHolding.Text = "";
                labelHoldingBTC.Text = "฿" + String.Format("{0:n4}", pos.LatestAmountBTC());
                labelMemo.Text = String.Format("{0:n2}", pos.LatestSourceRet1d()) + " %";
            }
            else
            {
                labelPrice.Text = "฿" + String.Format("{0:n8}", pos.LatestPriceBTC());
                labelHolding.Text = String.Format("{0:n2}", pos.Amount);
                labelHoldingBTC.Text = "฿" + String.Format("{0:n4}", pos.LatestAmountBTC());
                labelMemo.Text = String.Format("{0:n2}", pos.BTCRet1d()) + " %";
            }

            if (ShowBookingValue)
            {
                labelPrice.Text = String.Format("{0:n2}", pos.BookPriceUSD);
                labelValue.Text = String.Format("{0:n0}", pos.BookValue());
                labelMemo.Text = pos.BookedExchange == null ? "" : pos.BookedExchange.Name;
            }
            else
            {
                var logo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Images", pos.Coin.Id + ".png");
                ImageView.Image = logo == null ? null : UIImage.FromFile(logo);
                //imageView.ContentMode = UIViewContentMode.ScaleAspectFit;

                labelValue.Text = String.Format("{0:n2}", pos.LatestFiatValueBase());
            }

		}

    }

}
