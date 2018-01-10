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

namespace CoinBalance
{
    [Register ("PLViewController")]
    partial class PLViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem barbuttonShare { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton buttonExchange { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton buttonYear { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCashBook { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCashPL { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCashTrade { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelMarginBook { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelMarginPL { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelMarginTrade { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView PLSpreadsheetView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView PLTopView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (barbuttonShare != null) {
                barbuttonShare.Dispose ();
                barbuttonShare = null;
            }

            if (buttonExchange != null) {
                buttonExchange.Dispose ();
                buttonExchange = null;
            }

            if (buttonYear != null) {
                buttonYear.Dispose ();
                buttonYear = null;
            }

            if (labelCashBook != null) {
                labelCashBook.Dispose ();
                labelCashBook = null;
            }

            if (labelCashPL != null) {
                labelCashPL.Dispose ();
                labelCashPL = null;
            }

            if (labelCashTrade != null) {
                labelCashTrade.Dispose ();
                labelCashTrade = null;
            }

            if (labelMarginBook != null) {
                labelMarginBook.Dispose ();
                labelMarginBook = null;
            }

            if (labelMarginPL != null) {
                labelMarginPL.Dispose ();
                labelMarginPL = null;
            }

            if (labelMarginTrade != null) {
                labelMarginTrade.Dispose ();
                labelMarginTrade = null;
            }

            if (PLSpreadsheetView != null) {
                PLSpreadsheetView.Dispose ();
                PLSpreadsheetView = null;
            }

            if (PLTopView != null) {
                PLTopView.Dispose ();
                PLTopView = null;
            }
        }
    }
}