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
		UIKit.UIBarButtonItem barbuttonShare { get; set; }

		[Outlet]
		UIKit.UIButton buttonExchange { get; set; }

		[Outlet]
		UIKit.UIButton buttonSearch { get; set; }

		[Outlet]
		UIKit.UILabel labelBTCQtyBuy { get; set; }

		[Outlet]
		UIKit.UILabel labelBTCQtySell { get; set; }

		[Outlet]
		UIKit.UILabel labelBTCValBuy { get; set; }

		[Outlet]
		UIKit.UILabel labelBTCValSell { get; set; }

		[Outlet]
		UIKit.UILabel labelMonaQtyBuy { get; set; }

		[Outlet]
		UIKit.UILabel labelMonaQtySell { get; set; }

		[Outlet]
		UIKit.UILabel labelMonaValBuy { get; set; }

		[Outlet]
		UIKit.UILabel labelMonaValSell { get; set; }

		[Outlet]
		UIKit.UIView TradeTopView { get; set; }

		[Outlet]
		UIKit.UIView TransactionView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TradeTopView != null) {
				TradeTopView.Dispose ();
				TradeTopView = null;
			}

			if (barbuttonShare != null) {
				barbuttonShare.Dispose ();
				barbuttonShare = null;
			}

			if (buttonExchange != null) {
				buttonExchange.Dispose ();
				buttonExchange = null;
			}

			if (buttonSearch != null) {
				buttonSearch.Dispose ();
				buttonSearch = null;
			}

			if (labelBTCQtyBuy != null) {
				labelBTCQtyBuy.Dispose ();
				labelBTCQtyBuy = null;
			}

			if (labelBTCQtySell != null) {
				labelBTCQtySell.Dispose ();
				labelBTCQtySell = null;
			}

			if (labelBTCValBuy != null) {
				labelBTCValBuy.Dispose ();
				labelBTCValBuy = null;
			}

			if (labelBTCValSell != null) {
				labelBTCValSell.Dispose ();
				labelBTCValSell = null;
			}

			if (labelMonaQtyBuy != null) {
				labelMonaQtyBuy.Dispose ();
				labelMonaQtyBuy = null;
			}

			if (labelMonaQtySell != null) {
				labelMonaQtySell.Dispose ();
				labelMonaQtySell = null;
			}

			if (labelMonaValBuy != null) {
				labelMonaValBuy.Dispose ();
				labelMonaValBuy = null;
			}

			if (labelMonaValSell != null) {
				labelMonaValSell.Dispose ();
				labelMonaValSell = null;
			}

			if (TransactionView != null) {
				TransactionView.Dispose ();
				TransactionView = null;
			}
		}
	}
}
