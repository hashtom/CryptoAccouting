using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;

namespace CryptoAccouting
{
	public partial class BalanceEditViewController : UITableViewController
	{
		public Position PositionDetail;
		public BalanceViewController AppDel { get; set; }

		public BalanceEditViewController(IntPtr handle) : base(handle)
		{
		}

		public void SetPosition(BalanceViewController d, Position pos)
		{
			AppDel = d;
			PositionDetail = pos;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            labelCoinSymbol.Text = PositionDetail is null ? "New Coin" : PositionDetail.Coin.Name;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
		}

	}
}