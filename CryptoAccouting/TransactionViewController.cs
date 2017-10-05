using Foundation; using System; using UIKit; using CoreGraphics; using Syncfusion.SfDataGrid; using CryptoAccouting.CoreClass; using System.Linq; using CryptoAccouting.UIClass; using System.Threading.Tasks;

namespace CryptoAccouting
{
    public partial class TransactionViewController : UIViewController
    {         SfDataGrid sfGrid;         TradeList myTradeList;         string calendaryear;         Exchange thisExchange;         LoadingOverlay loadPop;          public TransactionViewController(IntPtr handle) : base(handle)         {             sfGrid = new SfDataGrid();             //sfGrid.AutoGeneratingColumn += HandleAutoGeneratingColumn;             sfGrid.AllowSorting = true;             sfGrid.AutoGenerateColumns = false;             sfGrid.BackgroundColor = UIColor.FromRGB(236, 184, 138);             //sfGrid.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = "txid" });              GridTextColumn txIdColumn = new GridTextColumn();             txIdColumn.MappingName = "TxId";             txIdColumn.HeaderText = "tx";              GridTextColumn dateColumn = new GridTextColumn();             dateColumn.MappingName = "TradeDate";             dateColumn.HeaderText = "TDate";              GridTextColumn buysellColumn = new GridTextColumn();             buysellColumn.MappingName = "Side";             buysellColumn.HeaderText = "BuySell";              GridTextColumn amountColumn = new GridTextColumn();             amountColumn.MappingName = "Quantity";             amountColumn.HeaderText = "Qty";              GridTextColumn priceColumn = new GridTextColumn();             priceColumn.MappingName = "TradePrice";             priceColumn.HeaderText = "Price";              GridTextColumn valueColumn = new GridTextColumn();             valueColumn.MappingName = "TradeValue";             valueColumn.HeaderText = "Value";              GridTextColumn bookColumn = new GridTextColumn();             bookColumn.MappingName = "BookPrice";             bookColumn.HeaderText = "Book";              sfGrid.Columns.Add(txIdColumn);             sfGrid.Columns.Add(dateColumn);             sfGrid.Columns.Add(buysellColumn);             sfGrid.Columns.Add(amountColumn);             sfGrid.Columns.Add(priceColumn);             sfGrid.Columns.Add(valueColumn);             sfGrid.Columns.Add(bookColumn);          }          public override void ViewDidLoad()
        { 
            base.ViewDidLoad();             //NavigationItem.RightBarButtonItem = EditButtonItem;             sfGrid.HeaderRowHeight = 30;             sfGrid.RowHeight = 30;             TransactionView.AddSubview(sfGrid);              thisExchange = ApplicationCore.PublicExchangeList.First(x => x.APIReady == true);             calendaryear = DateTime.Now.Year.ToString();             ReDrawScreen();              buttonExchange.TouchUpInside += (sender, e) =>              {                 UIAlertController exchangeAlert = UIAlertController.Create("Exchange", "Choose Exchange", UIAlertControllerStyle.ActionSheet);                 exchangeAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));                  foreach (var exc in ApplicationCore.PublicExchangeList.Where(x => x.APIReady == true))                 {                     exchangeAlert.AddAction(UIAlertAction.Create(exc.Name,                                                                      UIAlertActionStyle.Default,                                                                      (obj) =>                                                                      {                                                                          buttonExchange.SetTitle(exc.Name, UIControlState.Normal);                                                                          thisExchange = exc;                                                                      }                                                                 ));                 }                 this.PresentViewController(exchangeAlert, true, null);             };              buttonCalndarYear.TouchUpInside += (sender, e) =>              {                 var thisyear = DateTime.Now.Year;                 string[] years = new string[] { thisyear.ToString(), (thisyear - 1).ToString(), (thisyear - 2).ToString(), "ALL" };                  UIAlertController calndarYearAlert = UIAlertController.Create("Calendar Year", "Choose Year", UIAlertControllerStyle.ActionSheet);                 calndarYearAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));                  foreach (var yr in years)                 {                     calndarYearAlert.AddAction(UIAlertAction.Create(yr.ToString(), UIAlertActionStyle.Default,                                                                      (obj) =>                                                                      {
                                                                         buttonCalndarYear.SetTitle(yr, UIControlState.Normal);
                                                                         calendaryear = yr == "ALL" ? null : yr;                                                                      }                                                                 ));                 }                 this.PresentViewController(calndarYearAlert, true, null);              };              buttonSearch.TouchUpInside += async (sender, e) =>
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
                            this.sfGrid.Frame = new CGRect(0, 0, TransactionView.Frame.Width, TransactionView.Frame.Height);
                        }
                        else
                        {
                            UIAlertController okAlertController = UIAlertController.Create("Warning", "Exchange API call might be timed out. try again."
                                                                                           , UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                        }
                    }
                    loadPop.Hide();
                }             };
        }          public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated); 
        }          private void ReDrawScreen()         {             if (thisExchange != null) buttonExchange.SetTitle(thisExchange.Code, UIControlState.Normal);             buttonCalndarYear.SetTitle(calendaryear.ToString(), UIControlState.Normal);          }       //void HandleAutoGeneratingColumn(object sender, AutoGeneratingColumnArgs e)        //{         //  if (e.Column.MappingName == "txid")         //  {       //      e.Column.TextMargin = 1;        //      e.Column.TextAlignment = UITextAlignment.Left;      //  }       //}      } }
