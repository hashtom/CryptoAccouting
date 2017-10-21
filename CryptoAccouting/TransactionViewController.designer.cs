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
		UIKit.UILabel labelBTCBuy { get; set; }

		[Outlet]
		UIKit.UILabel labelBTCSell { get; set; }

		[Outlet]
		UIKit.UILabel labelFiatBuy { get; set; }

		[Outlet]
		UIKit.UILabel labelFiatSell { get; set; }

		[Outlet]
		UIKit.UILabel labelNumBuy { get; set; }

		[Outlet]
		UIKit.UILabel labelNumSell { get; set; }

		[Outlet]
		UIKit.UIView TradeTopView { get; set; }

		[Outlet]
		UIKit.UIView TransactionView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
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

			if (labelNumBuy != null) {
				labelNumBuy.Dispose ();
				labelNumBuy = null;
			}

			if (labelNumSell != null) {
				labelNumSell.Dispose ();
				labelNumSell = null;
			}

			if (labelFiatBuy != null) {
				labelFiatBuy.Dispose ();
				labelFiatBuy = null;
			}

			if (labelFiatSell != null) {
				labelFiatSell.Dispose ();
				labelFiatSell = null;
			}

			if (labelBTCBuy != null) {
				labelBTCBuy.Dispose ();
				labelBTCBuy = null;
			}

			if (labelBTCSell != null) {
				labelBTCSell.Dispose ();
				labelBTCSell = null;
			}

			if (TradeTopView != null) {
				TradeTopView.Dispose ();
				TradeTopView = null;
			}

			if (TransactionView != null) {
				TransactionView.Dispose ();
				TransactionView = null;
			}
		}
	}
}
