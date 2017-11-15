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
	[Register ("ExposureViewController")]
	partial class ExposureViewController
	{
		[Outlet]
		UIKit.UIView ExposureDrawingView { get; set; }

		[Outlet]
		UIKit.UIView ExposureGridView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ExposureDrawingView != null) {
				ExposureDrawingView.Dispose ();
				ExposureDrawingView = null;
			}

			if (ExposureGridView != null) {
				ExposureGridView.Dispose ();
				ExposureGridView = null;
			}
		}
	}
}
