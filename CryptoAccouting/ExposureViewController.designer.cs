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
    [Register ("ExposureViewController")]
    partial class ExposureViewController
    {
        [Outlet]
        UIKit.UIView ExposureDrawingView { get; set; }


        [Outlet]
        UIKit.UIView ExposureGridView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ExposureDrawingView != null) {
                ExposureDrawingView.Dispose ();
                ExposureDrawingView = null;
            }

            if (ExposureGridView != null) {
                ExposureGridView.Dispose ();
                ExposureGridView = null;
            }
        }
    }
}