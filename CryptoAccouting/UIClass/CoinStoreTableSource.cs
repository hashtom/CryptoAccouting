﻿using System;
using Foundation;
using UIKit;
using CoinBalance.CoreClass;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

namespace CoinBalance.UIClass
{
    public class CoinStoreTableSource : UITableViewSource
    {
        CoinStorageList storagelist;
        NSString cellIdentifier = new NSString("CoinStorageCell"); // set in the Storyboard
        CryptoTableViewController owner;

        public CoinStoreTableSource(CoinStorageList storagelist, CryptoTableViewController owner)
        {
            this.storagelist = storagelist;
            this.owner = owner;
        }
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return storagelist.Count();
            //return myBalance.positionsByBookingExchange.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {

            // OLD method
            //var cell = tableView.DequeueReusableCell(cellIdentifier) as CustomBalanceCell;
            //if (cell == null)
            //cell = new CustomBalanceCell (cellIdentifier);

            var cell = (CoinStoreCell)tableView.DequeueReusableCell(cellIdentifier, indexPath);
            cell.UpdateCell(storagelist.GetByIndex(indexPath.Row));
            return cell;
        }

        public CoinStorage GetItem(int id)
        {
            return storagelist.GetByIndex(id);
        }

		//public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
		//{
		//	return true;
		//}

        //public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        //{
        //    return true; // return false if you wish to disable editing for a specific indexPath or for all rows
        //}

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            return BuidBlanceViewHeader(tableView);
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 20;
        }

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			//owner.PerformSegue("PositionSegue", owner);
			tableView.DeselectRow(indexPath, true);
		}

        public static UIView BuidBlanceViewHeader(UITableView tv)
        {
            UIView view = new UIView(new System.Drawing.RectangleF(0, 0, (float)tv.Frame.Width, 20));
            view.BackgroundColor = UIColor.Gray;
            var width = (float)tv.Frame.Width / 4;

            var codeLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Frame = new System.Drawing.RectangleF(0, 0, 110, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Location"
            };

            var amountLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Frame = new System.Drawing.RectangleF(width, 0, 100, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Holding"
            };

            var valueLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Frame = new System.Drawing.RectangleF(width * 2, 0, 100, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Value"
            };

            var pctLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Frame = new System.Drawing.RectangleF(width * 3, 0, 60, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Weight"
            };

            view.AddSubviews(new UIView[] { codeLabel, amountLabel, valueLabel, pctLabel });
            return view;
        }

    }
}
