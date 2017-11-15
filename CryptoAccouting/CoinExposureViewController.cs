using Foundation;
using System;
using System.IO;
using UIKit;
using QuickLook;
using CoreGraphics;
using CoreAnimation;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.SfDataGrid;
using Syncfusion.SfDataGrid.Exporting;
using CoinBalance.CoreClass;
using CoinBalance.UIClass;

namespace CoinBalance
{
    public partial class CoinExposureViewController : UIViewController
    {
        SfDataGrid sfGrid;
        CoinStorageList myExposure;

        public CoinExposureViewController (IntPtr handle) : base (handle)
        {
            initDataGrid();

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ExposureGridView.AddSubview(sfGrid);

            myExposure = AppCore.CoinStorageList;
            this.sfGrid.Frame = new CGRect(0, 0, ExposureGridView.Frame.Width, ExposureGridView.Frame.Height);

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            sfGrid.ItemsSource = (myExposure.StorageCollection);
        }

        private void initDataGrid()
        {
            sfGrid = new SfDataGrid();
            sfGrid.AllowSorting = true;
            sfGrid.AutoGenerateColumns = false;
            sfGrid.BackgroundColor = UIColor.FromRGB(236, 184, 138);

            sfGrid.HeaderRowHeight = 30;
            sfGrid.RowHeight = 30;

            GridTextColumn locationColumn = new GridTextColumn()
            {
                MappingName = "ColumnLocation",
                HeaderText = "Location"
            };

            GridTextColumn holdingColumn = new GridTextColumn()
            {
                MappingName = "ColumnHolding",
                HeaderText = "Holding"
            };

            GridTextColumn valueColumn = new GridTextColumn()
            {
                MappingName = "ColumnValue",
                HeaderText = "Value"
            };

            GridTextColumn weightColumn = new GridTextColumn()
            {
                MappingName = "ColumnWeight",
                HeaderText = "Weight"
            };

            sfGrid.Columns.Add(locationColumn);
            sfGrid.Columns.Add(holdingColumn);
            sfGrid.Columns.Add(valueColumn);
            sfGrid.Columns.Add(weightColumn);
        }
    }
}