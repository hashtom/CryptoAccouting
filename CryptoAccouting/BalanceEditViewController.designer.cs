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
        UIKit.UIButton buttonExchange { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCoinSymbol { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField textExchange { get; set; }

        [Action ("ButtonExchange_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonExchange_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (buttonExchange != null) {
                buttonExchange.Dispose ();
                buttonExchange = null;
            }

            if (labelCoinSymbol != null) {
                labelCoinSymbol.Dispose ();
                labelCoinSymbol = null;
            }

            if (textExchange != null) {
                textExchange.Dispose ();
                textExchange = null;
            }
        }
    }
}