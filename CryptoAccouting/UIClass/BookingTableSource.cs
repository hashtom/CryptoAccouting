using System;
using Foundation;
using UIKit;
using CryptoAccouting.CoreClass;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

namespace CryptoAccouting.UIClass
{
	public class BookingTableSource : UITableViewSource
	{
		Balance myBalance;
		NSString cellIdentifier = new NSString("BookingViewCell");
		BalanceTableViewController owner;
		List<Position> bookingPositions;

        public BookingTableSource(string symbol, List<Position> bookingPositions, BalanceTableViewController owner)
        {
            this.myBalance = ApplicationCore.Balance;
            this.owner = owner;
            this.bookingPositions = bookingPositions; //= myBalance.positions.Where(x => x.Coin.Symbol == symbol).ToList();
        }

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return bookingPositions.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			// OLD method
			//var cell = tableView.DequeueReusableCell(cellIdentifier) as CustomBalanceCell;
			//if (cell == null)
            var cell = (CoinViewCell)tableView.DequeueReusableCell(cellIdentifier, indexPath);
			cell.UpdateCell(bookingPositions[indexPath.Row]);
			
            return cell;

		}

		public Position GetItem(int id)
		{
			return bookingPositions[id];
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
                    myBalance.DetachPosition(bookingPositions[indexPath.Row]);
                    break;

                case UITableViewCellEditingStyle.None:
                    Console.WriteLine("CommitEditingStyle:None called");
                    break;
            }

			ApplicationCore.SaveMyBalanceXML();
			owner.CellItemUpdated();

		}

		public static UIView BuidBlanceViewHeader(UITableView tv)
		{

			UILabel codeLabel, amountLabel, priceLabel, pctLabel;

			UIView view = new UIView(new System.Drawing.RectangleF(0, 0, (float)tv.Frame.Width, 20));
			view.BackgroundColor = UIColor.Gray;

			codeLabel = new UILabel()
			{
				Font = UIFont.FromName("ArialMT", 12f),
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Left,
				Frame = new System.Drawing.RectangleF(20, 0, 40, 20),
				BackgroundColor = UIColor.Clear,
				Text = "Coin"
			};

			amountLabel = new UILabel()
			{
				Font = UIFont.FromName("ArialMT", 12f),
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Left,
				Frame = new System.Drawing.RectangleF(100, 0, 60, 20),
				BackgroundColor = UIColor.Clear,
				Text = "Holdings"
			};

			priceLabel = new UILabel()
			{
				Font = UIFont.FromName("ArialMT", 12f),
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Left,
				Frame = new System.Drawing.RectangleF(200, 0, 60, 20),
				BackgroundColor = UIColor.Clear,
				Text = "Price"
			};

			pctLabel = new UILabel()
			{
				Font = UIFont.FromName("ArialMT", 12f),
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Left,
				Frame = new System.Drawing.RectangleF(300, 0, 40, 20),
				BackgroundColor = UIColor.Clear,
				Text = "Return"
			};

			view.AddSubviews(new UIView[] { codeLabel, amountLabel, priceLabel, pctLabel });
			return view;
		}
	}


}