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
    [Register ("BalanceViewController")]
    partial class BalanceViewController
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
        UIKit.UIButton ButtonSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCurrency { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelTotalAsset { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelTotalAssetPct { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelViewTitle { get; set; }

        [Action ("ButtonAddNew_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonAddNew_Activated (UIKit.UIBarButtonItem sender);

        [Action ("ButtonRefresh_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonRefresh_Activated (UIKit.UIBarButtonItem sender);

        [Action ("ButtonSwitch_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonSwitch_TouchUpInside (UIKit.UIButton sender);

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

            if (ButtonSwitch != null) {
                ButtonSwitch.Dispose ();
                ButtonSwitch = null;
            }

            if (labelCurrency != null) {
                labelCurrency.Dispose ();
                labelCurrency = null;
            }

            if (labelTotalAsset != null) {
                labelTotalAsset.Dispose ();
                labelTotalAsset = null;
            }

            if (labelTotalAssetPct != null) {
                labelTotalAssetPct.Dispose ();
                labelTotalAssetPct = null;
            }

            if (labelViewTitle != null) {
                labelViewTitle.Dispose ();
                labelViewTitle = null;
            }
        }
    }
}