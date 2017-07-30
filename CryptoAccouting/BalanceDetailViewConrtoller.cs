using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;
using CryptoAccouting.UIClass;
using System.Threading.Tasks;

namespace CryptoAccouting
{
    public partial class BalanceDetailViewConrtoller : UIViewController
    {
		private Balance myBalance;

        public BalanceDetailViewConrtoller (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {

			myBalance = ApplicationCore.GetMyBalance();

			// Configure Table source
			DetailTableView.RegisterNibForCellReuse(BalanceViewCell.Nib, "BalanceViewCell");
			//DetailTableView.Source = new BalanceTableSource(myBalance, this);

		}


		private void CreateNewPosition()
		{
			var SymbolSelectionViewC = Storyboard.InstantiateViewController("SymbolSelectionViewC") as SymbolSelectionViewConroller;
			NavigationController.PushViewController(SymbolSelectionViewC, true);

		}

		public void SaveItem(Position pos)
		{
			myBalance.AttachPosition(pos);
			DetailTableView.ReloadData();
			ApplicationCore.SaveMyBalance(myBalance);
			//NavigationController.PopViewController(true);
			NavigationController.PopToRootViewController(true);
		}


    }
}