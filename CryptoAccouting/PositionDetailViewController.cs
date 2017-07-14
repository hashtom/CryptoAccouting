using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;

namespace CryptoAccouting
{
    public partial class PositionDetailViewController : UIViewController
    {

        public Position PositionDetail;
        public BalanceViewController AppDel { get; set; }

        public PositionDetailViewController(IntPtr handle) : base(handle)
        {
        }


        public void SetItem(BalanceViewController d, Position pos)
        {
            AppDel = d;
            PositionDetail = pos;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            LabelCoinDetail.Text = PositionDetail.Coin.Name;
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