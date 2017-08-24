﻿using System;
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
        string symbol_selected;
		NSString cellIdentifier = new NSString("BookingViewCell");
		CryptoTableViewController owner;

        public CoinBookingTableSource(string symbol, Balance myBalance, CryptoTableViewController owner)
        {
            this.myBalance = myBalance;
            this.owner = owner;
            symbol_selected = symbol;
        }

        private List<Position> BookingPositions()
        {
            return myBalance.Where(x => x.Coin.Symbol == symbol_selected).ToList();
        }

		public override nint RowsInSection(UITableView tableview, nint section)
		{
            return BookingPositions().Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			// OLD method
			//var cell = tableView.DequeueReusableCell(cellIdentifier) as CustomBalanceCell;
			//if (cell == null)
            var cell = (CoinViewCell)tableView.DequeueReusableCell(cellIdentifier, indexPath);
			cell.UpdateCell(BookingPositions()[indexPath.Row], true);
			
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

			//UILabel codeLabel, amountLabel, priceLabel, pctLabel;

			UIView view = new UIView(new System.Drawing.RectangleF(0, 0, (float)tv.Frame.Width, 20));
			view.BackgroundColor = UIColor.Gray;

			var codeLabel = new UILabel()
			{
				Font = UIFont.FromName("ArialMT", 12f),
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Left,
				Frame = new System.Drawing.RectangleF(20, 0, 40, 20),
				BackgroundColor = UIColor.Clear,
				Text = "Coin"
			};

			var amountLabel = new UILabel()
			{
				Font = UIFont.FromName("ArialMT", 12f),
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Left,
				Frame = new System.Drawing.RectangleF(100, 0, 60, 20),
				BackgroundColor = UIColor.Clear,
				Text = "Holdings"
			};

			var priceLabel = new UILabel()
			{
				Font = UIFont.FromName("ArialMT", 12f),
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Left,
				Frame = new System.Drawing.RectangleF(200, 0, 60, 20),
				BackgroundColor = UIColor.Clear,
				Text = "Book"
			};

			var exchangeLabel = new UILabel()
			{
				Font = UIFont.FromName("ArialMT", 12f),
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Left,
				Frame = new System.Drawing.RectangleF(300, 0, 60, 20),
				BackgroundColor = UIColor.Clear,
				Text = "Exchange"
			};

			var storageLabel = new UILabel()
			{
				Font = UIFont.FromName("ArialMT", 12f),
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Left,
				Frame = new System.Drawing.RectangleF(300, 0, 60, 20),
				BackgroundColor = UIColor.Clear,
				Text = "Storage"
			};



            view.AddSubviews(new UIView[] { codeLabel, amountLabel, priceLabel, exchangeLabel, storageLabel });
			return view;
		}
	}


}