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
        UIKit.UIBarButtonItem buttonDone { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem buttonEdit { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton buttonExchange { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imageCoin { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelBTCPrice { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelBTCRet { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCoinName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCoinSymbol { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelFiat1dRet { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelFiatPrice { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField textBalanceDate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField textBookPrice { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField textQuantity { get; set; }

        [Action ("ButtonDone_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonDone_Activated (UIKit.UIBarButtonItem sender);

        [Action ("ButtonEdit_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonEdit_Activated (UIKit.UIBarButtonItem sender);

        [Action ("ButtonExchange_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonExchange_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (buttonDone != null) {
                buttonDone.Dispose ();
                buttonDone = null;
            }

            if (buttonEdit != null) {
                buttonEdit.Dispose ();
                buttonEdit = null;
            }

            if (buttonExchange != null) {
                buttonExchange.Dispose ();
                buttonExchange = null;
            }

            if (imageCoin != null) {
                imageCoin.Dispose ();
                imageCoin = null;
            }

            if (labelBTCPrice != null) {
                labelBTCPrice.Dispose ();
                labelBTCPrice = null;
            }

            if (labelBTCRet != null) {
                labelBTCRet.Dispose ();
                labelBTCRet = null;
            }

            if (labelCoinName != null) {
                labelCoinName.Dispose ();
                labelCoinName = null;
            }

            if (labelCoinSymbol != null) {
                labelCoinSymbol.Dispose ();
                labelCoinSymbol = null;
            }

            if (labelFiat1dRet != null) {
                labelFiat1dRet.Dispose ();
                labelFiat1dRet = null;
            }

            if (labelFiatPrice != null) {
                labelFiatPrice.Dispose ();
                labelFiatPrice = null;
            }

            if (textBalanceDate != null) {
                textBalanceDate.Dispose ();
                textBalanceDate = null;
            }

            if (textBookPrice != null) {
                textBookPrice.Dispose ();
                textBookPrice = null;
            }

            if (textQuantity != null) {
                textQuantity.Dispose ();
                textQuantity = null;
            }
        }
    }
}