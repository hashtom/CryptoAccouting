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
using CoinBalance.CoreModel;
using CoinBalance.UIModel;

namespace CoinBalance
{
    public partial class PLViewController : CryptoViewController
    {
        const int yearFrom = 2014;
        SfDataGrid sfGrid;
        TradeList myTradeList;
        Exchange thisExchange;
        int calendarYear = 0;
        LoadingOverlay loadPop;

        public PLViewController (IntPtr handle) : base (handle)
        {
            initDataGrid();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            PLSpreadsheetView.AddSubview(sfGrid);

            //thisExchange = ApplicationCore.PublicExchangeList.First(x => x.APIReady == true);
            //calendaryear = DateTime.Now.Year.ToString();
            ReDrawScreen();

            myTradeList = new TradeList();
            sfGrid.ItemsSource = (myTradeList.TransactionCollection);
            this.sfGrid.Frame = new CGRect(0, 0, PLSpreadsheetView.Frame.Width, PLSpreadsheetView.Frame.Height);

            //Color Design
            var gradient = new CAGradientLayer();
            gradient.Frame = PLTopView.Bounds;
            gradient.NeedsDisplayOnBoundsChange = true;
            gradient.MasksToBounds = true;
            gradient.Colors = new CGColor[] { UIColor.FromRGB(0, 126, 167).CGColor, UIColor.FromRGB(0, 168, 232).CGColor };
            PLTopView.Layer.InsertSublayer(gradient, 0);

            barbuttonShare.Clicked += (sender, e) => ExportToExcel(sender, e);

            buttonExchange.TouchUpInside += (sender, e) =>
            {
                UIAlertController exchangeAlert = UIAlertController.Create("Exchange", "Choose Exchange", UIAlertControllerStyle.ActionSheet);
                exchangeAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

                foreach (var exc in AppCore.PublicExchangeList.Where(x => x.Code == "Zaif"))
                {
                    exchangeAlert.AddAction(UIAlertAction.Create(exc.Name,
                                                                     UIAlertActionStyle.Default,
                                                                     async (obj) =>
                                                                     {
                                                                         buttonExchange.SetTitle(exc.Name, UIControlState.Normal);
                                                                         thisExchange = exc;
                                                                         await searchPLData();
                                                                     }
                                                                ));
                }
                this.PresentViewController(exchangeAlert, true, null);
            };

            buttonYear.TouchUpInside += (sender, e) =>
            {
                UIAlertController yearAlert = UIAlertController.Create("Calendar Year", "Choose Year", UIAlertControllerStyle.ActionSheet);
                yearAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

                foreach (int year in SearchYears())
                {
                    var title = year != 0 ? year.ToString() : "Max";
                    yearAlert.AddAction(UIAlertAction.Create(title,
                                                                     UIAlertActionStyle.Default,
                                                                     async (obj) =>
                                                                     {
                                                                         buttonYear.SetTitle(title, UIControlState.Normal);
                                                                         calendarYear = year;
                                                                         if (thisExchange != null) await searchPLData();
                                                                     }
                                                            ));
                }
                this.PresentViewController(yearAlert, true, null);
            };
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ReDrawScreen();
        }

        private void ExportToExcel(object sender, EventArgs e)
        {
            DataGridExcelExportingController excelExport = new DataGridExcelExportingController();
            var excelEngine = excelExport.ExportToExcel(this.sfGrid);
            var workbook = excelEngine.Excel.Workbooks[0];
            MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);
            workbook.Close();
            excelEngine.Dispose();
            ExportExcel(thisExchange.Code + "_pl.xlsx", "application/msexcel", stream);

        }

