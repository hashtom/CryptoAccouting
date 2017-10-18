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

        public void UpdateCell(Position pos)
        {
            labelSymbol.Text = pos.Coin.Symbol1;

            if (pos.Coin.Symbol1 is "BTC")
            {
                labelPrice.Text = "$" + ApplicationCore.NumberFormat(pos.LatestPriceUSD);
                labelHolding.Text = "";
                labelHoldingBTC.Text = pos.WatchOnly ? "" : "฿" + ApplicationCore.NumberFormat(pos.LatestAmountBTC);
                labelRet1d.Text = ApplicationCore.NumberFormat(pos.BaseRet1d(), true, false) + " %";
                labelRet1d.TextColor = pos.BaseRet1d() > 0 ? UIColor.FromRGB(18, 104, 114) : UIColor.Red;
            }
            else
            {
                labelPrice.Text = "฿" + ApplicationCore.NumberFormat(pos.LatestPriceBTC());
                labelHolding.Text = pos.WatchOnly ? "" : ApplicationCore.NumberFormat(pos.Amount);
                labelHoldingBTC.Text = pos.WatchOnly ? "" : "฿" + ApplicationCore.NumberFormat(pos.LatestAmountBTC);
                labelRet1d.Text = ApplicationCore.NumberFormat(pos.BaseRet1d(), true, false) + " %";
                labelRet1d.TextColor = pos.BaseRet1d() > 0 ? UIColor.FromRGB(18, 104, 114) : UIColor.Red;
            }

            var logo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Images", pos.Coin.Id + ".png");
            ImageView.Image = logo == null ? null : UIImage.FromFile(logo);
            labelValue.Text = pos.WatchOnly ? "" : ApplicationCore.NumberFormat(pos.LatestFiatValueBase());
        }

    }

}
