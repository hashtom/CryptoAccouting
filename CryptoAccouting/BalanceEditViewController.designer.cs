// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace CoinBalance
{
    [Register ("BalanceEditViewController")]
    partial class BalanceEditViewController
    {
        [Outlet]
        UIKit.UIButton buttonDelete { get; set; }


        [Outlet]
        UIKit.UIButton buttonTradeDate { get; set; }


        [Outlet]
        UIKit.UIButton buttonWallet { get; set; }


        [Outlet]
        UIKit.UISwitch switchWatchOnly { get; set; }

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
        UIKit.UILabel labelCoinName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCoinSymbol { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField textQuantity { get; set; }


        [Action ("ButtonDone_Activated:")]
        partial void ButtonDone_Activated (UIKit.UIBarButtonItem sender);


        [Action ("ButtonEdit_Activated:")]
        partial void ButtonEdit_Activated (UIKit.UIBarButtonItem sender);


        [Action ("buttonExchange_TouchUpInside:")]
        partial void buttonExchange_TouchUpInside (UIKit.UIButton sender);


        [Action ("ButtonExchange_TouchUpInside:")]
        partial void ButtonExchange_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (buttonDelete != null) {
                buttonDelete.Dispose ();
                buttonDelete = null;
            }

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

            if (buttonTradeDate != null) {
                buttonTradeDate.Dispose ();
                buttonTradeDate = null;
            }

            if (buttonWallet != null) {
                buttonWallet.Dispose ();
                buttonWallet = null;
            }

            if (imageCoin != null) {
                imageCoin.Dispose ();
                imageCoin = null;
            }

            if (labelCoinName != null) {
                labelCoinName.Dispose ();
                labelCoinName = null;
            }

            if (labelCoinSymbol != null) {
                labelCoinSymbol.Dispose ();
                labelCoinSymbol = null;
            }

            if (switchWatchOnly != null) {
                switchWatchOnly.Dispose ();
                switchWatchOnly = null;
            }

            if (textQuantity != null) {
                textQuantity.Dispose ();
                textQuantity = null;
            }
        }
    }
}