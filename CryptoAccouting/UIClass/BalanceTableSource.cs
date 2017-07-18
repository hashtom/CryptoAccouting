﻿using System;
using Foundation;
using UIKit;
using CryptoAccouting.CoreClass;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.UIClass
{
	public class BalanceTableSource : UITableViewSource
	{
        Balance balanceViewItems;   
        NSString cellIdentifier = new NSString("BalanceCell"); // set in the Storyboard
        UITableViewController owner;

        public BalanceTableSource(Balance myBalance, UITableViewController owner )
		{
			balanceViewItems = myBalance;
            this.owner = owner;
		}

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return balanceViewItems.Count();

        }

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{

            // OLD method
            //var cell = tableView.DequeueReusableCell(cellIdentifier) as CustomBalanceCell;
			//if (cell == null)
                //cell = new CustomBalanceCell (cellIdentifier);
            var cell = (CustomBalanceCell)tableView.DequeueReusableCell(cellIdentifier, indexPath);
            var logo = balanceViewItems.GetPositionByIndex(indexPath.Row).Coin.LogoFileName;

            cell.UpdateCell(balanceViewItems.GetPositionByIndex(indexPath.Row).Coin.Symbol
                            , balanceViewItems.GetPositionByIndex(indexPath.Row).Amount.ToString()
                            , balanceViewItems.GetPositionByIndex(indexPath.Row).MarketPrice().ToString()
                            , balanceViewItems.GetPositionByIndex(indexPath.Row).Pct1d().ToString() + "%"
                            , logo == null ? null : UIImage.FromFile(logo));
            cell.UserInteractionEnabled = true; //test to delete

			return cell;
		}
		public Position GetItem(int id)
		{
            return balanceViewItems.GetPositionByIndex(id);
		}

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //UIAlertController okAlertController = UIAlertController.Create("Row Selected", balanceViewItems.GetPositionByIndex(indexPath.Row).Coin.Name, UIAlertControllerStyle.Alert);
			//okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            //owner.PresentViewController(okAlertController, true, null);

            owner.PerformSegue("PositionSegue",owner);
            tableView.DeselectRow(indexPath, true); 

		}

	}

}
