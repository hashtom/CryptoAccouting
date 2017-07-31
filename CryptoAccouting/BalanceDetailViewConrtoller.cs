using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;
using CryptoAccouting.UIClass;
using System.Threading.Tasks;

namespace CryptoAccouting
{
    public partial class BalanceDetailViewConrtoller : BalanceTableViewController
    {

        string symbol_selected;

        public BalanceDetailViewConrtoller (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

			// Configure Table source
			TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "BookingCell");
            TableView.Source = new BookingTableSource(ApplicationCore.Balance, this, symbol_selected);

		}

        public void SetSymbol(string symbol)
        {
            symbol_selected = symbol;
        }
    }
}