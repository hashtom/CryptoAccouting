using System;
using Foundation;
using UIKit;

namespace CryptoAccouting
{
	public class BalanceTableSource : UITableViewSource
	{
		Position[] tableItems;
		string cellIdentifier = "BalanceCell"; // set in the Storyboard

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
			// in a Storyboard, Dequeue will ALWAYS return a cell, 
			var cell = tableView.DequeueReusableCell(cellIdentifier);
			// now set the properties as normal
			//cell.TextLabel.Text = tableItems[indexPath.Row].ItemName;
			//cell.DetailTextLabel.Text = tableItems[indexPath.Row].Category.ToString();
			//if ( tableItems[indexPath.Row].DressPhotoFile != null)
			//	cell.ImageView.Image = UIImage.FromFile(tableItems[indexPath.Row].DressPhotoFile);

			return cell;
		}
		public Position GetItem(int id)
		{
			return tableItems[id];
		}
	}

}
