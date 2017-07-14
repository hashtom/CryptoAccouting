using System;
using Foundation;
using UIKit;
using CryptoAccouting.CoreClass;

namespace CryptoAccouting.UIClass
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
            
            cell.UpdateCell(balanceViewItems.GetPositionByIndex(indexPath.Row).Coin.Symbol
                            , balanceViewItems.GetPositionByIndex(indexPath.Row).Amount.ToString()
                            , balanceViewItems.GetPositionByIndex(indexPath.Row).MarketPrice().ToString()
                            , balanceViewItems.GetPositionByIndex(indexPath.Row).Pct1d().ToString() + "%"
                            , UIImage.FromFile(balanceViewItems.GetPositionByIndex(indexPath.Row).Coin.LogoFileName));

			return cell;
		}
		public Position GetItem(int id)
		{
            return balanceViewItems.GetPositionByIndex(id);
		}

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            base.RowSelected(tableView, indexPath);


        }
	}

}
