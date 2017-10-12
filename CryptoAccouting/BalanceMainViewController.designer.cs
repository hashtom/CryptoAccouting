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
	[Register ("BalanceMainViewController")]
	partial class BalanceMainViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView BalanceTopView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIBarButtonItem buttonAddNew { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIBarButtonItem buttonRefresh { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel label1dPctBTC { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel labelCcy { get; set; }

		[Outlet]
		UIKit.UILabel labelLastUpdate { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel labelTotalBTC { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel labelTotalFiat { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UISegmentedControl SegmentBalance { get; set; }

		[Action ("ButtonAddNew_Activated:")]
		partial void ButtonAddNew_Activated (UIKit.UIBarButtonItem sender);

		[Action ("ButtonRefresh_Activated:")]
		partial void ButtonRefresh_Activated (UIKit.UIBarButtonItem sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (labelLastUpdate != null) {
				labelLastUpdate.Dispose ();
				labelLastUpdate = null;
			}

			if (BalanceTopView != null) {
				BalanceTopView.Dispose ();
				BalanceTopView = null;
			}

			if (buttonAddNew != null) {
				buttonAddNew.Dispose ();
				buttonAddNew = null;
			}

			if (buttonRefresh != null) {
				buttonRefresh.Dispose ();
				buttonRefresh = null;
			}

			if (label1dPctBTC != null) {
				label1dPctBTC.Dispose ();
				label1dPctBTC = null;
			}

			if (labelCcy != null) {
				labelCcy.Dispose ();
				labelCcy = null;
			}

			if (labelTotalBTC != null) {
				labelTotalBTC.Dispose ();
				labelTotalBTC = null;
			}

			if (labelTotalFiat != null) {
				labelTotalFiat.Dispose ();
				labelTotalFiat = null;
			}

			if (SegmentBalance != null) {
				SegmentBalance.Dispose ();
				SegmentBalance = null;
			}
		}
	}
}
