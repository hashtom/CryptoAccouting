// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace CoinBalance.Base.lproj
{
    [Register ("LeveragePLViewController")]
    partial class LeveragePLViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton buttonExchange { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView PositionPLView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (buttonExchange != null) {
                buttonExchange.Dispose ();
                buttonExchange = null;
            }

            if (PositionPLView != null) {
                PositionPLView.Dispose ();
                PositionPLView = null;
            }
        }
    }
}