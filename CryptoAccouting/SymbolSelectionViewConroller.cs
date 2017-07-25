using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;
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
            TableView.Source = new TableSource(ApplicationCore.GetInstrumentAll());
        }
    }

    class TableSource : UITableViewSource
	{

        List<Instrument> TableItems;
		string CellIdentifier = "SymbolListCell";

		public TableSource(List<Instrument> items)
		{
			TableItems = items;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
            return TableItems.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(CellIdentifier, indexPath);
			cell.TextLabel.Text = TableItems[indexPath.Row].Symbol;
            cell.DetailTextLabel.Text = TableItems[indexPath.Row].Name;
			return cell;
		}
	}
}