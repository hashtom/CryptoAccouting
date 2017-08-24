using System;

using Foundation;
using UIKit;

namespace CryptoAccouting.UIClass
{
    public partial class CoinBookingViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("CoinBookingViewCell");
        public static readonly UINib Nib;

        static CoinBookingViewCell()
        {
            Nib = UINib.FromName("CoinBookingViewCell", NSBundle.MainBundle);
        }

        protected CoinBookingViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
