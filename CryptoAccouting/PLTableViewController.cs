using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;

namespace CryptoAccouting
{
    public partial class PLTableViewController : UITableViewController
    {

        public PLTableViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

        }


        partial void ButtonPickExchange_TouchUpInside(UIButton sender)
        {

        }

        partial void ButtonFiatCurrency_TouchUpInside(UIButton sender)
        {
            
        }

        partial void ButtonCryptCurrency_TouchUpInside(UIButton sender)
        {
           
        }
    }

}