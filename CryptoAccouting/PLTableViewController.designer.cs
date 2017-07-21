// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace CryptoAccouting
{
    [Register ("PLTableViewController")]
    partial class PLTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonCryptCurrency { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonFiatCurrency { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonPickExchange { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelAvgBookPrice { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelAvgBookValue { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelBuyValue { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelGrossQty { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelQtyBuy { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelQtySell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelQtySell2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelRealizedPL { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelSellValue { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelSellValue2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelTotalValue { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelTradedCoins { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelTxsBuy { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelTxsSell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelTxsTotal { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField TextPeriod { get; set; }

        [Action ("ButtonCryptCurrency_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonCryptCurrency_TouchUpInside (UIKit.UIButton sender);

        [Action ("ButtonFiatCurrency_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonFiatCurrency_TouchUpInside (UIKit.UIButton sender);

        [Action ("ButtonPickExchange_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonPickExchange_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ButtonCryptCurrency != null) {
                ButtonCryptCurrency.Dispose ();
                ButtonCryptCurrency = null;
            }

            if (ButtonFiatCurrency != null) {
                ButtonFiatCurrency.Dispose ();
                ButtonFiatCurrency = null;
            }

            if (ButtonPickExchange != null) {
                ButtonPickExchange.Dispose ();
                ButtonPickExchange = null;
            }

            if (LabelAvgBookPrice != null) {
                LabelAvgBookPrice.Dispose ();
                LabelAvgBookPrice = null;
            }

            if (LabelAvgBookValue != null) {
                LabelAvgBookValue.Dispose ();
                LabelAvgBookValue = null;
            }

            if (LabelBuyValue != null) {
                LabelBuyValue.Dispose ();
                LabelBuyValue = null;
            }

            if (LabelGrossQty != null) {
                LabelGrossQty.Dispose ();
                LabelGrossQty = null;
            }

            if (LabelQtyBuy != null) {
                LabelQtyBuy.Dispose ();
                LabelQtyBuy = null;
            }

            if (LabelQtySell != null) {
                LabelQtySell.Dispose ();
                LabelQtySell = null;
            }

            if (LabelQtySell2 != null) {
                LabelQtySell2.Dispose ();
                LabelQtySell2 = null;
            }

            if (LabelRealizedPL != null) {
                LabelRealizedPL.Dispose ();
                LabelRealizedPL = null;
            }

            if (LabelSellValue != null) {
                LabelSellValue.Dispose ();
                LabelSellValue = null;
            }

            if (LabelSellValue2 != null) {
                LabelSellValue2.Dispose ();
                LabelSellValue2 = null;
            }

            if (LabelTotalValue != null) {
                LabelTotalValue.Dispose ();
                LabelTotalValue = null;
            }

            if (LabelTradedCoins != null) {
                LabelTradedCoins.Dispose ();
                LabelTradedCoins = null;
            }

            if (LabelTxsBuy != null) {
                LabelTxsBuy.Dispose ();
                LabelTxsBuy = null;
            }

            if (LabelTxsSell != null) {
                LabelTxsSell.Dispose ();
                LabelTxsSell = null;
            }

            if (LabelTxsTotal != null) {
                LabelTxsTotal.Dispose ();
                LabelTxsTotal = null;
            }

            if (TextPeriod != null) {
                TextPeriod.Dispose ();
                TextPeriod = null;
            }
        }
    }
}