﻿using System;
using Foundation;
using UIKit;
using CryptoAccouting.CoreClass;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

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

		public override nint NumberOfSections(UITableView tableView)
		{
            return 1;
		}

		//public override string TitleForHeader(UITableView tableView, nint section)
		//{
		//	return "Test title";
		//}

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

			//UILabel label = new UILabel();
			//label.Opaque = false;
   //         label.TextColor = UIColor.Black;
			//label.Font = UIFont.FromName("Helvetica-Bold", 16f);
			//label.Frame = new System.Drawing.RectangleF(5, 10, 315, 20);
			//label.Text = "test label";
			//view.AddSubview(label);

            codeLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Left,
                Frame = new System.Drawing.RectangleF(20, 0, 40, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Coin"
            };
            //view.AddSubview(codeLabel);

            amountLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Left,
                Frame = new System.Drawing.RectangleF(100, 0, 60, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Holdings"
            };
            //view.AddSubview(amountLabel);

            priceLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Left,
                Frame = new System.Drawing.RectangleF(200, 0, 60, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Price"
            };
            //view.AddSubview(priceLabel);

            pctLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Left,
                Frame = new System.Drawing.RectangleF(300, 0, 40, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Return"
            };
            //view.AddSubview(pctLabel);

            view.AddSubviews(new UIView[] { codeLabel, amountLabel, priceLabel, pctLabel });
			return view;
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
