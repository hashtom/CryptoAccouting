using Foundation; using System; using UIKit; using CoreGraphics; using Syncfusion.SfDataGrid; using CryptoAccouting.CoreClass; using System.Linq; using CryptoAccouting.UIClass; using System.Threading.Tasks;

namespace CryptoAccouting
{
    public partial class TransactionViewController : UIViewController
    {         SfDataGrid sfGrid;         TradeList myTradeList;         string calendaryear = null;         Exchange thisExchange;         LoadingOverlay loadPop;          public TransactionViewController(IntPtr handle) : base(handle)         {             sfGrid = new SfDataGrid();             //sfGrid.AutoGeneratingColumn += HandleAutoGeneratingColumn;             sfGrid.AllowSorting = true;             sfGrid.AutoGenerateColumns = false;             sfGrid.BackgroundColor = UIColor.FromRGB(236, 184, 138);             //sfGrid.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = "txid" });              GridDateTimeColumn dateColumn = new GridDateTimeColumn()             {                 MappingName = "TradeDate",                 HeaderText = "Date"             };              GridTextColumn coinColumn = new GridTextColumn()             {
                MappingName = "TradecCoinSymbol",                 HeaderText = "Coin"
            };              GridTextColumn settleColumn = new GridTextColumn()             {
                MappingName = "SettlementCCY_Str",
                HeaderText = "SettleCCY"             };              GridTextColumn buysellColumn = new GridTextColumn()             {
                MappingName = "Side",
                HeaderText = "Side"             };              GridNumericColumn amountColumn = new GridNumericColumn()             {
                MappingName = "Quantity",
                HeaderText = "Quantity"             };

