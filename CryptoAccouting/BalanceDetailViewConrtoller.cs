using Foundation;
using System;
using System.IO;
using UIKit;
using CoinBalance.CoreModel;
using CoinBalance.UIModel;
using System.Linq;
using System.Collections.Generic;
using CoreGraphics;
using CoreAnimation;

namespace CoinBalance
{
    public partial class BalanceDetailViewConrtoller : CryptoTableViewController
    {
        Instrument thisCoin;
        string instrumentId_selected;
        static readonly NSString MyCellId = new NSString("BookingCell");
        static List<(string, string)> pricesources;

        public BalanceDetailViewConrtoller (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			// Configure Table source
			//this.TableView.RegisterNibForCellReuse(BookingCell.Nib, MyCellId);
			this.TableView.RegisterClassForCellReuse(typeof(CoinBookingCell), MyCellId);
            this.TableView.Source = new CoinBookingTableSource(instrumentId_selected, AppCore.Balance, this);

            //Color Design
            var gradient = new CAGradientLayer();
            gradient.Frame = this.DetailTopView.Bounds;
            gradient.NeedsDisplayOnBoundsChange = true;
            gradient.MasksToBounds = true;
            gradient.Colors = new CGColor[] { UIColor.FromRGB(0, 126, 167).CGColor, UIColor.FromRGB(0, 168, 232).CGColor };
            this.DetailTopView.Layer.InsertSublayer(gradient, 0);

            //Price source list
            pricesources = new List<(string, string)>();
            pricesources.Add(("coinmarketcap", "coinmarketcap"));

            foreach (var ex in AppCore.PublicExchangeList.Where(x => x.HasPriceAPI))
            {
                if (ex.IsListed(thisCoin.Id)) pricesources.Add((ex.Name, ex.Code));
            }

			buttonPriceSource.TouchUpInside += (sender, e) =>
            {
				UIAlertController PriceSourceAlert = UIAlertController.Create("Price Source", "Choose Price Source", UIAlertControllerStyle.ActionSheet);
				PriceSourceAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

                foreach (var source in pricesources)
				{
                    PriceSourceAlert.AddAction(UIAlertAction.Create(source.Item1, UIAlertActionStyle.Default, async (obj) =>
                                                                         {
                                                                             var prevItem = thisCoin.PriceSourceCode;
                                                                             thisCoin.PriceSourceCode = source.Item2;
                                                                             buttonPriceSource.SetTitle("Loaing...", UIControlState.Normal);
                                                                             buttonPriceSource.Enabled = false;
                                                                             try
                                                                             {
                                                                                 await AppCore.FetchMarketDataAsync(thisCoin);
                                                                                 AppCore.SavePriceSourceXML();
                                                                             }
                                                                             catch (Exception ex)
                                                                             {
                                                                                 this.PopUpWarning("Warning", "Unable to update price data. " + ex.Message, null);
                                                                                 System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": ViewDidLoad: buttonPriceSource: " + ex.GetType() + ": " + ex.Message);
                                                                                 thisCoin.PriceSourceCode = prevItem;
                                                                             }
                                                                             finally
                                                                             {
                                                                                 buttonPriceSource.Enabled = true;
                                                                                 ReDrawScreen();
                                                                             }
                                                                         }
                                                                   ));
				}

				this.PresentViewController(PriceSourceAlert, true, null);
			};
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //if (!AppDelegate.IsInDesignerView)
            //{
                ReDrawScreen();
            //}
        }

        public override void ReDrawScreen()
        {
            var booking_positions = AppCore.Balance.Where(x => x.Coin.Id == instrumentId_selected).ToList();
            //var thisCoin = ApplicationCore.InstrumentList.GetByInstrumentId(instrumentId_selected);
            var logo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                    "Images", thisCoin.Id + ".png");

            imageCoin.Image = logo == null ? null : UIImage.FromFile(logo);
            labelName.Text = thisCoin.Name;

