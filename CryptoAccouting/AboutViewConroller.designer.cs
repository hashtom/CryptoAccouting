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
	[Register ("AboutViewConroller")]
	partial class AboutViewConroller
	{
		[Outlet]
		UIKit.UIWebView CreditWebView { get; set; }

		[Outlet]
		UIKit.UILabel labelVersion { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (labelVersion != null) {
				labelVersion.Dispose ();
				labelVersion = null;
			}

			if (CreditWebView != null) {
				CreditWebView.Dispose ();
				CreditWebView = null;
			}
		}
	}
}
