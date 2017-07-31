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
        UIKit.UIImageView imageCoin { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelBTCPct { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelBTCPrice { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelFiatPct { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelFiatPrice { get; set; }

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

        void ReleaseDesignerOutlets ()
        {
            if (imageCoin != null) {
                imageCoin.Dispose ();
                imageCoin = null;
            }

            if (labelBTCPct != null) {
                labelBTCPct.Dispose ();
                labelBTCPct = null;
            }

            if (labelBTCPrice != null) {
                labelBTCPrice.Dispose ();
                labelBTCPrice = null;
            }

            if (LabelFiatPct != null) {
                LabelFiatPct.Dispose ();
                LabelFiatPct = null;
            }

            if (labelFiatPrice != null) {
                labelFiatPrice.Dispose ();
                labelFiatPrice = null;
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