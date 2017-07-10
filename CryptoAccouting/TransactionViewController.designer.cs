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
    [Register ("TransactionViewController")]
    partial class TransactionViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelBougtAmount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelBougtPrice { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelExchangeName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelOutstandingAmount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelOutstandingPrice { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelRealizedPL { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelSoldAmount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelSoldPrice { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelUnrealizedPL { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView TransactionHistoryView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView TransactionSummaryView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (LabelBougtAmount != null) {
                LabelBougtAmount.Dispose ();
                LabelBougtAmount = null;
            }

            if (LabelBougtPrice != null) {
                LabelBougtPrice.Dispose ();
                LabelBougtPrice = null;
            }

            if (LabelExchangeName != null) {
                LabelExchangeName.Dispose ();
                LabelExchangeName = null;
            }

            if (LabelOutstandingAmount != null) {
                LabelOutstandingAmount.Dispose ();
                LabelOutstandingAmount = null;
            }

            if (LabelOutstandingPrice != null) {
                LabelOutstandingPrice.Dispose ();
                LabelOutstandingPrice = null;
            }

            if (LabelRealizedPL != null) {
                LabelRealizedPL.Dispose ();
                LabelRealizedPL = null;
            }

            if (LabelSoldAmount != null) {
                LabelSoldAmount.Dispose ();
                LabelSoldAmount = null;
            }

            if (LabelSoldPrice != null) {
                LabelSoldPrice.Dispose ();
                LabelSoldPrice = null;
            }

            if (LabelUnrealizedPL != null) {
                LabelUnrealizedPL.Dispose ();
                LabelUnrealizedPL = null;
            }

            if (TransactionHistoryView != null) {
                TransactionHistoryView.Dispose ();
                TransactionHistoryView = null;
            }

            if (TransactionSummaryView != null) {
                TransactionSummaryView.Dispose ();
                TransactionSummaryView = null;
            }
        }
    }
}