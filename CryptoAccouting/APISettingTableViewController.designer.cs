// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CryptoAccouting
{
	[Register ("APISettingTableViewController")]
	partial class APISettingTableViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIBarButtonItem buttonDone { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton buttonExchange { get; set; }

		[Outlet]
		UIKit.UIButton buttonFetchPosition { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField textAPIKey { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField textAPISecret { get; set; }

		[Action ("ButtonDone_Activated:")]
		partial void ButtonDone_Activated (UIKit.UIBarButtonItem sender);

		[Action ("ButtonExchange_TouchUpInside:")]
		partial void ButtonExchange_TouchUpInside (UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (buttonDone != null) {
				buttonDone.Dispose ();
				buttonDone = null;
			}

			if (buttonExchange != null) {
				buttonExchange.Dispose ();
				buttonExchange = null;
			}

			if (buttonFetchPosition != null) {
				buttonFetchPosition.Dispose ();
				buttonFetchPosition = null;
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
