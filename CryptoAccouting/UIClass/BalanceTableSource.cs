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
        NSString cellIdentifier = new NSString("BalanceViewCell"); // set in the Storyboard
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

            var cell = (BalanceViewCell)tableView.DequeueReusableCell(cellIdentifier, indexPath);
            cell.UpdateCell(balanceViewItems.GetPositionByIndex(indexPath.Row));

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

        //public void AddRowHeader()
        //{
        //    var coinheader = new Instrument("RowHeader", "Symbol", "Coin Name");
        //    balanceViewItems.AttachPosition(new Position(coinheader, 0));
        //    balanceViewItems.OrderBy(x => x.Id);
        //}

	}

}
