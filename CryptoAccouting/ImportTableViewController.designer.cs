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
    [Register ("ImportTableViewController")]
    partial class ImportTableViewController
    {
        [Outlet]
        UIKit.UIButton buttonExchange { get; set; }


        [Outlet]
        UIKit.UIButton buttonImport { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (buttonExchange != null) {
                buttonExchange.Dispose ();
                buttonExchange = null;
            }

            if (buttonImport != null) {
                buttonImport.Dispose ();
                buttonImport = null;
            }
        }
    }
}