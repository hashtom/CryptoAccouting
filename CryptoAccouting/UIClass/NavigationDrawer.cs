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
			navigation.DrawerWidth = 200;

            UIView contentView = new UIView(new CGRect(0, 0, thisview.Frame.Width, thisview.Frame.Height));

            //UILabel homeLabel = new UILabel();
            //homeLabel.Frame = new CGRect(15, 2, contentView.Frame.Width - 20, 30);
            //homeLabel.Text = "Home";
            //homeLabel.TextColor = UIColor.White;
            //homeLabel.BackgroundColor = UIColor.FromRGB(49, 173, 225);
            //homeLabel.TextAlignment = UITextAlignment.Center;
            //contentView.AddSubview(homeLabel);

            navigation.ContentView = contentView;
              

        }

        public void AddView(UIView thisview){


			//DrawerView
			UIView headerView = new UIView(new CGRect(0, 0, 100, 100));

			UIView HeaderView = new UIView();
			HeaderView.Frame = new CGRect(0, 0, navigation.DrawerWidth, 100);
			HeaderView.BackgroundColor = UIColor.FromRGB(49, 173, 225);
			UILabel usernameLabel = new UILabel();
			usernameLabel.Frame = new CGRect(0, 70, navigation.DrawerWidth, 30);
			usernameLabel.Text = (NSString)"James Pollock";
			usernameLabel.TextColor = UIColor.White;
			usernameLabel.TextAlignment = UITextAlignment.Center;
			HeaderView.AddSubview(usernameLabel);
			UIImageView userImg = new UIImageView();
			userImg.Frame = new CGRect((navigation.DrawerWidth / 2) - 25, 15, 50, 50);
			userImg.Image = new UIImage("Images/btc.png");
			HeaderView.AddSubview(userImg);
			headerView.AddSubview(HeaderView);

			UIView drawerContentView = new UIView(new CGRect(0, 0, navigation.DrawerWidth, 100));

			UIView centerview = new UIView();
			centerview.Frame = new CGRect(0, 0, navigation.DrawerWidth, 500);

			UIButton homeButton = new UIButton(new CGRect(0, 0, navigation.DrawerWidth, 50));
			homeButton.SetTitle("Home", UIControlState.Normal);
			homeButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
			homeButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			homeButton.Layer.CornerRadius = 0;
			homeButton.Layer.BorderWidth = 1;
			homeButton.Layer.BorderColor = UIColor.FromRGB(0, 0, 0).CGColor;
			centerview.AddSubview(homeButton);

			UIButton profileButton = new UIButton(new CGRect(0, 50, navigation.DrawerWidth, 50));
			profileButton.SetTitle("Profile", UIControlState.Normal);
			profileButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
			profileButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			profileButton.Layer.CornerRadius = 0;
			profileButton.Layer.BorderWidth = 1;
			profileButton.Layer.BorderColor = UIColor.FromRGB(0, 0, 0).CGColor;
			centerview.AddSubview(profileButton);

			UIButton inboxButton = new UIButton(new CGRect(0, 100, navigation.DrawerWidth, 50));
			inboxButton.SetTitle("Inbox", UIControlState.Normal);
			inboxButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
			inboxButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			inboxButton.Layer.CornerRadius = 0;
			inboxButton.Layer.BorderWidth = 1;
			inboxButton.Layer.BorderColor = UIColor.FromRGB(0, 0, 0).CGColor;
			centerview.AddSubview(inboxButton);

			UIButton outboxButton = new UIButton(new CGRect(0, 150, navigation.DrawerWidth, 50));
			outboxButton.SetTitle("Outbox", UIControlState.Normal);
			outboxButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
			outboxButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			outboxButton.Layer.CornerRadius = 0;
			outboxButton.Layer.BorderWidth = 1;
			outboxButton.Layer.BorderColor = UIColor.FromRGB(0, 0, 0).CGColor;
			centerview.AddSubview(outboxButton);

			UIButton sentItemsButton = new UIButton(new CGRect(0, 200, navigation.DrawerWidth, 50));
			sentItemsButton.SetTitle("SentItems", UIControlState.Normal);
			sentItemsButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
			sentItemsButton.Layer.CornerRadius = 0;
			sentItemsButton.Layer.BorderWidth = 1;
			sentItemsButton.Layer.BorderColor = UIColor.FromRGB(0, 0, 0).CGColor;
			centerview.AddSubview(sentItemsButton);

			UIButton trashButton = new UIButton(new CGRect(0, 250, navigation.DrawerWidth, 50));
			trashButton.SetTitle("Trash", UIControlState.Normal);
			trashButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
			trashButton.Layer.CornerRadius = 0;
			trashButton.Layer.BorderWidth = 1;
			trashButton.Layer.BorderColor = UIColor.FromRGB(0, 0, 0).CGColor;
			centerview.AddSubview(trashButton);
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