        private void ExportExcel(string filename, string contentType, MemoryStream stream)
        {
            string exception = string.Empty;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(path, filename);
            try
            {
                FileStream fileStream = File.Open(filePath, FileMode.Create);
                stream.Position = 0;
                stream.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
            catch (Exception e)
            {
                exception = e.ToString();
            }

            if (contentType == "application/html" || exception != string.Empty)
                return;

            UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (currentController.PresentedViewController != null)
                currentController = currentController.PresentedViewController;
            UIView currentView = currentController.View;

            QLPreviewController qlPreview = new QLPreviewController();
            QLPreviewItem item = new QLPreviewItemBundle(filename, filePath);
            qlPreview.DataSource = new PreviewControllerDS(item);

            currentController.PresentViewController((UIViewController)qlPreview, true, (Action)null);
        }

        public override void ReDrawScreen()
        {
            if (thisExchange != null)
            {
                buttonExchange.SetTitle(thisExchange.Name, UIControlState.Normal);
                var title = calendarYear != 0 ? calendarYear.ToString() : "Max";
                buttonYear.SetTitle(title, UIControlState.Normal);
            }
            else
            {
                buttonExchange.SetTitle("Select Exchange", UIControlState.Highlighted);
                //buttonYear.Alpha = 0;
            }

            if (myTradeList != null)
            {
                barbuttonShare.Enabled = myTradeList.Any() ? true : false;

                //labelNumBuy.Text = AppCore.NumberFormat(myTradeList.NumOrdersBuy, false, false);
                //labelNumSell.Text = AppCore.NumberFormat(myTradeList.NumOrdersSell, false, false);
                //labelBTCBuy.Text = AppCore.NumberFormat(myTradeList.TotalBTCTradeValueBuy, false, true);
                //labelBTCSell.Text = AppCore.NumberFormat(myTradeList.TotalBTCTradeValueSell, false, true);

                if (myTradeList.SettlementCCY != EnuCCY.BTC)
                {
                    //labelSettleCrossBuy.Text = AppCore.NumberFormat(myTradeList.TotalExchangeSettleTradeValueBuy, false, true);
                    //labelSettleCrossSell.Text = AppCore.NumberFormat(myTradeList.TotalExchangeSettleTradeValueSell, false, true);
                    //labelSettleCrossText.Text = "Trades/" + myTradeList.SettlementCCY.ToString();
                }
                else
                {
                    //labelSettleCrossBuy.Text = "---";
                    //labelSettleCrossSell.Text = "---";
                    //labelSettleCrossText.Text = "---";

                }
            }
        }

        private async Task searchPLData()
        {
            var bounds = PLSpreadsheetView.Bounds;
            loadPop = new LoadingOverlay(bounds);
            PLSpreadsheetView.Add(loadPop);
            buttonYear.Enabled = false;
            buttonExchange.Enabled = false;
            var prev_exc = myTradeList?.TradedExchange != null ? myTradeList?.TradedExchange : thisExchange;

            try
            {
                myTradeList = await AppCore.LoadTradeListsAsync(thisExchange, calendarYear);
                if (myTradeList.Any())
                {
                    sfGrid.ItemsSource = myTradeList.TransactionCollection;
                }
                else
                {
                    thisExchange = prev_exc;
                    this.PopUpWarning("Warning", "No data returned from the exchange.");
                }
            }
            catch (Exception ex)
            {
                thisExchange = prev_exc;
                this.PopUpWarning("Warning", ex.Message);
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": ViewDidLoad: buttonSearch: " + ex.GetType() + ": " + ex.Message);
            }
            finally
            {
                //thisExchange = myTradeList.Any() ? myTradeList.TradedExchange : thisExchange;
                loadPop.Hide();
                buttonYear.Enabled = true;
                buttonExchange.Enabled = true;
                ReDrawScreen();
            }
        }

        private int[] SearchYears()
        {
            int[] years = new int[DateTime.Now.Year - yearFrom + 2];
            years[0] = 0;
            for (int i = 1; i <= years.Length - 1; i++)
            {
                years[i] = yearFrom - 1 + i;
            }
            return years;
        }

        private void initDataGrid()
        {
            sfGrid = new SfDataGrid();
            sfGrid.AllowSorting = true;
            sfGrid.AutoGenerateColumns = false;
            sfGrid.BackgroundColor = UIColor.FromRGB(236, 184, 138);
            sfGrid.GridStyle = new TransactionStyle();

            sfGrid.HeaderRowHeight = 30;
            sfGrid.RowHeight = 30;

            GridDateTimeColumn dateColumn = new GridDateTimeColumn()
            {
                MappingName = "TradeDate",
                HeaderText = "Date"
            };

            GridTextColumn coinColumn = new GridTextColumn()
            {
                MappingName = "ColumnCoinSymbol",
                HeaderText = "Coin"
            };

            GridTextColumn PLTypeColumn = new GridTextColumn()
            {
                MappingName = "PLType",
                HeaderText = "Type"
            };

            GridTextColumn settleColumn = new GridTextColumn()
            {
                MappingName = "SettlementCCY",
                HeaderText = "Settle"
            };

            GridTextColumn sideColumn = new GridTextColumn()
            {
                MappingName = "Side",
                HeaderText = "Side"
            };

            GridNumericColumn qtyColumn = new GridNumericColumn()
            {
                MappingName = "Quantity",
                HeaderText = "Quantity"
            };

            GridNumericColumn bookColumn = new GridNumericColumn()
            {
                MappingName = "AvgBookPrice",
                HeaderText = "Book"
            };

            GridNumericColumn closePriceColumn = new GridNumericColumn()
            {
                MappingName = "ClosePrice",
                HeaderText = "Close"
            };

            GridNumericColumn TradeFeeColumn = new GridNumericColumn()
            {
                MappingName = "TradeFee",
                HeaderText = "TradeFee",
                NumberDecimalDigits = 0
            };

            GridNumericColumn MarginFeeColumn = new GridNumericColumn()
            {
                MappingName = "MarginFee",
                HeaderText = "MarginFee",
                NumberDecimalDigits = 0
            };

            GridNumericColumn SwapColumn = new GridNumericColumn()
            {
                MappingName = "Swap",
                HeaderText = "Swap",
                NumberDecimalDigits = 0
            };

            GridNumericColumn DepWithFeeColumn = new GridNumericColumn()
            {
                MappingName = "DepWithFee",
                HeaderText = "DepWithFee",
                NumberDecimalDigits = 0
            };

            sfGrid.Columns.Add(dateColumn);
            sfGrid.Columns.Add(coinColumn);
            sfGrid.Columns.Add(PLTypeColumn);
            sfGrid.Columns.Add(settleColumn);
            sfGrid.Columns.Add(sideColumn);
            sfGrid.Columns.Add(qtyColumn);
            sfGrid.Columns.Add(bookColumn);
            sfGrid.Columns.Add(closePriceColumn);
            sfGrid.Columns.Add(TradeFeeColumn);
            sfGrid.Columns.Add(MarginFeeColumn);
            sfGrid.Columns.Add(SwapColumn);
            sfGrid.Columns.Add(DepWithFeeColumn);
        }

        private class TransactionStyle : DataGridStyle
        {

            public override GridLinesVisibility GetGridLinesVisibility()
            {
                return GridLinesVisibility.Both;
            }
        }
    }
}