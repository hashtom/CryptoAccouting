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
    [Register ("BalanceMainViewController")]
    partial class BalanceMainViewController
    {
        [Outlet]
        UIKit.UILabel label1dPct { get; set; }


        [Outlet]
        UIKit.UILabel label1dPctText { get; set; }


        [Outlet]
        UIKit.UILabel labelLastUpdate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView BalanceTopView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem buttonAddNew { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCcy { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelTotalBTC { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelTotalFiat { get; set; }


        [Action ("ButtonAddNew_Activated:")]
        partial void ButtonAddNew_Activated (UIKit.UIBarButtonItem sender);

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

            if (label1dPct != null) {
                label1dPct.Dispose ();
                label1dPct = null;
            }

            if (label1dPctText != null) {
                label1dPctText.Dispose ();
                label1dPctText = null;
            }

            if (labelCcy != null) {
                labelCcy.Dispose ();
                labelCcy = null;
            }

            if (labelLastUpdate != null) {
                labelLastUpdate.Dispose ();
                labelLastUpdate = null;
            }

            if (labelTotalBTC != null) {
                labelTotalBTC.Dispose ();
                labelTotalBTC = null;
            }

            if (labelTotalFiat != null) {
                labelTotalFiat.Dispose ();
                labelTotalFiat = null;
            }
        }
    }
}