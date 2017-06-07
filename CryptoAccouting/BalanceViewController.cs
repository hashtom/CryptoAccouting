using Foundation;
using System;
using UIKit;
using System.Collections.Generic;

namespace CryptoAccouting
{
    public partial class BalanceViewController : UITableViewController
    {

        List<Position> positions;

        public BalanceViewController(IntPtr handle) : base(handle)
        {
            positions = new List<Position> {
                new Position(new AssetAttribute{Code="BTC"}) {Amount=1000, ClosePrice=2800, CcyPrice="BTC"},
                new Position (new AssetAttribute{Code = "REP"}){Amount=5000, ClosePrice=0.0013, CcyPrice="BTC"}
			};
        }

        public override void ViewDidLoad()
        {
            
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			TableView.Source = new BalanceTableSource(positions.ToArray());
		}
    }
}