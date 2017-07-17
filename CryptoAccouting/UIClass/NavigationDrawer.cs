using System;
using Foundation;
using UIKit;
using Syncfusion.SfNavigationDrawer.iOS;
using CoreGraphics;

namespace CryptoAccouting.UIClass
{

    public class NavigationDrawer
    {

        SFNavigationDrawer navigation;
        UITableViewController posViewC;
        UIViewController transViewC, settingViewC;

        public SfNavigationDrawer Navigation
        {
            get { return navigation; }
        }

        public NavigationDrawer(UITableViewController PositionViewC, UIViewController TransactionViewC, UIViewController SettingViewC)
        {
            navigation = new SFNavigationDrawer();
            this.posViewC = PositionViewC;
            this.transViewC = TransactionViewC;
            this.settingViewC = SettingViewC;

            navigation.Frame = new CGRect(0, 0, 200, 400);//Thisview.Frame.Width, Thisview.Frame.Height);
			navigation.DrawerHeaderHeight = 100;
            navigation.DrawerWidth = 150;

            //UIView contentView = new UIView(); //new CGRect(0, 0, Thisview.Frame.Width, Thisview.Frame.Height));
            UIView contentView = new UIView(new CGRect(0, 0, navigation.DrawerWidth, 300));
            navigation.ContentView = contentView;
        }

        public void AddView(UIView thisview){


			//DrawerView
			//UIView HeaderView = new UIView(new CGRect(0, 0, 70, 100));

			UIView innerHeaderView = new UIView();
			innerHeaderView.Frame = new CGRect(0, 0, navigation.DrawerWidth, 100);
            innerHeaderView.BackgroundColor = UIColor.White;

			UILabel appnameLabel = new UILabel();
			appnameLabel.Frame = new CGRect(0, 70, navigation.DrawerWidth, 30);
			appnameLabel.Text = (NSString)"暗号通貨手帳";
            appnameLabel.TextColor = UIColor.Black;
			appnameLabel.TextAlignment = UITextAlignment.Center;
			innerHeaderView.AddSubview(appnameLabel);
			UIImageView userImg = new UIImageView();
			userImg.Frame = new CGRect((navigation.DrawerWidth / 2) - 25, 15, 50, 50);
			userImg.Image = new UIImage("Images/btc.png");
			innerHeaderView.AddSubview(userImg);
			//HeaderView.AddSubview(innerHeaderView);

			UIView drawerContentView = new UIView(new CGRect(0, 0, navigation.DrawerWidth, 200));
			UIView centerview = new UIView();
            centerview.Frame = new CGRect(0, 0, drawerContentView.Frame.Width, drawerContentView.Frame.Height);

			UIButton TxButton = new UIButton(new CGRect(0, 0, navigation.DrawerWidth, 30));
			TxButton.SetTitle("取引履歴", UIControlState.Normal);
			TxButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
			TxButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			TxButton.Layer.CornerRadius = 0;
			TxButton.Layer.BorderWidth = 1;
			TxButton.Layer.BorderColor = UIColor.FromRGB(231, 231, 235).CGColor;
            TxButton.Layer.BackgroundColor = UIColor.White.CGColor;
			TxButton.TouchUpInside += (sender, e) =>
			{
                posViewC.NavigationController.PushViewController(transViewC, true);
                //posViewC.NavigationController.PushViewController(settingViewC, true);
				navigation.ToggleDrawer();
			};

			UIButton settingButton = new UIButton(new CGRect(0, 30, navigation.DrawerWidth, 30));
			settingButton.SetTitle("設定", UIControlState.Normal);
			settingButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
			settingButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			settingButton.Layer.CornerRadius = 0;
			settingButton.Layer.BorderWidth = 1;
			settingButton.Layer.BorderColor = UIColor.FromRGB(231, 231, 235).CGColor;
            settingButton.Layer.BackgroundColor = UIColor.White.CGColor;
            settingButton.TouchUpInside += (sender, e) =>
            {
                posViewC.NavigationController.PushViewController(settingViewC, true);
                navigation.ToggleDrawer();
            };

			UIButton aboutButton = new UIButton(new CGRect(0, 60, navigation.DrawerWidth, 30));
			aboutButton.SetTitle("About", UIControlState.Normal);
			aboutButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
			aboutButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			aboutButton.Layer.CornerRadius = 0;
			aboutButton.Layer.BorderWidth = 1;
			aboutButton.Layer.BorderColor = UIColor.FromRGB(231, 231, 235).CGColor;
            aboutButton.Layer.BackgroundColor = UIColor.White.CGColor;
            aboutButton.TouchUpInside += (sender, e) =>
            {
                posViewC.NavigationController.PushViewController(settingViewC, true);
                navigation.ToggleDrawer();
            };
			

            centerview.AddSubview(TxButton);
            centerview.AddSubview(settingButton);
            centerview.AddSubview(aboutButton);
			drawerContentView.AddSubview(centerview);

			navigation.DrawerContentView = drawerContentView;
            navigation.DrawerHeaderView = innerHeaderView; //HeaderView;
			thisview.AddSubview(navigation);
        }

        public SFNavigationDrawer GetNavigation(){
            return navigation;
        }

    }
}
