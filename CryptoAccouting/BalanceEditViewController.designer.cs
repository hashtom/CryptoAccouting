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
	[Register ("BalanceEditViewController")]
	partial class BalanceEditViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIBarButtonItem buttonDone { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIBarButtonItem buttonEdit { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton buttonExchange { get; set; }

		[Outlet]
		UIKit.UIButton buttonWallet { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIImageView imageCoin { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel labelBTCPrice { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel labelBTCRet { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel labelCoinName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel labelCoinSymbol { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel labelFiat1dRet { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel labelFiatPrice { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField textBalanceDate { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField textBookPrice { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField textQuantity { get; set; }

		[Action ("ButtonDone_Activated:")]
		partial void ButtonDone_Activated (UIKit.UIBarButtonItem sender);

		[Action ("ButtonEdit_Activated:")]
		partial void ButtonEdit_Activated (UIKit.UIBarButtonItem sender);

		[Action ("buttonExchange_TouchUpInside:")]
		partial void buttonExchange_TouchUpInside (UIKit.UIButton sender);

		[Action ("ButtonExchange_TouchUpInside:")]
		partial void ButtonExchange_TouchUpInside (UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (buttonDone != null) {
				buttonDone.Dispose ();
				buttonDone = null;
			}

			if (buttonEdit != null) {
				buttonEdit.Dispose ();
				buttonEdit = null;
			}

			if (buttonExchange != null) {
				buttonExchange.Dispose ();
				buttonExchange = null;
			}

			if (buttonWallet != null) {
				buttonWallet.Dispose ();
				buttonWallet = null;
			}

			if (imageCoin != null) {
				imageCoin.Dispose ();
				imageCoin = null;
			}

			if (labelBTCPrice != null) {
				labelBTCPrice.Dispose ();
				labelBTCPrice = null;
			}

			if (labelBTCRet != null) {
				labelBTCRet.Dispose ();
				labelBTCRet = null;
			}

			if (labelCoinName != null) {
				labelCoinName.Dispose ();
				labelCoinName = null;
			}

			if (labelCoinSymbol != null) {
				labelCoinSymbol.Dispose ();
				labelCoinSymbol = null;
			}

			if (labelFiat1dRet != null) {
				labelFiat1dRet.Dispose ();
				labelFiat1dRet = null;
			}

			if (labelFiatPrice != null) {
				labelFiatPrice.Dispose ();
				labelFiatPrice = null;
			}

			if (textBalanceDate != null) {
				textBalanceDate.Dispose ();
				textBalanceDate = null;
			}

			if (textBookPrice != null) {
				textBookPrice.Dispose ();
				textBookPrice = null;
			}

			if (textQuantity != null) {
				textQuantity.Dispose ();
				textQuantity = null;
			}
		}
	}
}
