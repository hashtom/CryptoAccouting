using System;
using Foundation;
using UIKit;
using CryptoAccouting.CoreClass;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using System.Threading.Tasks;

namespace CryptoAccouting.UIClass
{
    public class CoinTableSource : UITableViewSource
    {
        Balance myBalance;
        Balance balanceByCoin;
        //List<Instrument> coins;
        NSString cellIdentifier = new NSString("CoinViewCell");
        CryptoTableViewController owner;

        public CoinTableSource(Balance mybalance, CryptoTableViewController owner)
        {
            this.myBalance = mybalance;
            this.balanceByCoin = mybalance.BalanceByCoin;
            this.owner = owner;
            //this.coins = myBalance is null ? new List<Instrument>() : myBalance.Select(x => x.Coin).Distinct().ToList();
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return balanceByCoin.Count();
            //return coins.Count();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {

            // OLD method
            //var cell = tableView.DequeueReusableCell(cellIdentifier) as CustomBalanceCell;
            //if (cell == null)
            //cell = new CustomBalanceCell (cellIdentifier);

            var cell = (CoinViewCell)tableView.DequeueReusableCell(cellIdentifier, indexPath);
            cell.UpdateCell(balanceByCoin.GetByIndex(indexPath.Row));

            return cell;
        }

        public async override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            cell.BackgroundColor = UIColor.FromRGBA(242,216,223,60);
            await Task.Delay(80);
            cell.BackgroundColor = UIColor.Clear;
        }

        public Position GetItem(int id)
        {
            //return coins[id].TotalPosition();
            return balanceByCoin.GetByIndex(id);
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
            //         UIAlertController okAlertController = UIAlertController.Create("Row Selected", balanceViewItems.GetPositionByIndex(indexPath.Row).Coin.Name, UIAlertControllerStyle.Alert);
            //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            //owner.PresentViewController(okAlertController, true, null);

            owner.PerformSegue("PositionDetailsSegue", owner);
            tableView.DeselectRow(indexPath, true);
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
        {
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
					//tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
					//myBalance.DetachPositionByCoin(coins[indexPath.Row].Symbol);
                    myBalance.DetachPositionByCoin(balanceByCoin.GetByIndex(indexPath.Row).Coin.Symbol1);
                    myBalance.ReCalculate();
                    break;

                case UITableViewCellEditingStyle.None:
                    Console.WriteLine("CommitEditingStyle:None called");
                    break;
            }

            ApplicationCore.SaveMyBalanceXML();
            owner.ReDrawScreen();
            owner.CellItemUpdated(EnuPopTo.None);
        }

		public static UIView BuidBlanceViewHeader(UITableView tv)
		{

			UILabel codeLabel, amountLabel, priceLabel, pctLabel;

			UIView view = new UIView(new System.Drawing.RectangleF(0, 0, (float)tv.Frame.Width, 20));
			view.BackgroundColor = UIColor.Gray;
            var width = (float)tv.Frame.Width / 4;

            codeLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Frame = new System.Drawing.RectangleF(20, 0, 90, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Coin"
            };

            amountLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Frame = new System.Drawing.RectangleF(width + 20, 0, 75, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Holding"
            };


            priceLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Frame = new System.Drawing.RectangleF((width * 2) + 20, 0, 75, 20),
                BackgroundColor = UIColor.Clear,
                Text = "Price/Value"
            };

            pctLabel = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Frame = new System.Drawing.RectangleF((float)tv.Frame.Width - 60, 0, 60, 20),
                BackgroundColor = UIColor.Clear,
                Text = "24hr Chg"
            };

			view.AddSubviews(new UIView[] { codeLabel, amountLabel, priceLabel, pctLabel });
			return view;
		}

    }


}