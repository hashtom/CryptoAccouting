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

namespace CoinBalance
{
    [Register ("SettingTableViewController")]
    partial class SettingTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelBaseCurrency { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell rowBaseCurrency { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView SettingTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (labelBaseCurrency != null) {
                labelBaseCurrency.Dispose ();
                labelBaseCurrency = null;
            }

            if (rowBaseCurrency != null) {
                rowBaseCurrency.Dispose ();
                rowBaseCurrency = null;
            }

            if (SettingTableView != null) {
                SettingTableView.Dispose ();
                SettingTableView = null;
            }
        }
    }
}