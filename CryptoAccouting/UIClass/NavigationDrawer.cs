using System;
using Foundation;
using UIKit;
using Syncfusion.SfNavigationDrawer.iOS;
using CoreGraphics;

namespace CryptoAccouting.UIClass
{

    public class NavigationDrawer
    {

        private SFNavigationDrawer navigation;
        public SfNavigationDrawer Navigation
        {
            get { return navigation; }
        }

        public NavigationDrawer(UIView thisview)
        {
            navigation = new SFNavigationDrawer();

			navigation.Frame = new CGRect(0, 0, thisview.Frame.Width, thisview.Frame.Height);
			navigation.DrawerHeaderHeight = 100;
			navigation.DrawerWidth = 150;

            UIView contentView = new UIView(new CGRect(0, 0, thisview.Frame.Width, thisview.Frame.Height));
            //UIView contentView = new UIView(new CGRect(0, 0, 150, 160));
            navigation.ContentView = contentView;
        }

        public void AddView(UIView thisview){


			//DrawerView
			UIView headerView = new UIView(new CGRect(0, 0, 70, 100));

			UIView HeaderView = new UIView();
			HeaderView.Frame = new CGRect(0, 0, navigation.DrawerWidth, 100);
			HeaderView.BackgroundColor = UIColor.FromRGB(49, 173, 225);

			UILabel appnameLabel = new UILabel();
			appnameLabel.Frame = new CGRect(0, 70, navigation.DrawerWidth, 30);
			appnameLabel.Text = (NSString)"暗号通貨手帳";
			appnameLabel.TextColor = UIColor.White;
			appnameLabel.TextAlignment = UITextAlignment.Center;
			HeaderView.AddSubview(appnameLabel);
			UIImageView userImg = new UIImageView();
			userImg.Frame = new CGRect((navigation.DrawerWidth / 2) - 25, 15, 50, 50);
			userImg.Image = new UIImage("Images/btc.png");
			HeaderView.AddSubview(userImg);
			headerView.AddSubview(HeaderView);

			UIView drawerContentView = new UIView(new CGRect(0, 0, navigation.DrawerWidth, 30));

			UIView centerview = new UIView();
			centerview.Frame = new CGRect(0, 0, navigation.DrawerWidth, 30);

			UIButton settingButton = new UIButton(new CGRect(0, 0, navigation.DrawerWidth, 30));
			settingButton.SetTitle("Setting", UIControlState.Normal);
			settingButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
			settingButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			settingButton.Layer.CornerRadius = 0;
			settingButton.Layer.BorderWidth = 1;
			settingButton.Layer.BorderColor = UIColor.FromRGB(0, 0, 0).CGColor;
			centerview.AddSubview(settingButton);

			UIButton aboutButton = new UIButton(new CGRect(0, 30, navigation.DrawerWidth, 30));
			aboutButton.SetTitle("About", UIControlState.Normal);
			aboutButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
			aboutButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			aboutButton.Layer.CornerRadius = 0;
			aboutButton.Layer.BorderWidth = 1;
			aboutButton.Layer.BorderColor = UIColor.FromRGB(0, 0, 0).CGColor;
			centerview.AddSubview(aboutButton);

			drawerContentView.AddSubview(centerview);

			navigation.DrawerContentView = drawerContentView;
			navigation.DrawerHeaderView = headerView;
			thisview.AddSubview(navigation);
        }

        public SFNavigationDrawer GetNavigation(){
            return navigation;
        }

    }
}
