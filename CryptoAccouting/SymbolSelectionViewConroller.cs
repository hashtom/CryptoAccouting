using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;
using CryptoAccouting.UIClass;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

namespace CryptoAccouting
{
    public partial class SymbolSelectionViewConroller : UITableViewController
    {
        public SymbolSelectionViewConroller (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.Source = new TableSource(ApplicationCore.GetInstrumentAll(), this);
            this.NavigationItem.HidesBackButton = true;
        }

    }

    class TableSource : UITableViewSource
	{
        UITableViewController owner;
        List<Instrument> instruments;
		string CellIdentifier = "SymbolListCell";

		public TableSource(List<Instrument> instruments, UITableViewController owner)
		{
            this.instruments = instruments;
            this.owner = owner;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
            return instruments.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(CellIdentifier, indexPath);
			cell.TextLabel.Text = instruments[indexPath.Row].Symbol;
            cell.DetailTextLabel.Text = instruments[indexPath.Row].Name;
			return cell;
		}

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //UIAlertController okAlertController = UIAlertController.Create("Row Selected", instruments[indexPath.Row].Symbol, UIAlertControllerStyle.Alert);
            //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            //owner.PresentViewController(okAlertController, true, null);

            var BalanceEditViewC = owner.Storyboard.InstantiateViewController("BalanceEditViewC") as BalanceEditViewController;
            BalanceEditViewC.CoinSelected(instruments[indexPath.Row].Symbol);
            owner.NavigationController.PushViewController(BalanceEditViewC, true);

		}
	}
}