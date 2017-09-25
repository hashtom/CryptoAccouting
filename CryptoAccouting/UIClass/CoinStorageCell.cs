using System;
using Foundation;
using UIKit;
using CryptoAccouting.CoreClass;

namespace CryptoAccouting.UIClass
{
    public partial class CoinStorageCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("CoinStorageCell");
        public static readonly UINib Nib;

        static CoinStorageCell()
        {
            Nib = UINib.FromName("CoinStorageCell", NSBundle.MainBundle);
        }

        protected CoinStorageCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

		public void UpdateCell(CoinStorage storage)
		{
			labelName.Text = storage.Name;
			labelHoldingBTC.Text = "฿" + ApplicationCore.NumberFormat(storage.AmountBTC());
			labelValueK.Text = ApplicationCore.NumberFormat(storage.LatestFiatValueBase());
			labelRet.Text = String.Format("{0:n2}", storage.Weight * 100) + "%";
		}
    }
}
