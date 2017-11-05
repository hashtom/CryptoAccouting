// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace CoinBalance.UIClass
{
    [Register ("CoinViewCell")]
    partial class CoinViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelHolding { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelHoldingBTC { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelPrice { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelRet1d { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelSymbol { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelValue { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (imageView != null) {
                imageView.Dispose ();
                imageView = null;
            }

            if (labelHolding != null) {
                labelHolding.Dispose ();
                labelHolding = null;
            }

            if (labelHoldingBTC != null) {
                labelHoldingBTC.Dispose ();
                labelHoldingBTC = null;
            }

            if (labelPrice != null) {
                labelPrice.Dispose ();
                labelPrice = null;
            }

            if (labelRet1d != null) {
                labelRet1d.Dispose ();
                labelRet1d = null;
            }

            if (labelSymbol != null) {
                labelSymbol.Dispose ();
                labelSymbol = null;
            }

            if (labelValue != null) {
                labelValue.Dispose ();
                labelValue = null;
            }
        }
    }
}