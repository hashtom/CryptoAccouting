using Foundation;
using System;
using UIKit;
using CoreGraphics;
using Syncfusion.SfDataGrid;

namespace CryptoAccouting
{
    public partial class TransactionViewController : UIViewController
    {
		SfDataGrid sfGrid;

        public TransactionViewController(IntPtr handle) : base(handle)
        {
			sfGrid = new SfDataGrid();
   			//sfGrid.AutoGeneratingColumn += HandleAutoGeneratingColumn;
            sfGrid.AllowSorting = true;
            sfGrid.AutoGenerateColumns = false;
			//sfGrid.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = "txid" });

			GridTextColumn txIdColumn = new GridTextColumn();
			txIdColumn.MappingName = "txid";
			txIdColumn.HeaderText = "txID";

			GridTextColumn coinColumn = new GridTextColumn();
			coinColumn.MappingName = "CoinName";
			coinColumn.HeaderText = "Coin";

			GridTextColumn buysellColumn = new GridTextColumn();
			buysellColumn.MappingName = "BuySell";
            buysellColumn.HeaderText = "BuySell";

			GridTextColumn amountColumn = new GridTextColumn();
			amountColumn.MappingName = "Amount";
            amountColumn.HeaderText = "Amount";

			GridTextColumn priceColumn = new GridTextColumn();
			priceColumn.MappingName = "TradePrice";
			priceColumn.HeaderText = "Price";

			GridTextColumn valueColumn = new GridTextColumn();
			valueColumn.MappingName = "TradeValue";
            valueColumn.HeaderText = "Value";

			GridTextColumn dateColumn = new GridTextColumn();
			dateColumn.MappingName = "TradeDate";
            dateColumn.HeaderText = "TradeDate";


			sfGrid.Columns.Add(txIdColumn);
			sfGrid.Columns.Add(coinColumn);
            sfGrid.Columns.Add(buysellColumn);
			sfGrid.Columns.Add(amountColumn);
			sfGrid.Columns.Add(priceColumn);
            sfGrid.Columns.Add(valueColumn);
            sfGrid.Columns.Add(dateColumn);
		}
        
        public async override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
            var exg = new ExchangeAPI();
            var myTxs = await exg.FetchTransaction2((EnuExchangeType.Zaif));

            sfGrid.ItemsSource = (myTxs.TransactionCollection);
            this.sfGrid.Frame = new CGRect(0, 70, View.Frame.Width, View.Frame.Height);
            sfGrid.HeaderRowHeight = 30;
            sfGrid.RowHeight = 30;
			View.AddSubview(sfGrid);
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