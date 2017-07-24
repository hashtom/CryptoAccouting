using Foundation;
using System;
using UIKit;

namespace CryptoAccouting
{
    public partial class SettingTableViewController : UITableViewController
    {
        public SettingTableViewController (IntPtr handle) : base (handle)
        {
        }

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);

			if (segue.Identifier == "SettingDetailSegue")
			{
                var navctlr = segue.DestinationViewController as SettingTableViewController;
				if (navctlr != null)
				{

				}
			}
		}
    }
}