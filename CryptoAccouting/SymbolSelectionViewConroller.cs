using Foundation;
using System;
using System.IO;
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
        public List<SelectionSearchItem> SelectionItems { get; set; }
        public string DestinationID { get; set; }

        public SymbolSelectionViewConroller(IntPtr handle) : base (handle)
        {
            NavigationItem.HidesBackButton = true;
            NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (sender, e) =>
             {
                 NavigationController.PopToRootViewController(true);
             }), true);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (SelectionItems is null) SelectionItems = new List<SelectionSearchItem>();
            TableView.Source = new TableSource(SelectionItems, this, DestinationID);

            resultsTableController = new ResultsTableController()
            {
                SearhItems = new List<SelectionSearchItem>()
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
            tableController.SearhItems = PerformSearch(searchController.SearchBar.Text);
            tableController.TableView.Source = new TableSource(tableController.SearhItems, this, DestinationID);
            tableController.TableView.ReloadData();
		}

        List<SelectionSearchItem> PerformSearch(string searchString)
		{
			searchString = searchString.Trim();
			string[] searchItems = string.IsNullOrEmpty(searchString)
				? new string[0]
				: searchString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var filteredItems = new List<SelectionSearchItem>();

			foreach (var item in searchItems)
			{
                IEnumerable<SelectionSearchItem> query =
                    from x in SelectionItems
                    where x.SearchItem1.IndexOf(item, StringComparison.OrdinalIgnoreCase) >= 0
                             || x.SearchItem2.IndexOf(item, StringComparison.OrdinalIgnoreCase) >= 0
                    orderby x.SearchItem1
                    select x;

				filteredItems.AddRange(query);
			}

			return filteredItems.Distinct().ToList();
		}

    }


    public class ResultsTableController : UITableViewController
    {
        //static NSString MyCellId = new NSString("SymbolListCell");
        public List<SelectionSearchItem> SearhItems { get; set; }
        public string DestinationID { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //TableView.RegisterNibForCellReuse(CoinViewCell.Nib, "CoinViewCell");
            //TableView.RegisterClassForCellReuse(typeof(UITableViewCell), MyCellId);
            TableView.Source = new TableSource(SearhItems, this, DestinationID);
        }

    }


    class TableSource : UITableViewSource
    {
        UITableViewController owner;
        string destinationid;
        List<SelectionSearchItem> searchitems;
        string cellIdentifier = "SymbolListCell";

        public TableSource(List<SelectionSearchItem> searchitems, UITableViewController owner, string destinationid)
        {
            this.searchitems = searchitems.OrderBy(x=>x.SortOrder).ToList();
            this.owner = owner;
            this.destinationid = destinationid;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return searchitems.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            //UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier, indexPath);
            //if (cell == null) 
            var cell = new UITableViewCell(UITableViewCellStyle.Subtitle, cellIdentifier);

            cell.TextLabel.Text = searchitems[indexPath.Row].SearchItem1;
            cell.DetailTextLabel.Text = searchitems[indexPath.Row].SearchItem2;

            if (searchitems[indexPath.Row].ImageFile != null)
            {
                var logo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                        "Images", searchitems[indexPath.Row].ImageFile);
                cell.ImageView.Image = logo == null ? null : UIImage.FromFile(logo);
            }

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (destinationid is null)
            {
                CryptoTableViewController root = owner.NavigationController.ViewControllers[0] as CryptoTableViewController;
                root.SetSearchSelectionItem(searchitems[indexPath.Row].SearchItem1);
                owner.NavigationController.PopToRootViewController(true);
            }else{
				var DestinationViewC = owner.Storyboard.InstantiateViewController(destinationid) as BalanceEditViewController;
				DestinationViewC.SetSearchSelectionItem(searchitems[indexPath.Row].SearchItem1);
				owner.NavigationController.PushViewController(DestinationViewC, false);
            }
        }

    }

    public class SelectionSearchItem
    {
        public string SearchItem1 { get; set; }
        public string SearchItem2 { get; set; }
        public string ImageFile { get; set; }
        public int SortOrder { get; set; }

    }
}