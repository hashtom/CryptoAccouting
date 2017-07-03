using System;
using Foundation;
using UIKit;

namespace CryptoAccouting
{
    public class TransactionTableSource : UITableViewSource
    {
        Transactions txsViewItems;
		NSString cellIdentifier = new NSString("TransactionCell"); // set in the Storyboard

        public TransactionTableSource(Transactions myTransactions)
        {
            txsViewItems = myTransactions;
        }

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{

			var cell = tableView.DequeueReusableCell(cellIdentifier) as CustomBalanceCell;
			if (cell == null)
				cell = new CustomBalanceCell(cellIdentifier);

			cell.UpdateCell(txsViewItems.GetTransactionByIndex(indexPath.Row).Coin.Symbol
							, txsViewItems.GetTransactionByIndex(indexPath.Row).Amount.ToString()
							, txsViewItems.GetTransactionByIndex(indexPath.Row).PriceData.LatestPrice.ToString()
							, txsViewItems.GetTransactionByIndex(indexPath.Row).PriceData.DayVolume.ToString()
							, UIImage.FromFile(txsViewItems.GetTransactionByIndex(indexPath.Row).Coin.LogoFileName));

			return cell;
		}
        public Transaction GetItem(int id)
		{
			return txsViewItems.GetTransactionByIndex(id);
		}
    }
}
