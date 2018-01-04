using Foundation;
using System;
using System.IO;
using UIKit;
using QuickLook;
using CoreGraphics;
using CoreAnimation;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Syncfusion.SfDataGrid;
using Syncfusion.SfDataGrid.Exporting;
using CoinBalance.CoreModel;
using CoinBalance.UIModel;

namespace CoinBalance
{
    public partial class PLViewController : CryptoViewController
    {
        public List<RealizedPL> PLCollection { get; set; } = new List<RealizedPL>();

        const int yearFrom = 2014;
        SfDataGrid sfGrid;
        TradeList myTradeList = new TradeList();
        Exchange thisExchange;
        int calendarYear = DateTime.Now.Year;
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

                foreach (var exc in AppCore.PublicExchangeList.Where(x => x.CanCalcPL))
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
                    yearAlert.AddAction(UIAlertAction.Create(year.ToString(),
                                                                     UIAlertActionStyle.Default,
                                                                     async (obj) =>
                                                                     {
                                                                         buttonYear.SetTitle(year.ToString(), UIControlState.Normal);
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
                buttonYear.SetTitle(calendarYear.ToString(), UIControlState.Normal);
            }
            else
            {
                buttonExchange.SetTitle("Select Exchange", UIControlState.Highlighted);
                //buttonYear.Alpha = 0;
            }

            barbuttonShare.Enabled = PLCollection.Any() ? true : false;

            //Cash
            var cashcollection = PLCollection.Where(x => x.PLType == EnuPLType.CashTrade).Where(x => x.TradeDate.Year == calendarYear).ToList();

            labelCashBook.Text = cashcollection.Any() ? AppCore.NumberFormat(cashcollection.Last().AvgBookPrice, false, false) : "0";
            labelCashRPL.Text = cashcollection.Any() ? AppCore.NumberFormat(cashcollection.Sum(x => x.NetPL), false, false) : "0";

        }

        private async Task searchPLData()
        {
            PLCollection = new List<RealizedPL>();
            var bounds = PLSpreadsheetView.Bounds;
            loadPop = new LoadingOverlay(bounds);
            PLSpreadsheetView.Add(loadPop);
            buttonYear.Enabled = false;
            buttonExchange.Enabled = false;
            var prev_exc = myTradeList?.TradedExchange != null ? myTradeList?.TradedExchange : thisExchange;

            try
            {
                if (!myTradeList.Any() || myTradeList.ExchangeName() != thisExchange.Name)
                {
                    myTradeList = await AppCore.LoadTradeListsAsync(thisExchange);
                }

                if (myTradeList.Any())
                {
                    var pldata = myTradeList.CalculateCashTradesPL();
                    PLCollection.AddRange(pldata.Where(x => x.TradeDate.Year == calendarYear));
                }

                if(PLCollection.Any())
                {
                    sfGrid.ItemsSource = PLCollection;
                }
                else
                {
                    thisExchange = prev_exc;
                    this.PopUpWarning("Warning", $"There is no PL data at {thisExchange.Name} in {calendarYear}.");
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
            int[] years = new int[DateTime.Now.Year - yearFrom + 1];
            for (int i = 0; i <= years.Length - 1; i++)
            {
                years[i] = yearFrom + i;
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
                HeaderText = "BookPrice"
            };

            GridNumericColumn closePriceColumn = new GridNumericColumn()
            {
                MappingName = "ClosePrice",
                HeaderText = "ClosePrice"
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

            GridNumericColumn PLColumn = new GridNumericColumn()
            {
                MappingName = "NetPL",
                HeaderText = "Profit",
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
            sfGrid.Columns.Add(PLColumn);
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