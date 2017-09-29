using System;
using Foundation;
using UIKit;
using CryptoAccouting.CoreClass;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

namespace CryptoAccouting.UIClass
{
	public class CoinBookingTableSource : UITableViewSource
	{
		Balance myBalance;
        string instrumentid_selected;
		NSString cellIdentifier = new NSString("BookingCell");
		CryptoTableViewController owner;

        public CoinBookingTableSource(string instrumentid, Balance myBalance, CryptoTableViewController owner)
        {
            this.myBalance = myBalance;
            this.owner = owner;
            this.instrumentid_selected = instrumentid;
        }

        private List<Position> BookingPositions()
        {
            return myBalance.Where(x => x.Coin.Id == instrumentid_selected).ToList();
        }

		public override nint RowsInSection(UITableView tableview, nint section)
		{
            return BookingPositions().Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			// OLD method
            //var cell = tableView.DequeueReusableCell(cellIdentifier) as CoinBookingCell;
			//if (cell == null)
            //    cell = new CoinBookingCell(cellIdentifier);
            
            var cell = (CoinBookingCell)tableView.DequeueReusableCell(cellIdentifier, indexPath);
			cell.UpdateCell(BookingPositions()[indexPath.Row]);
			
            return cell;
		}

		public Position GetItem(int id)
		{
			return BookingPositions()[id];
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
			owner.PerformSegue("PositionEditSegue", owner);
			tableView.DeselectRow(indexPath, true);
		}

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
		{
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    //tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    myBalance.Detach(BookingPositions()[indexPath.Row]);
                    myBalance.ReCalculate();
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
            UIView view = new UIView(new System.Drawing.RectangleF(0, 0, (float)tv.Frame.Width, 20));
            view.BackgroundColor = UIColor.Gray;

			var codeLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear,
                Text = "Coin"
            };

            var amountLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear,
                Text = "Holdings"
            };

            var TDLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear,
                Text = "Date"
            };

            var exchangeLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear,
                Text = "Exchange"
            };

            var storageLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear,
                Text = "Storage"
            };

			var showTD = (float)tv.Frame.Width > 320 ? true : false;
			var width = showTD ? (float)tv.Frame.Width / 5 : (float)tv.Frame.Width / 4;

			codeLabel.Frame = new System.Drawing.RectangleF(20, 0, 50, 20);
			amountLabel.Frame = new System.Drawing.RectangleF(width, 0, 90, 20);

			if (showTD)
            {
                TDLabel.Frame = new System.Drawing.RectangleF((width * 2) + 10, 0, 60, 20);
                exchangeLabel.Frame = new System.Drawing.RectangleF(width * 3, 0, 60, 20);
                storageLabel.Frame = new System.Drawing.RectangleF(width * 4, 0, 60, 20);
                view.AddSubviews(new UIView[] { codeLabel, amountLabel, TDLabel, exchangeLabel, storageLabel });
            }
            else
            {
				exchangeLabel.Frame = new System.Drawing.RectangleF(width * 2, 0, 60, 20);
				storageLabel.Frame = new System.Drawing.RectangleF(width * 3, 0, 60, 20);
                view.AddSubviews(new UIView[] { codeLabel, amountLabel, exchangeLabel, storageLabel });
            }

            return view;
        }
	}


}