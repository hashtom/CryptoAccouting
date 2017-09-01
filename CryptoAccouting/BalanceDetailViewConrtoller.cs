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

        string instrumentId_selected;
        //List<Position> booking_positions;

        public BalanceDetailViewConrtoller (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			// Configure Table source
			this.TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "BookingViewCell");
            this.TableView.Source = new CoinBookingTableSource(instrumentId_selected, ApplicationCore.Balance, this);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ReDrawScreen();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

        }

        public override void ReDrawScreen()
        {
            var booking_positions = ApplicationCore.Balance.Where(x => x.Coin.Id == instrumentId_selected).ToList();
            var thisCoin = ApplicationCore.InstrumentList.GetByInstrumentId(instrumentId_selected);
            var logo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                    "Images", thisCoin.Id + ".png");

            imageCoin.Image = logo == null ? null : UIImage.FromFile(logo);
            NavigationItem.Title = thisCoin.Name;
            buttonPriceSourceExchange.SetTitle(thisCoin.PriceSourceCode, UIControlState.Normal);

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

            labelMarketValue.Text = "$" + String.Format("{0:n0}", booking_positions.Sum(x => x.LatestFiatValueUSD()));
            labelTotalQty.Text = String.Format("{0:n0}", booking_positions.Sum(x => x.Amount));
            labelTotalBookCost.Text = "$" + String.Format("{0:n0}", booking_positions.Sum(x => x.BookValue()));

            TableView.ReloadData();

        }

        public void SetInstrument(string instrumentId)
        {
            instrumentId_selected = instrumentId;
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