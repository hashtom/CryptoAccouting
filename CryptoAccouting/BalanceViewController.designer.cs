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
        UIKit.UIBarButtonItem AddCoinButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView BalanceTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView BalanceTopView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCurrency { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelTotalAsset { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AddCoinButton != null) {
                AddCoinButton.Dispose ();
                AddCoinButton = null;
            }

            if (BalanceTableView != null) {
                BalanceTableView.Dispose ();
                BalanceTableView = null;
            }

            if (BalanceTopView != null) {
                BalanceTopView.Dispose ();
                BalanceTopView = null;
            }

            if (labelCurrency != null) {
                labelCurrency.Dispose ();
                labelCurrency = null;
            }

            if (labelTotalAsset != null) {
                labelTotalAsset.Dispose ();
                labelTotalAsset = null;
            }
        }
    }
}