            GridNumericColumn priceColumn = new GridNumericColumn()
            {
                MappingName = "TradePrice",
                HeaderText = "Trade Price"
            };              GridNumericColumn valueColumn = new GridNumericColumn()             {
                MappingName = "TradeNetValue",
                HeaderText = "Net Value"             };              GridNumericColumn numtxColumn = new GridNumericColumn()             {
                MappingName = "NumOfTransaction",
                HeaderText = "#tx",                 NumberDecimalDigits = 0             };              sfGrid.Columns.Add(dateColumn);             sfGrid.Columns.Add(coinColumn);             sfGrid.Columns.Add(settleColumn);             sfGrid.Columns.Add(buysellColumn);             sfGrid.Columns.Add(amountColumn);             sfGrid.Columns.Add(priceColumn);             sfGrid.Columns.Add(valueColumn);             sfGrid.Columns.Add(numtxColumn);          }          public override void ViewDidLoad()
        {
            base.ViewDidLoad();             sfGrid.HeaderRowHeight = 30;             sfGrid.RowHeight = 30;             TransactionView.AddSubview(sfGrid);              //thisExchange = ApplicationCore.PublicExchangeList.First(x => x.APIReady == true);             //calendaryear = DateTime.Now.Year.ToString();             ReDrawScreen();              myTradeList = new TradeList(ApplicationCore.BaseCurrency);             sfGrid.ItemsSource = (myTradeList.TransactionCollection);             this.sfGrid.Frame = new CGRect(0, 0, TransactionView.Frame.Width, TransactionView.Frame.Height);              buttonExchange.TouchUpInside += (sender, e) =>              {                 UIAlertController exchangeAlert = UIAlertController.Create("Exchange", "Choose Exchange", UIAlertControllerStyle.ActionSheet);                 exchangeAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));                  foreach (var exc in ApplicationCore.PublicExchangeList.Where(x => x.APIReady == true))                 {                     exchangeAlert.AddAction(UIAlertAction.Create(exc.Name,                                                                      UIAlertActionStyle.Default,                                                                      (obj) =>                                                                      {                                                                          buttonExchange.SetTitle(exc.Name, UIControlState.Normal);                                                                          thisExchange = exc;
                                                                         buttonSearch.Enabled = true;                                                                      }                                                                 ));                 }                 this.PresentViewController(exchangeAlert, true, null);             };              //buttonCalndarYear.TouchUpInside += (sender, e) =>              //{             //    var thisyear = DateTime.Now.Year;             //    string[] years = new string[] { thisyear.ToString(), (thisyear - 1).ToString(), (thisyear - 2).ToString(), "ALL" };              //    UIAlertController calndarYearAlert = UIAlertController.Create("Calendar Year", "Choose Year", UIAlertControllerStyle.ActionSheet);             //    calndarYearAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));              //    foreach (var yr in years)             //    {             //        calndarYearAlert.AddAction(UIAlertAction.Create(yr.ToString(), UIAlertActionStyle.Default,             //                                                         (obj) =>             //                                                         {
            //                                                             buttonCalndarYear.SetTitle(yr, UIControlState.Normal);
            //                                                             calendaryear = yr == "ALL" ? null : yr;             //                                                         }             //                                                    ));             //    }             //    this.PresentViewController(calndarYearAlert, true, null);              //};              buttonSearch.TouchUpInside += async (sender, e) =>
            {                 if (!thisExchange.APIKeyAvailable())
                {
                    UIAlertController okAlertController = UIAlertController.Create("Warning", "Please setup API keys before search.", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                    this.PresentViewController(okAlertController, true, null);                 }
                else
                {
                    //var bounds = UIScreen.MainScreen.Bounds;
                    var bounds = TransactionView.Bounds;
                    loadPop = new LoadingOverlay(bounds);
                    TransactionView.Add(loadPop);

                    if (await ApplicationCore.LoadTradeListsAsync(thisExchange.Code, calendaryear, true) is EnuAPIStatus.Success)
                    {
                        myTradeList = ApplicationCore.GetExchangeTradeList(thisExchange.Code);

                        if (myTradeList != null)
                        {
                            //myTradeList.CalculateTotalValue(calendaryear, "BTC");

                            //Show Grid View
                            sfGrid.ItemsSource = (myTradeList.TransactionCollection);
                            //this.sfGrid.Frame = new CGRect(0, 0, TransactionView.Frame.Width, TransactionView.Frame.Height);
                        }
                        else
                        {
                            UIAlertController okAlertController = UIAlertController.Create("Warning", "Exchange API call might be timed out. try again."
                                                                                           , UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                    }                     ReDrawScreen();
                    loadPop.Hide();
                }             };
        }          public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);             ReDrawScreen(); 
        }          private void ReDrawScreen()         {             if (thisExchange != null)
            {
                buttonExchange.SetTitle(thisExchange.Code, UIControlState.Normal);                 buttonSearch.Enabled = true;             }
            else
            {
                buttonExchange.SetTitle("Select Exchange", UIControlState.Highlighted);                 buttonSearch.Enabled = false;
            }              if (myTradeList != null)             {
                labelBTCQtyBuy.Text = ApplicationCore.NumberFormat(myTradeList.TotalQuantity("bitcoin", EnuBuySell.Buy));                 labelBTCValBuy.Text = ApplicationCore.NumberFormat(myTradeList.TotalNetValue("bitcoin", EnuBuySell.Buy) / myTradeList.TotalQuantity("bitcoin", EnuBuySell.Buy));                 labelBTCQtySell.Text = ApplicationCore.NumberFormat(myTradeList.TotalQuantity("bitcoin", EnuBuySell.Sell));                 labelBTCValSell.Text = ApplicationCore.NumberFormat(myTradeList.TotalNetValue("bitcoin", EnuBuySell.Sell) / myTradeList.TotalQuantity("bitcoin", EnuBuySell.Sell));                 labelMonaQtyBuy.Text = ApplicationCore.NumberFormat(myTradeList.TotalQuantity("monacoin", EnuBuySell.Buy));                 labelMonaValBuy.Text = ApplicationCore.NumberFormat(myTradeList.TotalNetValue("monacoin", EnuBuySell.Buy) / myTradeList.TotalQuantity("monacoin", EnuBuySell.Buy));                 labelMonaQtySell.Text = ApplicationCore.NumberFormat(myTradeList.TotalQuantity("monacoin", EnuBuySell.Sell));                 labelMonaValSell.Text = ApplicationCore.NumberFormat(myTradeList.TotalNetValue("monacoin", EnuBuySell.Sell) / myTradeList.TotalQuantity("monacoin", EnuBuySell.Sell));
            }          }       //void HandleAutoGeneratingColumn(object sender, AutoGeneratingColumnArgs e)        //{         //  if (e.Column.MappingName == "txid")         //  {       //      e.Column.TextMargin = 1;        //      e.Column.TextAlignment = UITextAlignment.Left;      //  }       //}      } }
