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
	[Register ("TransactionViewController")]
	partial class TransactionViewController
	{
		[Outlet]
		UIKit.UIButton buttonCalndarYear { get; set; }

		[Outlet]
		UIKit.UIButton buttonExchange { get; set; }

		[Outlet]
		UIKit.UIButton buttonSearch { get; set; }

		[Outlet]
		UIKit.UIView TransactionView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TransactionView != null) {
				TransactionView.Dispose ();
				TransactionView = null;
			}

			if (buttonSearch != null) {
				buttonSearch.Dispose ();
				buttonSearch = null;
			}

			if (buttonExchange != null) {
				buttonExchange.Dispose ();
				buttonExchange = null;
			}

			if (buttonCalndarYear != null) {
				buttonCalndarYear.Dispose ();
				buttonCalndarYear = null;
			}
		}
	}
}
