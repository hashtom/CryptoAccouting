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
	[Register ("BalanceDetailViewConrtoller")]
	partial class BalanceDetailViewConrtoller
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIBarButtonItem buttonAddNew { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton buttonPriceSource { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView DetailTopView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIImageView imageCoin { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel labelMarketValue { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel labelMarketValueTitle { get; set; }

		[Outlet]
		UIKit.UILabel labelName { get; set; }

		[Outlet]
		UIKit.UILabel labelPrice { get; set; }

		[Outlet]
		UIKit.UILabel labelPriceBase { get; set; }

		[Outlet]
		UIKit.UILabel labelPriceBaseTitle { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel labelTotalQty { get; set; }

		[Outlet]
		UIKit.UILabel labelVolume { get; set; }

		[Action ("ButtonAddNew_Activated:")]
		partial void ButtonAddNew_Activated (UIKit.UIBarButtonItem sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (buttonAddNew != null) {
				buttonAddNew.Dispose ();
				buttonAddNew = null;
			}

			if (buttonPriceSource != null) {
				buttonPriceSource.Dispose ();
				buttonPriceSource = null;
			}

			if (DetailTopView != null) {
				DetailTopView.Dispose ();
				DetailTopView = null;
			}

			if (imageCoin != null) {
				imageCoin.Dispose ();
				imageCoin = null;
			}

			if (labelMarketValue != null) {
				labelMarketValue.Dispose ();
				labelMarketValue = null;
			}

			if (labelMarketValueTitle != null) {
				labelMarketValueTitle.Dispose ();
				labelMarketValueTitle = null;
			}

			if (labelPrice != null) {
				labelPrice.Dispose ();
				labelPrice = null;
			}

			if (labelPriceBase != null) {
				labelPriceBase.Dispose ();
				labelPriceBase = null;
			}

			if (labelVolume != null) {
				labelVolume.Dispose ();
				labelVolume = null;
			}

			if (labelPriceBaseTitle != null) {
				labelPriceBaseTitle.Dispose ();
				labelPriceBaseTitle = null;
			}

			if (labelName != null) {
				labelName.Dispose ();
				labelName = null;
			}

			if (labelTotalQty != null) {
				labelTotalQty.Dispose ();
				labelTotalQty = null;
			}
		}
	}
}
