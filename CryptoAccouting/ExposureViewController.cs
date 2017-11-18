using Foundation;
using System;
using UIKit;
using System.ComponentModel;
using CoreGraphics;
//using CoreAnimation;
using Syncfusion.SfDataGrid;
using Syncfusion.SfChart.iOS;
using CoinBalance.CoreClass;
using CoinBalance.UIClass;

namespace CoinBalance
{
    public partial class ExposureViewController : UIViewController
    {
        SfDataGrid sfGrid;
        SFChart sfChartCoinWeight, sfChartLocation;
        SFPieSeries CoinWeightSeries, LocationSeries;
        CoinStorageList myExposure;
        Balance myBalance;

        public ExposureViewController (IntPtr handle) : base (handle)
        {
            myExposure = AppCore.CoinStorageList;
            myBalance = AppCore.Balance;
            initCoinWeightChart();
            initLocationChart();
            //initDataGrid();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Color Design
            //var gradient = new CAGradientLayer();
            //gradient.Frame = ExposureDrawingView.Bounds;
            //gradient.NeedsDisplayOnBoundsChange = true;
            //gradient.MasksToBounds = true;
            //gradient.Colors = new CGColor[] { UIColor.FromRGB(0, 126, 167).CGColor, UIColor.FromRGB(0, 168, 232).CGColor };
            //ExposureDrawingView.Layer.InsertSublayer(gradient, 0);

            //Chart
            ExposureDrawingView.AddSubview(sfChartCoinWeight);
            sfChartCoinWeight.Frame = new CGRect(0, 0, ExposureDrawingView.Frame.Width, ExposureDrawingView.Frame.Height);

            ExposureGridView.AddSubview(sfChartLocation);
            sfChartLocation.Frame = new CGRect(0, 0, ExposureGridView.Frame.Width, ExposureGridView.Frame.Height);

            //Grid
            //ExposureGridView.AddSubview(sfGrid);
            //this.sfGrid.Frame = new CGRect(0, 0, ExposureGridView.Frame.Width, ExposureGridView.Frame.Height);

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //sfGrid.ItemsSource = (myExposure.StorageCollection);
            CoinWeightSeries.ItemsSource = myBalance.BalanceCollection;
            LocationSeries.ItemsSource = myExposure.StorageCollection;
        }

        private void initCoinWeightChart()
        {
            sfChartCoinWeight = new SFChart();
            sfChartCoinWeight.Title.Text = "Coin Weight";

            CoinWeightSeries = new SFPieSeries()
            {
                //ItemsSource = myBalance.BalanceCollection,
                XBindingPath = "ColumnCoinSymbol",
                YBindingPath = "ColumnAmountBTC"
                //CircularCoefficient = 0.75,
            };

            CoinWeightSeries.DataMarkerPosition = SFChartCircularSeriesLabelPosition.Inside;
            CoinWeightSeries.DataMarker.LabelContent = SFChartLabelContent.Percentage;
            CoinWeightSeries.EnableSmartLabels = true;
            CoinWeightSeries.ConnectorLineType = SFChartConnectorLineType.Bezier;
            CoinWeightSeries.DataMarker.ShowLabel = true;
            CoinWeightSeries.LegendIcon = SFChartLegendIcon.Rectangle;

            sfChartCoinWeight.Series.Add(CoinWeightSeries);
            sfChartCoinWeight.Legend.Visible = true;
            //sfChart.Legend.Orientation = SFChartLegendOrientation.Vertical;
            //sfChart.Legend.ToggleSeriesVisibility = true;
        }

        private void initLocationChart()
        {
            sfChartLocation = new SFChart();
            sfChartLocation.Title.Text = "Wallet Location";

            LocationSeries = new SFPieSeries()
            {
                //ItemsSource = myExposure.StorageCollection,
                XBindingPath = "Name",
                YBindingPath = "ColumnHolding"
                //CircularCoefficient = 0.75,
            };

            LocationSeries.DataMarkerPosition = SFChartCircularSeriesLabelPosition.OutsideExtended;
            LocationSeries.DataMarker.LabelContent = SFChartLabelContent.Percentage;
            LocationSeries.EnableSmartLabels = true;
            LocationSeries.ConnectorLineType = SFChartConnectorLineType.Bezier;
            LocationSeries.DataMarker.ShowLabel = true;
            LocationSeries.LegendIcon = SFChartLegendIcon.Rectangle;

            sfChartLocation.Series.Add(LocationSeries);
            sfChartLocation.Legend.Visible = true;
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