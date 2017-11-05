using System;
using Foundation;
using UIKit;
using System.IO;

namespace CoinBalance
{
	public partial class AboutViewConroller : UIViewController
	{
		public AboutViewConroller (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var version = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
            var build = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"];

            labelVersion.Text = "Ver " + version + " , Build " + build;

            string contentDirectoryPath = Path.Combine(NSBundle.MainBundle.BundlePath, "Content/");
            string html = @"<html><head> Credit & Thanks </head><p>Icons made by<a href= ""https://www.flaticon.com/authors/gregor-cresnar"""
                + @"title=""Gregor Cresnar""> Gregor Cresnar</a> from <a href = ""https://www.flaticon.com/"" title=""Flaticon""> www.flaticon.com </a>"
                + @" is licensed by <a href=""http://creativecommons.org/licenses/by/3.0/"" title=""Creative Commons BY 3.0"" target=""_blank""> CC 3.0 BY </a></html>";
            CreditWebView.LoadHtmlString(html, new NSUrl(contentDirectoryPath, true));
        }

	}
}
