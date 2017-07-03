using Foundation;
using System;
using UIKit;

namespace CryptoAccouting
{
    public partial class TransactionViewController : UITableViewController
    {

        private Transactions myTransactions;

        public TransactionViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{

		}

		public async override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			//TableView.Source = new BalanceTableSource(myBalance);

			//test
			//var exg = new ExchangeAPI();

			//var p = await exg.FetchPriceAsync(EnuExchangeType.Zaif);
			//labelTotalAsset.Text = p.LatestPrice.ToString();//test

			//var txs = await exg.FetchTransaction((EnuExchangeType.Zaif));

			//labelCurrency.Text = AppConfig.BaseCurrency;


		}
    }
}