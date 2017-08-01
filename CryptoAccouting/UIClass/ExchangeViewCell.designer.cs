// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace CryptoAccouting.UIClass
{
    [Register ("ExchangeViewCell")]
    partial class ExchangeViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelExchange { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelHoldingBTC { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelRet { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelValueK { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (labelExchange != null) {
                labelExchange.Dispose ();
                labelExchange = null;
            }

            if (labelHoldingBTC != null) {
                labelHoldingBTC.Dispose ();
                labelHoldingBTC = null;
            }

            if (labelRet != null) {
                labelRet.Dispose ();
                labelRet = null;
            }

            if (labelValueK != null) {
                labelValueK.Dispose ();
                labelValueK = null;
            }
        }
    }
}