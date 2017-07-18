using Foundation;
using System;
using UIKit;
using CoreGraphics;
using Syncfusion.SfDataGrid;
using CryptoAccouting.CoreClass;
using CryptoAccouting.UIClass;
using CryptoAccouting.CoreClass.APIClass; // To be deleted

namespace CryptoAccouting
{
    public partial class TransactionViewController : UIViewController
    {
		SfDataGrid sfGrid;
        private NavigationDrawer menu;

        public TransactionViewController(IntPtr handle) : base(handle)
        {
			sfGrid = new SfDataGrid();
   			//sfGrid.AutoGeneratingColumn += HandleAutoGeneratingColumn;
            sfGrid.AllowSorting = true;
            sfGrid.AutoGenerateColumns = false;
            sfGrid.BackgroundColor = UIColor.FromRGB(236, 184, 138);
			//sfGrid.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = "txid" });

			GridTextColumn txIdColumn = new GridTextColumn();
			txIdColumn.MappingName = "TxId";
			txIdColumn.HeaderText = "tx";

			GridTextColumn dateColumn = new GridTextColumn();
			dateColumn.MappingName = "TradeDate";
			dateColumn.HeaderText = "TDate";

			//GridTextColumn coinColumn = new GridTextColumn();
			//coinColumn.MappingName = "Symbol";
			//coinColumn.HeaderText = "Coin";

			GridTextColumn buysellColumn = new GridTextColumn();
			buysellColumn.MappingName = "Side";
            buysellColumn.HeaderText = "BuySell";

			GridTextColumn amountColumn = new GridTextColumn();
			amountColumn.MappingName = "Quantity";
            amountColumn.HeaderText = "Qty";

			GridTextColumn priceColumn = new GridTextColumn();
			priceColumn.MappingName = "TradePrice";
			priceColumn.HeaderText = "Price";

			GridTextColumn valueColumn = new GridTextColumn();
			valueColumn.MappingName = "TradeValue";
            valueColumn.HeaderText = "Value";


			sfGrid.Columns.Add(txIdColumn);
			sfGrid.Columns.Add(dateColumn);
            //sfGrid.Columns.Add(coinColumn);
            sfGrid.Columns.Add(buysellColumn);
			sfGrid.Columns.Add(amountColumn);
			sfGrid.Columns.Add(priceColumn);
            sfGrid.Columns.Add(valueColumn);

		}
        
        public async override void ViewDidLoad()
		{
			base.ViewDidLoad();
            NavigationItem.RightBarButtonItem = EditButtonItem;
			menu = ApplicationCore.Navigation;

            var exg = new ExchangeAPI();
            var myTxs = await exg.FetchTransactionAsync((EnuExchangeType.Zaif), true, true);

            //Show Summary
            myTxs.ReEvaluate();
            this.LabelBougtAmount.Text = Math.Round(myTxs.TotalAmountBougt,0).ToString();
            this.LabelSoldAmount.Text = Math.Round(myTxs.TotalAmountSold,0).ToString();
            this.LabelOutstandingAmount.Text = Math.Round(myTxs.TotalAmountOutstanding,0).ToString();
            //this.LabelBougtPrice.Text = myTxs.BookPrice.ToString();
            //this.LabelSoldPrice.Text = myTxs..ToString();
            this.LabelOutstandingPrice.Text = Math.Round(myTxs.BookPrice,2).ToString();
            this.LabelRealizedPL.Text = Math.Round(myTxs.RealizedPL()/1000,2).ToString();
            this.LabelUnrealizedPL.Text = Math.Round(myTxs.UnrealizedPL()/1000,2).ToString();

            //Show Grid View
            sfGrid.ItemsSource = (myTxs.TransactionCollection);
            this.sfGrid.Frame = new CGRect(0,
                                           0, //TransactionSummaryView.Frame.Height + 60,
                                           TransactionHistoryView.Frame.Width,
                                           TransactionHistoryView.Frame.Height); // - TransactionSummaryView.Frame.Height - 70);
            sfGrid.HeaderRowHeight = 30;
            sfGrid.RowHeight = 30;
			//View.AddSubview(sfGrid);
            TransactionHistoryView.AddSubview(sfGrid);
		}

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        //void HandleAutoGeneratingColumn(object sender, AutoGeneratingColumnArgs e)
        //{
        //	if (e.Column.MappingName == "txid")
        //	{
        //		e.Column.TextMargin = 1;
        //		e.Column.TextAlignment = UITextAlignment.Left;
        //	}
        //}

    }
}