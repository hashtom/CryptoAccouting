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
    [Register ("SettingTableViewController")]
    partial class SettingTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView SettingTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (SettingTableView != null) {
                SettingTableView.Dispose ();
                SettingTableView = null;
            }
        }
    }
}