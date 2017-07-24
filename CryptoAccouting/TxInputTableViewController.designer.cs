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
    [Register ("TxInputTableViewController")]
    partial class TxInputTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelCoinName { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (labelCoinName != null) {
                labelCoinName.Dispose ();
                labelCoinName = null;
            }
        }
    }
}