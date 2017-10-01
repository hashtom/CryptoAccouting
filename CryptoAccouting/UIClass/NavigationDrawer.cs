//using System;
//using Foundation;
//using UIKit;
//using Syncfusion.SfNavigationDrawer.iOS;
//using CoreGraphics;

//namespace CryptoAccouting.UIClass
//{

//    public class NavigationDrawer
//    {

//        SFNavigationDrawer navigation;
//        UITableViewController posViewC, plViewC;
//        UIViewController transViewC, perfViewC, settingViewC;

//        public SfNavigationDrawer Navigation
//        {
//            get { return navigation; }
//        }

//        public NavigationDrawer(nfloat Viewwidth, nfloat Viewheight, 
//                                UITableViewController PositionViewC,
//                                UIViewController TransactionViewC,
//                                UITableViewController PLViewC,
//                                UIViewController PerfViewC,
//                                UIViewController SettingViewC)
//        {
//            navigation = new SFNavigationDrawer();
//            this.posViewC = PositionViewC;
//            this.transViewC = TransactionViewC;
//            this.plViewC = PLViewC;
//            this.perfViewC = PerfViewC;
//            this.settingViewC = SettingViewC;

//            navigation.Frame = new CGRect(0, 0, 100, Viewheight);//Thisview.Frame.Width, Thisview.Frame.Height);
//			navigation.DrawerHeaderHeight = 100;
//            navigation.DrawerWidth = 150;

//            //UIView contentView = new UIView(); //new CGRect(0, 0, Thisview.Frame.Width, Thisview.Frame.Height));
//            UIView contentView = new UIView(new CGRect(0, 0, navigation.DrawerWidth, 300));
//            navigation.ContentView = contentView;
//            navigation.Transition = SFNavigationDrawerTransition.SFNavigationDrawerTransitionPush;
//        }

//        public void AddView(UIView thisview){


//			//DrawerView
//			//UIView HeaderView = new UIView(new CGRect(0, 0, 70, 100));

//			UIView innerHeaderView = new UIView();
//			innerHeaderView.Frame = new CGRect(0, 0, navigation.DrawerWidth, 100);
//            innerHeaderView.BackgroundColor = UIColor.White;

//			UILabel appnameLabel = new UILabel();
//			appnameLabel.Frame = new CGRect(0, 70, navigation.DrawerWidth, 30);
//			appnameLabel.Text = (NSString)"暗号通貨手帳";
//            appnameLabel.TextColor = UIColor.Black;
//			appnameLabel.TextAlignment = UITextAlignment.Center;
//			innerHeaderView.AddSubview(appnameLabel);
//			UIImageView userImg = new UIImageView();
//			userImg.Frame = new CGRect((navigation.DrawerWidth / 2) - 25, 15, 50, 50);
//			userImg.Image = new UIImage("Images/btc.png");
//			innerHeaderView.AddSubview(userImg);
//			//HeaderView.AddSubview(innerHeaderView);

//			UIView drawerContentView = new UIView(new CGRect(0, 0, navigation.DrawerWidth, 200));
//			UIView centerview = new UIView();
//            centerview.Frame = new CGRect(0, 0, drawerContentView.Frame.Width, drawerContentView.Frame.Height);

//			UIButton posButton = new UIButton(new CGRect(0, 0, navigation.DrawerWidth, 25));
//			posButton.SetTitle("ポジション", UIControlState.Normal);
//			posButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
//			posButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
//			posButton.Layer.CornerRadius = 0;
//			posButton.Layer.BorderWidth = 1;
//			posButton.Layer.BorderColor = UIColor.FromRGB(231, 231, 235).CGColor;
//			posButton.Layer.BackgroundColor = UIColor.White.CGColor;
//			posButton.TouchUpInside += (sender, e) =>
//			{
//                //posViewC.NavigationController.PushViewController(posViewC, true);
//				navigation.ToggleDrawer();
//			};

