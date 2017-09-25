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
    [Register ("CoinStorageCell")]
    partial class CoinStorageCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelHoldingBTC { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelRet { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelValueK { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (labelHoldingBTC != null) {
                labelHoldingBTC.Dispose ();
                labelHoldingBTC = null;
            }

            if (labelName != null) {
                labelName.Dispose ();
                labelName = null;
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