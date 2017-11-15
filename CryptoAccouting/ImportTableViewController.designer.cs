// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CoinBalance
{
	[Register ("ImportTableViewController")]
	partial class ImportTableViewController
	{
		[Outlet]
		UIKit.UIButton buttonExchange { get; set; }

		[Outlet]
		UIKit.UIButton buttonImport { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (buttonExchange != null) {
				buttonExchange.Dispose ();
				buttonExchange = null;
			}

			if (buttonImport != null) {
				buttonImport.Dispose ();
				buttonImport = null;
			}
		}
	}
}
