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
        ResultsTableController searchResultsController;

        List<Instrument> instruments;

        public SymbolSelectionViewConroller(IntPtr handle) : base(handle)
        {
            instruments = ApplicationCore.GetInstrumentAll(true);
            NavigationItem.HidesBackButton = true;
            NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (sender, e) =>
             {
                 NavigationController.PopToRootViewController(true);
             }
                                                                    ), true);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.Source = new TableSource(instruments, this);
            //this.NavigationItem.HidesBackButton = true;

            searchResultsController = new ResultsTableController(Handle);
            searchResultsController.FilteredInstruments = instruments;

			var searchUpdater = new SearchResultsUpdator();
			searchUpdater.UpdateSearchResults += searchResultsController.Search;

			//add the search controller
			var searchController = new UISearchController(searchResultsController)
			{
				SearchResultsUpdater = searchUpdater
			};

			//format the search bar
			searchController.SearchBar.SizeToFit();
			searchController.SearchBar.SearchBarStyle = UISearchBarStyle.Minimal;
			searchController.SearchBar.Placeholder = "Enter a search query";

			//the search bar is contained in the navigation bar, so it should be visible
			searchController.HidesNavigationBarDuringPresentation = false;

			//Ensure the searchResultsController is presented in the current View Controller 
			DefinesPresentationContext = true;

			//Set the search bar
			//NavigationItem.TitleView = searchController.SearchBar;
            TableView.TableHeaderView = searchController.SearchBar;
		}

    }

    public class ResultsTableController : SymbolSelectionViewConroller
    {
        public List<Instrument> FilteredInstruments { get; set; }

        public ResultsTableController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            TableView.Source = new TableSource(FilteredInstruments, this);
        }

        public void Search(string forSearchString)
        {
            FilteredInstruments = FilteredInstruments.Where(x => x.Symbol.Contains(forSearchString)).ToList();
            this.TableView.ReloadData();
        }

    }

	public class SearchResultsUpdator : UISearchResultsUpdating
	{
		public event Action<string> UpdateSearchResults = delegate { };

		public override void UpdateSearchResultsForSearchController(UISearchController searchController)
		{
			this.UpdateSearchResults(searchController.SearchBar.Text);
		}
	}

    class TableSource : UITableViewSource
    {
        UITableViewController owner;
        List<Instrument> instruments;
        string CellIdentifier = "SymbolListCell";

        public TableSource(List<Instrument> instruments, UITableViewController owner)
        {
            this.instruments = instruments.OrderBy(x=>x.Symbol).ToList();
            this.owner = owner;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return instruments.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellIdentifier, indexPath);
            cell.TextLabel.Text = instruments[indexPath.Row].Symbol;
            cell.DetailTextLabel.Text = instruments[indexPath.Row].Name;
			var logo = instruments[indexPath.Row].LogoFileName;
            cell.ImageView.Image = logo == null ? null : UIImage.FromFile(logo);
                
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //UIAlertController okAlertController = UIAlertController.Create("Row Selected", instruments[indexPath.Row].Symbol, UIAlertControllerStyle.Alert);
            //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            //owner.PresentViewController(okAlertController, true, null);

            var BalanceEditViewC = owner.Storyboard.InstantiateViewController("BalanceEditViewC") as BalanceEditViewController;
            BalanceEditViewC.NewCoinSelected(instruments[indexPath.Row].Symbol);
            owner.NavigationController.PushViewController(BalanceEditViewC, false);
        }

    }

}