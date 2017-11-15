using Foundation;
using System;
using UIKit;
using System.ComponentModel;
using CoreGraphics;
using Syncfusion.SfDataGrid;
using Syncfusion.SfChart.iOS;
using CoinBalance.CoreClass;
using CoinBalance.UIClass;

namespace CoinBalance
{
    public partial class ExposureViewController : UIViewController
    {
        SfDataGrid sfGrid;
        SFChart sfChart;
        CoinStorageList myExposure;
        Balance myBalance;

        public ExposureViewController (IntPtr handle) : base (handle)
        {
            myExposure = AppCore.CoinStorageList;
            myBalance = AppCore.Balance;
            initAssetChart();
            initDataGrid();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Chart
            ExposureDrawingView.AddSubview(sfChart);
            sfChart.Frame = new CGRect(0, 0, ExposureDrawingView.Frame.Width, ExposureDrawingView.Frame.Height);

            //Grid
            ExposureGridView.AddSubview(sfGrid);
            this.sfGrid.Frame = new CGRect(0, 0, ExposureGridView.Frame.Width, ExposureGridView.Frame.Height);

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            sfGrid.ItemsSource = (myExposure.StorageCollection);
        }

        private void initAssetChart()
        {
            sfChart = new SFChart();
            sfChart.Title.Text = "Asset Weights";

            SFPieSeries pieSeries = new SFPieSeries()
            {
                ItemsSource = myBalance.BalanceCollection,
                XBindingPath = "ColumnCoinSymbol",
                YBindingPath = "ColumnAmountBTC",
                //CircularCoefficient = 0.75,  
            };

            pieSeries.DataMarkerPosition = SFChartCircularSeriesLabelPosition.Inside;
            pieSeries.DataMarker.LabelContent = SFChartLabelContent.Percentage;
            pieSeries.EnableSmartLabels = true;
            pieSeries.ConnectorLineType = SFChartConnectorLineType.Bezier;
            pieSeries.DataMarker.ShowLabel = true;
            pieSeries.LegendIcon = SFChartLegendIcon.Rectangle;

            sfChart.Series.Add(pieSeries);

            sfChart.Legend.Visible = true;
            //sfChart.Legend.Orientation = SFChartLegendOrientation.Vertical;
            //sfChart.Legend.ToggleSeriesVisibility = true;
        }

        private void initStorageChart()
        {
            sfChart = new SFChart();
            sfChart.Title.Text = "Coin Location";

            SFPieSeries pieSeries = new SFPieSeries()
            {
                ItemsSource = myExposure.StorageCollection,
                XBindingPath = "Name",
                YBindingPath = "Weight",
                CircularCoefficient = 0.75,
            };

            pieSeries.DataMarkerPosition = SFChartCircularSeriesLabelPosition.OutsideExtended;
            pieSeries.DataMarker.LabelContent = SFChartLabelContent.Percentage;
            pieSeries.EnableSmartLabels = true;
            pieSeries.ConnectorLineType = SFChartConnectorLineType.Bezier;
            pieSeries.DataMarker.ShowLabel = true;
            pieSeries.LegendIcon = SFChartLegendIcon.Rectangle;

            sfChart.Series.Add(pieSeries);
        }

        private void initDataGrid()
        {
            
            sfGrid = new SfDataGrid();
            sfGrid.AllowSorting = true;
            sfGrid.AutoGenerateColumns = false;
            sfGrid.BackgroundColor = UIColor.FromRGB(236, 184, 138);

            sfGrid.HeaderRowHeight = 30;
            sfGrid.RowHeight = 30;
            sfGrid.GridStyle = new ExposureStyle();

            GridTextColumn locationColumn = new GridTextColumn()
            {
                MappingName = "Name",
                HeaderText = "Location",
                TextAlignment = UITextAlignment.Left
            };

            GridNumericColumn holdingColumn = new GridNumericColumn()
            {
                MappingName = "ColumnHolding",
                HeaderText = "Holding(BTC)",
                NumberDecimalDigits = 4
            };

            GridNumericColumn valueColumn = new GridNumericColumn()
            {
                MappingName = "ColumnValue",
                HeaderText = "Fiat Value",
                NumberDecimalDigits = 0
                //NumberDecimalSeparator = ","
            };

            GridTextColumn weightColumn = new GridTextColumn()
            {
                MappingName = "ColumnWeight",
                HeaderText = "Weight"
            };

            sfGrid.Columns.Add(locationColumn);
            sfGrid.Columns.Add(weightColumn);
            sfGrid.Columns.Add(holdingColumn);
            sfGrid.Columns.Add(valueColumn);

            //sfGrid.SortColumnDescriptions.Add(new SortColumnDescription()
            //{
            //    ColumnName = "Value",
            //    SortDirection = ListSortDirection.Descending
            //});

        }

        private class ExposureStyle : DataGridStyle
        {
            
            public override GridLinesVisibility GetGridLinesVisibility()
            {
                return GridLinesVisibility.Both;
            }
        }
    }
}