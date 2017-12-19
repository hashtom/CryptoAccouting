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
    [Register ("AboutViewConroller")]
    partial class AboutViewConroller
    {
        [Outlet]
        UIKit.UIWebView CreditWebView { get; set; }


        [Outlet]
        UIKit.UILabel labelVersion { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CreditWebView != null) {
                CreditWebView.Dispose ();
                CreditWebView = null;
            }

            if (labelVersion != null) {
                labelVersion.Dispose ();
                labelVersion = null;
            }
        }
    }
}