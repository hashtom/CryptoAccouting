using System;
using Foundation;
using UIKit;
using CryptoAccouting.CoreClass;

namespace CryptoAccouting.UIClass
{
    public partial class CoinStorageViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("CoinStorageViewCell");
        public static readonly UINib Nib;

        static CoinStorageViewCell()
        {
            Nib = UINib.FromName("CoinStorageViewCell", NSBundle.MainBundle);
        }

        protected CoinStorageViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public void UpdateCell(Exchange exc)
		{
            
            labelExchange.Text = exc.Name; // pos.BookedExchange.Name;
            //labelHoldingBTC.Text = "฿" + String.Format("{0:n2}", pos.AmountBTC());
            //labelValueK.Text = String.Format("{0:n2}", pos.LatestFiatValueUSD());
            labelRet.Text = "%";
		}
    }
}
