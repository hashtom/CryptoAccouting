// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace CryptoAccouting
{
    [Register ("BalanceDetailViewConrtoller")]
    partial class BalanceDetailViewConrtoller
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem buttonAddNew { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton buttonPriceSource { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView DetailTopView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imageCoin { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelMarketValue { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelProfitLoss { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelTotalBookCost { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelTotalQty { get; set; }


        [Action ("ButtonAddNew_Activated:")]
        partial void ButtonAddNew_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (buttonAddNew != null) {
                buttonAddNew.Dispose ();
                buttonAddNew = null;
            }

            if (buttonPriceSource != null) {
                buttonPriceSource.Dispose ();
                buttonPriceSource = null;
            }

            if (DetailTopView != null) {
                DetailTopView.Dispose ();
                DetailTopView = null;
            }

            if (imageCoin != null) {
                imageCoin.Dispose ();
                imageCoin = null;
            }

            if (labelMarketValue != null) {
                labelMarketValue.Dispose ();
                labelMarketValue = null;
            }

            if (labelProfitLoss != null) {
                labelProfitLoss.Dispose ();
                labelProfitLoss = null;
            }

            if (labelTotalBookCost != null) {
                labelTotalBookCost.Dispose ();
                labelTotalBookCost = null;
            }

            if (labelTotalQty != null) {
                labelTotalQty.Dispose ();
                labelTotalQty = null;
            }
        }
    }
}