            if (pricesources.Any(x => x.Item2 == thisCoin.PriceSourceCode))
            {
                buttonPriceSource.SetTitle(pricesources.Where(x => x.Item2 == thisCoin.PriceSourceCode).Select(x => x.Item1).First(), UIControlState.Normal);
            }
            else
            {
                thisCoin.PriceSourceCode = "coinmarketcap";
                buttonPriceSource.SetTitle(thisCoin.PriceSourceCode, UIControlState.Normal);
            }

            if (booking_positions.Any())
            {
                if (thisCoin.Symbol1 == "BTC")
                {
                    labelPrice.Text = AppCore.NumberFormat(booking_positions.First().LatestPriceUSD, false, true, "$");
                    //labelVolume.Text = ApplicationCore.NumberFormat(booking_positions.First().MarketDayVolume(), false, true, "$");
                }
                else
                {
                    labelPrice.Text = AppCore.NumberFormat(booking_positions.First().LatestPriceBTC, false, true, "฿");
                    //labelVolume.Text = ApplicationCore.NumberFormat(booking_positions.First().MarketDayVolume(), false, true, "฿");
                }

                labelPrice.TextColor = booking_positions.First().USDRet1d > 0 ? UIColor.FromRGB(247, 255, 247) : UIColor.FromRGB(128, 0, 0);
                labelPriceBase.Text = AppCore.NumberFormat(booking_positions.First().LatestPriceBase);
                labelPriceBase.TextColor = booking_positions.First().USDRet1d > 0 ? UIColor.FromRGB(247, 255, 247) : UIColor.FromRGB(128, 0, 0);
                labelPriceBaseTitle.Text = "Price(" + AppCore.BaseCurrency + ")";
                labelVolume.Text = AppCore.NumberFormat(booking_positions.First().MarketDayVolume, false, true, "฿");

                //labelProfitLoss.Text = "$" + ApplicationCore.NumberFormat(booking_positions.Sum(x => x.PLUSD()));
                labelMarketValueTitle.Text = "TotalValue(" + AppCore.BaseCurrency.ToString() + ")";
                labelMarketValue.Text = AppCore.NumberFormat(booking_positions.Sum(x => x.LatestFiatValueBase()));
                labelTotalQty.Text = AppCore.NumberFormat(booking_positions.Sum(x => x.Amount));
                //labelTotalBookCost.Text = "$" + String.Format("{0:n0}", booking_positions.Sum(x => x.BookValueUSD()));
            }

            TableView.ReloadData();

        }

        public void SetInstrument(string instrumentId)
        {
            instrumentId_selected = instrumentId;
            thisCoin = AppCore.InstrumentList.GetByInstrumentId(instrumentId_selected);
            //this.booking_positions = ApplicationCore.Balance.positions.Where(x => x.Coin.Symbol == symbol_selected).ToList();
        }


		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);

			if (segue.Identifier == "PositionEditSegue")
			{
                var navctlr = segue.DestinationViewController as BalanceEditViewController;
				if (navctlr != null)
				{
					var source = TableView.Source as CoinBookingTableSource;
					var rowPath = TableView.IndexPathForSelectedRow;
					var item = source.GetItem(rowPath.Row);
                    navctlr.SetPosition(item, EnuPopTo.OnePop, false);
				}
			}

		}

        partial void ButtonAddNew_Activated(UIBarButtonItem sender)
        {
			var DestinationViewC = Storyboard.InstantiateViewController("BalanceEditViewC") as BalanceEditViewController;
            //DestinationViewC.SetSearchSelectionItem(symbol_selected);
            DestinationViewC.SetPosition(
                new Position(AppCore.InstrumentList.GetByInstrumentId(instrumentId_selected)),
                EnuPopTo.OnePop, true);
            NavigationController.PushViewController(DestinationViewC, true);
        }
    }
}