//			UIButton TxButton = new UIButton(new CGRect(0, 25, navigation.DrawerWidth, 25));
//			TxButton.SetTitle("取引履歴", UIControlState.Normal);
//			TxButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
//			TxButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
//			TxButton.Layer.CornerRadius = 0;
//			TxButton.Layer.BorderWidth = 1;
//			TxButton.Layer.BorderColor = UIColor.FromRGB(231, 231, 235).CGColor;
//            TxButton.Layer.BackgroundColor = UIColor.White.CGColor;
//			TxButton.TouchUpInside += (sender, e) =>
//			{
//                posViewC.NavigationController.PushViewController(transViewC, true);
//                //posViewC.NavigationController.PushViewController(settingViewC, true);
//				navigation.ToggleDrawer();
//			};

//			UIButton plButton = new UIButton(new CGRect(0, 50, navigation.DrawerWidth, 25));
//			plButton.SetTitle("実現損益", UIControlState.Normal);
//			plButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
//			plButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
//			plButton.Layer.CornerRadius = 0;
//			plButton.Layer.BorderWidth = 1;
//			plButton.Layer.BorderColor = UIColor.FromRGB(231, 231, 235).CGColor;
//			plButton.Layer.BackgroundColor = UIColor.White.CGColor;
//			plButton.TouchUpInside += (sender, e) =>
//			{
//                posViewC.NavigationController.PushViewController(plViewC, true);
//				navigation.ToggleDrawer();
//			};

//			UIButton pfButton = new UIButton(new CGRect(0, 75, navigation.DrawerWidth, 25));
//			pfButton.SetTitle("収益率", UIControlState.Normal);
//			pfButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
//			pfButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
//			pfButton.Layer.CornerRadius = 0;
//			pfButton.Layer.BorderWidth = 1;
//			pfButton.Layer.BorderColor = UIColor.FromRGB(231, 231, 235).CGColor;
//            pfButton.Layer.BackgroundColor = UIColor.White.CGColor;
//            pfButton.TouchUpInside += (sender, e) =>
//            {
//                posViewC.NavigationController.PushViewController(perfViewC, true);
//                navigation.ToggleDrawer();
//            };

//			UIButton setButton = new UIButton(new CGRect(0, 100, navigation.DrawerWidth, 25));
//			setButton.SetTitle("設定", UIControlState.Normal);
//			setButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
//			setButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
//			setButton.Layer.CornerRadius = 0;
//			setButton.Layer.BorderWidth = 1;
//			setButton.Layer.BorderColor = UIColor.FromRGB(231, 231, 235).CGColor;
//            setButton.Layer.BackgroundColor = UIColor.White.CGColor;
//            setButton.TouchUpInside += (sender, e) =>
//            {
//                posViewC.NavigationController.PushViewController(settingViewC, true);
//                navigation.ToggleDrawer();
//            };

//			UIButton aboutButton = new UIButton(new CGRect(0, 125, navigation.DrawerWidth, 25));
//			aboutButton.SetTitle("About", UIControlState.Normal);
//			aboutButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
//			aboutButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
//			aboutButton.Layer.CornerRadius = 0;
//			aboutButton.Layer.BorderWidth = 1;
//			aboutButton.Layer.BorderColor = UIColor.FromRGB(231, 231, 235).CGColor;
//			aboutButton.Layer.BackgroundColor = UIColor.White.CGColor;
//			aboutButton.TouchUpInside += (sender, e) =>
//			{
//				posViewC.NavigationController.PushViewController(settingViewC, true);
//				navigation.ToggleDrawer();
//			};
			
//            centerview.AddSubview(posButton);
//            centerview.AddSubview(TxButton);
//            centerview.AddSubview(plButton);
//            centerview.AddSubview(pfButton);
//            centerview.AddSubview(setButton);
//            centerview.AddSubview(aboutButton);
//			drawerContentView.AddSubview(centerview);

//			navigation.DrawerContentView = drawerContentView;
//            navigation.DrawerHeaderView = innerHeaderView; //HeaderView;
//			thisview.AddSubview(navigation);
//            //navigation.SendSubviewToBack(drawerContentView);
//        }

//        public SFNavigationDrawer GetNavigation(){
//            return navigation;
//        }

//    }
//}
