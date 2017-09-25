using System;
using Foundation;
using UIKit;
using CryptoAccouting.CoreClass;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

namespace CryptoAccouting.UIClass
{
    public class CoinStorageTableSource : UITableViewSource
    {
        CoinStorageList storagelist;
        NSString cellIdentifier = new NSString("CoinStorageCell"); // set in the Storyboard
        CryptoTableViewController owner;

        public CoinStorageTableSource(CoinStorageList storagelist, CryptoTableViewController owner)
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

            var cell = (CoinStorageCell)tableView.DequeueReusableCell(cellIdentifier, indexPath);
            cell.UpdateCell(storagelist.GetByIndex(indexPath.Row));
            return cell;
        }

        public CoinStorage GetItem(int id)
        {
            return storagelist.GetByIndex(id);
        }

		public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true; // return false if you wish to disable editing for a specific indexPath or for all rows
        }

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

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
		{
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    //tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    //myBalance.DetachPositionByExchange(myBalance.positionsByBookingExchange[indexPath.Row]);
                    break;

                case UITableViewCellEditingStyle.None:
                    Console.WriteLine("CommitEditingStyle:None called");
                    break;
            }

			ApplicationCore.SaveMyBalanceXML();
            owner.CellItemUpdated(EnuPopTo.None);

		}

        public static UIView BuidBlanceViewHeader(UITableView tv)
        {

            UILabel codeLabel, amountLabel, valueLabel, pctLabel;

            UIView view = new UIView(new System.Drawing.RectangleF(0, 0, (float)tv.Frame.Width, 20));
            view.BackgroundColor = UIColor.Gray;

            codeLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Left,
                Frame = new System.Drawing.RectangleF(20, 0, 50, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Location"
            };

            amountLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Left,
                Frame = new System.Drawing.RectangleF(100, 0, 60, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Holding"
            };

            valueLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Left,
                Frame = new System.Drawing.RectangleF(200, 0, 60, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Value"
            };

            pctLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Left,
                Frame = new System.Drawing.RectangleF(300, 0, 40, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Weight"
            };

            view.AddSubviews(new UIView[] { codeLabel, amountLabel, valueLabel, pctLabel });
            return view;
        }

    }
}
