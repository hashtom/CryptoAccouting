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
        UIKit.UILabel LabelRealizedPL { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelSoldAmount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelSoldValue { get; set; }

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

            if (LabelRealizedPL != null) {
                LabelRealizedPL.Dispose ();
                LabelRealizedPL = null;
            }

            if (LabelSoldAmount != null) {
                LabelSoldAmount.Dispose ();
                LabelSoldAmount = null;
            }

            if (LabelSoldValue != null) {
                LabelSoldValue.Dispose ();
                LabelSoldValue = null;
            }

            if (TextPeriod != null) {
                TextPeriod.Dispose ();
                TextPeriod = null;
            }
        }
    }
}