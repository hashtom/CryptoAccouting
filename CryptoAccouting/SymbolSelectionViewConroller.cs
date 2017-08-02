using Foundation;
using System;
using UIKit;
using CryptoAccouting.CoreClass;
using CryptoAccouting.UIClass;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

namespace CryptoAccouting
{
    public partial class SymbolSelectionViewConroller : UITableViewController
    {
        //static NSString MyCellId = new NSString("SymbolListCell");
		ResultsTableController resultsTableController;
		UISearchController searchController;

		bool searchControllerWasActive;
		bool searchControllerSearchFieldWasFirstResponder;
        List<Instrument> instruments;

        public SymbolSelectionViewConroller(IntPtr handle) : base (handle)
        {

            instruments = ApplicationCore.GetInstrumentAll(true);
            NavigationItem.HidesBackButton = true;
            NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (sender, e) =>
             {
                 NavigationController.PopToRootViewController(true);
             }), true);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.Source = new TableSource(instruments, this);

            resultsTableController = new ResultsTableController()
            {
                FilteredInstruments = new List<Instrument>()
            };


			searchController = new UISearchController(resultsTableController)
			{
				WeakDelegate = this,
				DimsBackgroundDuringPresentation = false,
				WeakSearchResultsUpdater = this
			};

			searchController.SearchBar.SizeToFit();
			TableView.TableHeaderView = searchController.SearchBar;

			resultsTableController.TableView.WeakDelegate = this;
			searchController.SearchBar.WeakDelegate = this;

            DefinesPresentationContext = true;

			if (searchControllerWasActive)
			{
				searchController.Active = searchControllerWasActive;
				searchControllerWasActive = false;

				if (searchControllerSearchFieldWasFirstResponder)
				{
					searchController.SearchBar.BecomeFirstResponder();
					searchControllerSearchFieldWasFirstResponder = false;
				}
			}
		}

		[Export("searchBarSearchButtonClicked:")]
		public virtual void SearchButtonClicked(UISearchBar searchBar)
		{
			searchBar.ResignFirstResponder();
		}

		[Export("updateSearchResultsForSearchController:")]
		public virtual void UpdateSearchResultsForSearchController(UISearchController searchController)
		{
			var tableController = (ResultsTableController)searchController.SearchResultsController;
            tableController.FilteredInstruments = PerformSearch(searchController.SearchBar.Text);
            tableController.TableView.Source = new TableSource(tableController.FilteredInstruments, this);
            tableController.TableView.ReloadData();
		}

        List<Instrument> PerformSearch(string searchString)
		{
			searchString = searchString.Trim();
			string[] searchItems = string.IsNullOrEmpty(searchString)
				? new string[0]
				: searchString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var filteredCoins = new List<Instrument>();

			foreach (var item in searchItems)
			{
				//int year = Int32.MinValue;
				//Int32.TryParse(item, out year);

				//double price = Double.MinValue;
				//Double.TryParse(item, out price);

                IEnumerable<Instrument> query =
                    from p in instruments
                        where p.Symbol.IndexOf(item, StringComparison.OrdinalIgnoreCase) >= 0
					//|| p.IntroPrice == price
					//|| p.YearIntroduced == year
					orderby p.Symbol
					select p;

				filteredCoins.AddRange(query);
			}

			return filteredCoins.Distinct().ToList();
		}

    }


    public class ResultsTableController : UITableViewController
    {
        //static NSString MyCellId = new NSString("SymbolListCell");
        public List<Instrument> FilteredInstruments { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "CoinViewCell");
            //TableView.RegisterClassForCellReuse(typeof(UITableViewCell), MyCellId);
            TableView.Source = new TableSource(FilteredInstruments, this);
        }

        //public override void ViewWillAppear(bool animated)
        //{
        //    base.ViewWillAppear(animated);
        //    //TableView.Source = new TableSource(FilteredInstruments, this);
        //}

    }


    class TableSource : UITableViewSource
    {
        UITableViewController owner;
        List<Instrument> instruments;
        string cellIdentifier = "SymbolListCell";

        public TableSource(List<Instrument> instruments, UITableViewController owner)
        {
            this.instruments = instruments.OrderBy(x=>x.rank).ToList();
            this.owner = owner;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return instruments.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            //UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier, indexPath);
            //if (cell == null) 
            var cell = new UITableViewCell(UITableViewCellStyle.Subtitle, cellIdentifier);

            cell.TextLabel.Text = instruments[indexPath.Row].Symbol;
            cell.DetailTextLabel.Text = instruments[indexPath.Row].Name;
            var logo = instruments[indexPath.Row].LogoFileName;
            cell.ImageView.Image = logo == null ? null : UIImage.FromFile(logo);

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var BalanceEditViewC = owner.Storyboard.InstantiateViewController("BalanceEditViewC") as BalanceEditViewController;
            BalanceEditViewC.SetPositionForNewCoin(instruments[indexPath.Row].Symbol);
            owner.NavigationController.PushViewController(BalanceEditViewC, false);
        }

    }

}