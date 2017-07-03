using System;
using Foundation;
using UIKit;

namespace CryptoAccouting
{
	public class BalanceTableSource : UITableViewSource
	{
        Balance balanceViewItems;   
        NSString cellIdentifier = new NSString("BalanceCell"); // set in the Storyboard

        public BalanceTableSource(Balance myBalance)
		{
			balanceViewItems = myBalance;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return balanceViewItems.Count();
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{

            var cell = tableView.DequeueReusableCell(cellIdentifier) as CustomBalanceCell;
			if (cell == null)
				cell = new CustomBalanceCell (cellIdentifier);
            
            cell.UpdateCell(balanceViewItems.fetchPositionByIndex(indexPath.Row).Coin.Symbol
                            , balanceViewItems.fetchPositionByIndex(indexPath.Row).Amount.ToString()
                            , balanceViewItems.fetchPositionByIndex(indexPath.Row).PriceData.LatestPrice.ToString()
                            , balanceViewItems.fetchPositionByIndex(indexPath.Row).PriceData.DayVolume.ToString()
                            , UIImage.FromFile(balanceViewItems.fetchPositionByIndex(indexPath.Row).Coin.LogoFileName));

			return cell;
		}
		public Position GetItem(int id)
		{
            return balanceViewItems.fetchPositionByIndex(id);
		}
	}

}
