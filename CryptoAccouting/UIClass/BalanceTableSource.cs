using System;
using Foundation;
using UIKit;

namespace CryptoAccouting
{
	public class BalanceTableSource : UITableViewSource
	{
		Position[] tableItems;
        NSString cellIdentifier = new NSString("BalanceCell"); // set in the Storyboard

		public BalanceTableSource(Position[] items)
		{
			tableItems = items;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return tableItems.Length;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{

            var cell = tableView.DequeueReusableCell(cellIdentifier) as CustomBalanceCell;
			if (cell == null)
				cell = new CustomBalanceCell (cellIdentifier);
            cell.UpdateCell(tableItems[indexPath.Row].Asset.Symbol
                            , tableItems[indexPath.Row].Amount.ToString()
                            , UIImage.FromFile("Images/bitcoin.png"));

			return cell;
		}
		public Position GetItem(int id)
		{
			return tableItems[id];
		}
	}

}
