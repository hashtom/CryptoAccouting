﻿using System;
using Foundation;
using UIKit;
using CryptoAccouting.CoreClass;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

namespace CryptoAccouting.UIClass
{
    public class ExchangeTableSource : UITableViewSource
    {
        List<Position> balanceViewItems;
        NSString cellIdentifier = new NSString("ExchangeViewCell"); // set in the Storyboard
        UITableViewController owner;

        public ExchangeTableSource(Balance myBalance, UITableViewController owner)
        {
            balanceViewItems = myBalance.positions;
            this.owner = owner;
        }
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return balanceViewItems.Count;

        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {

            // OLD method
            //var cell = tableView.DequeueReusableCell(cellIdentifier) as CustomBalanceCell;
            //if (cell == null)
            //cell = new CustomBalanceCell (cellIdentifier);

            var cell = (ExchangeViewCell)tableView.DequeueReusableCell(cellIdentifier, indexPath);
            cell.UpdateCell(balanceViewItems[indexPath.Row]);

            return cell;
        }
        public Position GetItem(int id)
        {
            return balanceViewItems[id];
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
                Text = "Exchange"
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

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //UIAlertController okAlertController = UIAlertController.Create("Row Selected", balanceViewItems.GetPositionByIndex(indexPath.Row).Coin.Name, UIAlertControllerStyle.Alert);
            //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            //owner.PresentViewController(okAlertController, true, null);

            owner.PerformSegue("PositionSegue", owner);
            tableView.DeselectRow(indexPath, true);

        }
    }
}
