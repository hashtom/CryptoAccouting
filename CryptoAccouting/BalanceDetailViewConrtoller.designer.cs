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
    [Register ("BalanceDetailViewConrtoller")]
    partial class BalanceDetailViewConrtoller
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem buttonAddNew { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView DetailTopView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imageCoin { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelBTCPrice { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelBTCRet1d { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCoinName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelFiatPrice { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelFiatRet1d { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelMarketCap { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelMarketValue { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelTotalBookCost { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelTotalQty { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelVolume { get; set; }

        [Action ("ButtonAddNew_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonAddNew_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (buttonAddNew != null) {
                buttonAddNew.Dispose ();
                buttonAddNew = null;
            }

            if (DetailTopView != null) {
                DetailTopView.Dispose ();
                DetailTopView = null;
            }

            if (imageCoin != null) {
                imageCoin.Dispose ();
                imageCoin = null;
            }

            if (labelBTCPrice != null) {
                labelBTCPrice.Dispose ();
                labelBTCPrice = null;
            }

            if (labelBTCRet1d != null) {
                labelBTCRet1d.Dispose ();
                labelBTCRet1d = null;
            }

            if (labelCoinName != null) {
                labelCoinName.Dispose ();
                labelCoinName = null;
            }

            if (labelFiatPrice != null) {
                labelFiatPrice.Dispose ();
                labelFiatPrice = null;
            }

            if (labelFiatRet1d != null) {
                labelFiatRet1d.Dispose ();
                labelFiatRet1d = null;
            }

            if (labelMarketCap != null) {
                labelMarketCap.Dispose ();
                labelMarketCap = null;
            }

            if (labelMarketValue != null) {
                labelMarketValue.Dispose ();
                labelMarketValue = null;
            }

            if (labelTotalBookCost != null) {
                labelTotalBookCost.Dispose ();
                labelTotalBookCost = null;
            }

            if (labelTotalQty != null) {
                labelTotalQty.Dispose ();
                labelTotalQty = null;
            }

            if (labelVolume != null) {
                labelVolume.Dispose ();
                labelVolume = null;
            }
        }
    }
}