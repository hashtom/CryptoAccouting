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
    [Register ("APISettingTableViewController")]
    partial class APISettingTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem buttonCancel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem buttonDone { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton buttonExchange { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField textAPIKey { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField textAPISecret { get; set; }

        [Action ("ButtonCancel_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonCancel_Activated (UIKit.UIBarButtonItem sender);

        [Action ("ButtonDone_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonDone_Activated (UIKit.UIBarButtonItem sender);

        [Action ("ButtonExchange_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonExchange_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (buttonCancel != null) {
                buttonCancel.Dispose ();
                buttonCancel = null;
            }

            if (buttonDone != null) {
                buttonDone.Dispose ();
                buttonDone = null;
            }

            if (buttonExchange != null) {
                buttonExchange.Dispose ();
                buttonExchange = null;
            }

            if (textAPIKey != null) {
                textAPIKey.Dispose ();
                textAPIKey = null;
            }

            if (textAPISecret != null) {
                textAPISecret.Dispose ();
                textAPISecret = null;
            }
        }
    }
}