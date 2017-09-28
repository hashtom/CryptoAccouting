using Foundation;
using System;
using System.IO;
using UIKit;
using CryptoAccouting.CoreClass;
using CryptoAccouting.UIClass;
//using System.Threading.Tasks;
//using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting
{
    public partial class BalanceDetailViewConrtoller : CryptoTableViewController
    {
        Instrument thisCoin;
        string instrumentId_selected;
        static readonly NSString MyCellId = new NSString("BookingCell");
        //List<Position> booking_positions;

        public BalanceDetailViewConrtoller (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			// Configure Table source
			//this.TableView.RegisterNibForCellReuse(BookingCell.Nib, MyCellId);
			this.TableView.RegisterClassForCellReuse(typeof(CoinBookingCell), MyCellId);
            this.TableView.Source = new CoinBookingTableSource(instrumentId_selected, ApplicationCore.Balance, this);

			buttonPriceSource.TouchUpInside += (sender, e) =>
			{
				string[] sources;

                //todo
                if (thisCoin.Symbol1 == "BTC")
                {
                    sources = new string[] { "Bitstamp", "coinmarketcap" };
                }
                else
                {
                    if (ApplicationCore.GetExchange("Bittrex").IsListed(thisCoin.Id))
                    {
                        sources = new string[] { "Bittrex", "coinmarketcap" };
                    }
                    else
                    {
                        sources = new string[] { "coinmarketcap" };
                    }
                }

				UIAlertController PriceSourceAlert = UIAlertController.Create("Price Source", "Choose Price Source", UIAlertControllerStyle.ActionSheet);
				PriceSourceAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

				foreach (var source in sources)
				{
                    PriceSourceAlert.AddAction(UIAlertAction.Create(source, UIAlertActionStyle.Default,
                                                                         (obj) =>
                                                                         {
                                                                             buttonPriceSource.SetTitle(source, UIControlState.Normal);
                                                                             thisCoin.PriceSourceCode = source;
                                                                             ApplicationCore.SaveInstrumentXML();
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
            var booking_positions = ApplicationCore.Balance.Where(x => x.Coin.Id == instrumentId_selected).ToList();
            //var thisCoin = ApplicationCore.InstrumentList.GetByInstrumentId(instrumentId_selected);
            var logo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                    "Images", thisCoin.Id + ".png");
            
            imageCoin.Image = logo == null ? null : UIImage.FromFile(logo);
            NavigationItem.Title = thisCoin.Name;
            buttonPriceSource.SetTitle(thisCoin.PriceSourceCode, UIControlState.Normal);

            if (thisCoin.MarketPrice != null)
            {
                //labelBTCPrice.Text = thisCoin.Symbol == "BTC" ?
                    //"" :
                    //"฿" + String.Format("{0:n8}", thisCoin.MarketPrice.LatestPriceBTC);
                //labelFiatPrice.Text = thisCoin.Symbol == "BTC" ?
                //    "$" + String.Format("{0:n2}", thisCoin.MarketPrice.LatestPriceUSD) :
                //    "$" + String.Format("{0:n6}", thisCoin.MarketPrice.LatestPriceBTC);
                //labelFiatRet1d.Text = String.Format("{0:n2}", thisCoin.MarketPrice.SourceRet1d()) + "%";
                //labelBTCRet1d.Text = thisCoin.Symbol == "BTC" ? "" : String.Format("{0:n2}", thisCoin.MarketPrice.BTCRet1d()) + "%";
                //labelVolume.Text = String.Format("{0:n0}", thisCoin.MarketPrice.DayVolume);
                //labelMarketCap.Text = "$" + String.Format("{0:n0}", thisCoin.MarketPrice.MarketCap);
            }

            labelProfitLoss.Text = "$" + ApplicationCore.NumberFormat(booking_positions.Sum(x => x.PLUSD()));
            labelMarketValue.Text = "$" + String.Format("{0:n0}", booking_positions.Sum(x => x.LatestFiatValueUSD()));
            labelTotalQty.Text = String.Format("{0:n0}", booking_positions.Sum(x => x.Amount));
            labelTotalBookCost.Text = "$" + String.Format("{0:n0}", booking_positions.Sum(x => x.BookValueUSD()));

            TableView.ReloadData();

        }

        public void SetInstrument(string instrumentId)
        {
            instrumentId_selected = instrumentId;
            thisCoin = ApplicationCore.InstrumentList.GetByInstrumentId(instrumentId_selected);
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
                new Position(ApplicationCore.InstrumentList.GetByInstrumentId(instrumentId_selected)),
                EnuPopTo.OnePop, true);
            NavigationController.PushViewController(DestinationViewC, true);
        }
    }
}