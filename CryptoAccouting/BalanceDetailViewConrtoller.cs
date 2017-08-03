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
    public partial class BalanceDetailViewConrtoller : BalanceTableViewController
    {

        string symbol_selected;
        //List<Position> booking_positions;

        public BalanceDetailViewConrtoller (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			// Configure Table source
			this.TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "BookingViewCell");
			this.TableView.Source = new BookingTableSource(symbol_selected,this);
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

        private void ReDrawScreen()
        {
            var booking_positions = ApplicationCore.Balance.positions.Where(x => x.Coin.Symbol == symbol_selected).ToList();
            var thisCoin = ApplicationCore.GetInstrument(symbol_selected);
            var logo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                    "Images", thisCoin.Id + ".png");

            imageCoin.Image = logo == null ? null : UIImage.FromFile(logo);
            labelCoinName.Text = thisCoin.Name;

            if (thisCoin.MarketPrice != null)
            {
                labelBTCPrice.Text = thisCoin.Symbol == "BTC" ?
                    "" :
                    "฿" + String.Format("{0:n8}", thisCoin.MarketPrice.LatestPriceBTC);
                labelFiatPrice.Text = thisCoin.Symbol == "BTC" ?
                    "$" + String.Format("{0:n2}", thisCoin.MarketPrice.LatestPrice) :
                    "$" + String.Format("{0:n6}", thisCoin.MarketPrice.LatestPriceBTC);
                labelFiatRet1d.Text = String.Format("{0:n2}", thisCoin.MarketPrice.SourceRet1d()) + "%";
                labelBTCRet1d.Text = thisCoin.Symbol == "BTC" ? "" : String.Format("{0:n2}", thisCoin.MarketPrice.BTCRet1d()) + "%";
                labelVolume.Text = String.Format("{0:n0}", thisCoin.MarketPrice.DayVolume);
                labelMarketCap.Text = "$" + String.Format("{0:n0}", thisCoin.MarketPrice.MarketCap);
            }

            labelMarketValue.Text = "$" + String.Format("{0:n0}", booking_positions.Sum(x => x.LatestFiatValue()));
            labelTotalQty.Text = String.Format("{0:n0}", booking_positions.Sum(x => x.AmountBTC()));
            labelTotalBookCost.Text = "$" + String.Format("{0:n0}", booking_positions.Sum(x => x.BookValue()));

            TableView.ReloadData();

        }

        public void SetSymbol(string symbol)
        {
            symbol_selected = symbol;
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
					var source = TableView.Source as BookingTableSource;
					var rowPath = TableView.IndexPathForSelectedRow;
					var item = source.GetItem(rowPath.Row);
                    navctlr.SetPosition(item);
				}
			}

		}
    }
}