﻿using Foundation; using System; using System.IO; using UIKit; using QuickLook; using CoreGraphics; using CoreAnimation; using System.Linq; using System.Threading.Tasks; using Syncfusion.SfDataGrid; using Syncfusion.SfDataGrid.Exporting; using CoinBalance.CoreClass; using CoinBalance.UIClass;

namespace CoinBalance
{
    public partial class TransactionViewController : CryptoViewController
    {         SfDataGrid sfGrid;         TradeList myTradeList;         Exchange thisExchange;         LoadingOverlay loadPop;          public TransactionViewController(IntPtr handle) : base(handle)         {             initDataGrid();         }          public override void ViewDidLoad()
        {
            base.ViewDidLoad();              TransactionView.AddSubview(sfGrid);              //thisExchange = ApplicationCore.PublicExchangeList.First(x => x.APIReady == true);             //calendaryear = DateTime.Now.Year.ToString();             ReDrawScreen();              myTradeList = new TradeList();             sfGrid.ItemsSource = (myTradeList.TransactionCollection);             this.sfGrid.Frame = new CGRect(0, 0, TransactionView.Frame.Width, TransactionView.Frame.Height);              //Color Design             var gradient = new CAGradientLayer();             gradient.Frame = TradeTopView.Bounds;             gradient.NeedsDisplayOnBoundsChange = true;             gradient.MasksToBounds = true;             gradient.Colors = new CGColor[] { UIColor.FromRGB(0, 126, 167).CGColor, UIColor.FromRGB(0, 168, 232).CGColor };             TradeTopView.Layer.InsertSublayer(gradient, 0);              barbuttonShare.Clicked += (sender, e) => ExportToExcel(sender, e);              buttonExchange.TouchUpInside += (sender, e) =>              {                 UIAlertController exchangeAlert = UIAlertController.Create("Exchange", "Choose Exchange", UIAlertControllerStyle.ActionSheet);                 exchangeAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));                  foreach (var exc in AppCore.PublicExchangeList.Where(x => x.HasTradeAPI == true))                 {                     exchangeAlert.AddAction(UIAlertAction.Create(exc.Name,                                                                      UIAlertActionStyle.Default,                                                                      (obj) =>                                                                      {                                                                          buttonExchange.SetTitle(exc.Name, UIControlState.Normal);                                                                          thisExchange = exc;
                                                                         buttonSearch.Enabled = true;                                                                      }                                                                 ));                 }                 this.PresentViewController(exchangeAlert, true, null);             };              buttonSearch.TouchUpInside += async (sender, e) =>
            {                                  //if (!thisExchange.APIKeyAvailable())
                //{                 //    thisExchange = myTradeList.TradedExchange;
                //    UIAlertController okAlertController = UIAlertController.Create("Warning", "Please setup API keys before search.", UIAlertControllerStyle.Alert);
                //    okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                //    this.PresentViewController(okAlertController, true, null);                 //}
                //else
                //{
                    await searchTrades();
                //}             };
        }          public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);             ReDrawScreen();
        }          private void ExportToExcel(object sender, EventArgs e)         {             DataGridExcelExportingController excelExport = new DataGridExcelExportingController();             var excelEngine = excelExport.ExportToExcel(this.sfGrid);             var workbook = excelEngine.Excel.Workbooks[0];             MemoryStream stream = new MemoryStream();             workbook.SaveAs(stream);             workbook.Close();             excelEngine.Dispose();             ExportExcel(thisExchange.Code + "_trades.xlsx", "application/msexcel", stream);          }          private void ExportExcel(string filename, string contentType, MemoryStream stream)         {             string exception = string.Empty;             string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);             string filePath = Path.Combine(path, filename);             try             {                 FileStream fileStream = File.Open(filePath, FileMode.Create);                 stream.Position = 0;                 stream.CopyTo(fileStream);                 fileStream.Flush();                 fileStream.Close();             }             catch (Exception e)             {                 exception = e.ToString();             }              if (contentType == "application/html" || exception != string.Empty)                 return;              UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;             while (currentController.PresentedViewController != null)                 currentController = currentController.PresentedViewController;             UIView currentView = currentController.View;              QLPreviewController qlPreview = new QLPreviewController();             QLPreviewItem item = new QLPreviewItemBundle(filename, filePath);             qlPreview.DataSource = new PreviewControllerDS(item);              currentController.PresentViewController((UIViewController)qlPreview, true, (Action)null);         }          public override void ReDrawScreen()         {             if (thisExchange != null)
            {
                buttonExchange.SetTitle(thisExchange.Name, UIControlState.Normal);                 buttonSearch.Enabled = true;             }
            else
            {
                buttonExchange.SetTitle("Select Exchange", UIControlState.Highlighted);                 buttonSearch.Enabled = false;
            }              if (myTradeList != null)             {                 barbuttonShare.Enabled = myTradeList.Any() ? true : false;                  labelNumBuy.Text = AppCore.NumberFormat(myTradeList.NumOrdersBuy, false, false);                 labelNumSell.Text = AppCore.NumberFormat(myTradeList.NumOrdersSell, false, false);
                labelBTCBuy.Text = AppCore.NumberFormat(myTradeList.TotalBTCTradeValueBuy, false, true);
                labelBTCSell.Text = AppCore.NumberFormat(myTradeList.TotalBTCTradeValueSell, false, true);                  if (myTradeList.SettlementCCY != EnuCCY.BTC)
                {
                    labelSettleCrossBuy.Text = AppCore.NumberFormat(myTradeList.TotalExchangeSettleTradeValueBuy, false, true);
                    labelSettleCrossSell.Text = AppCore.NumberFormat(myTradeList.TotalExchangeSettleTradeValueSell, false, true);
                    labelSettleCrossText.Text = "Trades/" + myTradeList.SettlementCCY.ToString();                 }
                else                 {
                    labelSettleCrossBuy.Text = "---";                     labelSettleCrossSell.Text = "---";                     labelSettleCrossText.Text = "---"; 
                }             }         }          private async Task searchTrades()         {             var bounds = TransactionView.Bounds;             loadPop = new LoadingOverlay(bounds);             TransactionView.Add(loadPop);             buttonSearch.Enabled = false;             buttonExchange.Enabled = false;              try             {                 await AppCore.LoadTradeListsAsync(thisExchange.Code);                 myTradeList = AppCore.GetExchangeTradeList(thisExchange.Code);                  if (myTradeList.Any())                 {                     sfGrid.ItemsSource = myTradeList.TransactionCollection;                 }                 else                 {                     throw new AppCoreWarning("No Recent Trade data available.");
                }
            }             catch (Exception ex)             {                 this.PopUpWarning("Warning", ex.Message + " Please check your API keys.");                 //UIAlertController okAlertController = UIAlertController.Create("Warning"                 //                                                               , ex.Message + " Please check your API keys."                 //                                                               , UIAlertControllerStyle.Alert);                 //okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));                 //this.PresentViewController(okAlertController, true, null);                  System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": ViewDidLoad: buttonSearch: " + ex.GetType() + ": " + ex.Message);             }             finally             {                 thisExchange = myTradeList.Any() ? myTradeList.TradedExchange : thisExchange;                 loadPop.Hide();                 buttonSearch.Enabled = true;                 buttonExchange.Enabled = true;                 ReDrawScreen();             }         }          private void initDataGrid()         {             sfGrid = new SfDataGrid();             sfGrid.AllowSorting = true;             sfGrid.AutoGenerateColumns = false;             sfGrid.BackgroundColor = UIColor.FromRGB(236, 184, 138);             sfGrid.GridStyle = new TransactionStyle();              sfGrid.HeaderRowHeight = 30;             sfGrid.RowHeight = 30;              GridDateTimeColumn dateColumn = new GridDateTimeColumn()             {                 MappingName = "TradeDate",                 HeaderText = "Date"             };              GridTextColumn coinColumn = new GridTextColumn()             {                 MappingName = "ColumnCoinSymbol",                 HeaderText = "Coin"             };              GridTextColumn settleColumn = new GridTextColumn()             {                 MappingName = "SettlementCCY",                 HeaderText = "Settle"             };              GridTextColumn buysellColumn = new GridTextColumn()             {                 MappingName = "BuySell",                 HeaderText = "Side"             };              GridNumericColumn amountColumn = new GridNumericColumn()             {                 MappingName = "Quantity",                 HeaderText = "Quantity"             };              GridTextColumn priceColumn = new GridTextColumn()             {                 MappingName = "ColumnTradePriceSettle",                 HeaderText = "Trade Price"             };              GridTextColumn valueColumn = new GridTextColumn()             {                 MappingName = "ColumnTradeNetValue",                 HeaderText = "Net Value"             };              GridNumericColumn numtxColumn = new GridNumericColumn()             {                 MappingName = "NumTransaction",                 HeaderText = "#orders",                 NumberDecimalDigits = 0             };              sfGrid.Columns.Add(dateColumn);             sfGrid.Columns.Add(coinColumn);             sfGrid.Columns.Add(settleColumn);             sfGrid.Columns.Add(buysellColumn);             sfGrid.Columns.Add(amountColumn);             sfGrid.Columns.Add(priceColumn);             sfGrid.Columns.Add(valueColumn);             sfGrid.Columns.Add(numtxColumn);         }          private class TransactionStyle : DataGridStyle         {              public override GridLinesVisibility GetGridLinesVisibility()             {                 return GridLinesVisibility.Both;             }         }     } }
