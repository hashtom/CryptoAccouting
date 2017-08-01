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
    [Register ("BalanceMainViewController")]
    partial class BalanceMainViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView BalanceTopView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem buttonAddNew { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem buttonRefresh { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCurrency { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelTotalBTC { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelTotalFiat { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl SegmentBalance { get; set; }

        [Action ("ButtonAddNew_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonAddNew_Activated (UIKit.UIBarButtonItem sender);

        [Action ("ButtonRefresh_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonRefresh_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (BalanceTopView != null) {
                BalanceTopView.Dispose ();
                BalanceTopView = null;
            }

            if (buttonAddNew != null) {
                buttonAddNew.Dispose ();
                buttonAddNew = null;
            }

            if (buttonRefresh != null) {
                buttonRefresh.Dispose ();
                buttonRefresh = null;
            }

            if (labelCurrency != null) {
                labelCurrency.Dispose ();
                labelCurrency = null;
            }

            if (labelTotalBTC != null) {
                labelTotalBTC.Dispose ();
                labelTotalBTC = null;
            }

            if (labelTotalFiat != null) {
                labelTotalFiat.Dispose ();
                labelTotalFiat = null;
            }

            if (SegmentBalance != null) {
                SegmentBalance.Dispose ();
                SegmentBalance = null;
            }
        }
    }
}