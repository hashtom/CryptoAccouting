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
    [Register ("BalanceEditViewController")]
    partial class BalanceEditViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem buttonCancel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton buttonExchange { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem buttonSave { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imageCoin { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCoinSymbol { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCurrentPrice { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField textQuantity { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField textTradePrice { get; set; }

        [Action ("ButtonExchange_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonExchange_TouchUpInside (UIKit.UIButton sender);

        [Action ("ButtonSave_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonSave_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (buttonCancel != null) {
                buttonCancel.Dispose ();
                buttonCancel = null;
            }

            if (buttonExchange != null) {
                buttonExchange.Dispose ();
                buttonExchange = null;
            }

            if (buttonSave != null) {
                buttonSave.Dispose ();
                buttonSave = null;
            }

            if (imageCoin != null) {
                imageCoin.Dispose ();
                imageCoin = null;
            }

            if (labelCoinSymbol != null) {
                labelCoinSymbol.Dispose ();
                labelCoinSymbol = null;
            }

            if (labelCurrentPrice != null) {
                labelCurrentPrice.Dispose ();
                labelCurrentPrice = null;
            }

            if (textQuantity != null) {
                textQuantity.Dispose ();
                textQuantity = null;
            }

            if (textTradePrice != null) {
                textTradePrice.Dispose ();
                textTradePrice = null;
            }
        }
    }
}