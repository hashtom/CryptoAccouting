// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace CoinBalance
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
        UIKit.UILabel labelNumBuy { get; set; }


        [Outlet]
        UIKit.UILabel labelNumSell { get; set; }


        [Outlet]
        UIKit.UILabel labelSettleCrossBuy { get; set; }


        [Outlet]
        UIKit.UILabel labelSettleCrossSell { get; set; }


        [Outlet]
        UIKit.UILabel labelSettleCrossText { get; set; }


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

            if (labelBTCBuy != null) {
                labelBTCBuy.Dispose ();
                labelBTCBuy = null;
            }

            if (labelBTCSell != null) {
                labelBTCSell.Dispose ();
                labelBTCSell = null;
            }

            if (labelNumBuy != null) {
                labelNumBuy.Dispose ();
                labelNumBuy = null;
            }

            if (labelNumSell != null) {
                labelNumSell.Dispose ();
                labelNumSell = null;
            }

            if (labelSettleCrossBuy != null) {
                labelSettleCrossBuy.Dispose ();
                labelSettleCrossBuy = null;
            }

            if (labelSettleCrossSell != null) {
                labelSettleCrossSell.Dispose ();
                labelSettleCrossSell = null;
            }

            if (labelSettleCrossText != null) {
                labelSettleCrossText.Dispose ();
                labelSettleCrossText = null;
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