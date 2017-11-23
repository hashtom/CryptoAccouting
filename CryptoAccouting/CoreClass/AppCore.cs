﻿using System; using System.Linq; using System.Threading.Tasks; using System.Collections.Generic; using CoinBalance.CoreClass.APIClass;  namespace CoinBalance.CoreClass {     public static class AppCore     {         public const string AppName = "CoinBalance";          public static Balance Balance { get; private set; }         public static InstrumentList InstrumentList { get; private set; }
        public static ExchangeList PublicExchangeList { get; private set; }         public static CoinStorageList CoinStorageList { get; private set; }         private static List<CrossRate> USDCrossRates;         private static EnuBaseFiatCCY baseCurrency;          public static EnuBaseFiatCCY BaseCurrency
        {             get
            {
                return baseCurrency;
            }             set
            {
                baseCurrency = value;
            }         }          public static CrossRate USDCrossRate         {             get             {                 return USDCrossRates.First(x => x.Currency == baseCurrency);             }         }          public static Instrument Bitcoin         {             get
            {                 return InstrumentList.GetByInstrumentId("bitcoin");
            }         }

        //public static NavigationDrawer Navigation { get; set; }

        //public static NavigationDrawer InitializeSlideMenu(UIView BalanceTableView,
        //                                                   UITableViewController PositionViewC,
        //                                                   UIViewController TransactionViewC,
        //                                                   UITableViewController PLViewC,
        //                                                   UIViewController PerfViewC,
        //                                                   UIViewController SettingViewC)
        //{
        //    Navigation = new NavigationDrawer(BalanceTableView.Frame.Width, BalanceTableView.Frame.Height,
        //                                      PositionViewC,
        //                                      TransactionViewC,
        //                                      PLViewC,
        //                                      PerfViewC,
        //                                      SettingViewC);
        //    Navigation.AddView(BalanceTableView);
        //    return Navigation;
        //}

        public static void InitializeCore()
        {

            //Initialize 
            CoinStorageList = new CoinStorageList(); 
            try             {                 //try
                //{                 InstrumentList = StorageAPI.LoadInstrument(); //MarketDataAPI.FetchAllCoinData();                 //}
                //catch (Exception e)                 //{                     //System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": InitializeCore: Failed to read InstrumentList: " + e.GetType() + ": " + e.Message);
                    //InstrumentList = StorageAPI.LoadInstrument(); // Load Bundle file
                //}                 //LoadExchangeList();                  //if (PublicExchangeList is null) PublicExchangeList = new ExchangeList();                 PublicExchangeList = StorageAPI.LoadExchangeList();                  Balance = StorageAPI.LoadBalanceXML(InstrumentList);                 RefreshBalance();                  try
                {
                    //Load App Configuration + API keys
                    StorageAPI.LoadAppSettingXML();                 }                 catch (Exception e)                 {                     BaseCurrency = EnuBaseFiatCCY.USD; //Default setting                     System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": InitializeCore: Failed to read AppSettingfile" + e.GetType() + ": " + e.Message);                     //throw new AppCoreWarning(e.Message);                 }             }             catch (AppCoreBalanceException e)             {                 System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": InitializeCore: " + e.GetType() + ": " + e.Message);                 Balance = new Balance();                 //throw;             }             catch (Exception e)             {                 System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": InitializeCore: " + e.GetType() + ": " + e.Message);                 throw;             }          }          public static async Task LoadCrossRateAsync()
        {             try             {
                USDCrossRates = await MarketDataAPI.FetchCrossRateAsync();
            }             catch(Exception e)             {                 USDCrossRates = await StorageAPI.LoadCrossRateAsync();                 System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": LoadCrossRateAsync(continued with file): " + e.GetType() + ": " + e.Message);             }         } 
        public static async Task FetchMarketDataFromBalanceAsync()
        { 
            if (Balance != null)
            {                 try                 {
                    var mycoins = new InstrumentList(); 
                    Balance.Select(x => x.Coin).Distinct().ToList().ForEach(x => mycoins.Attach(x));
                    //if (!mycoins.Any(x => x.Id == "bitcoin")) mycoins.DetachByInstrumentId("bitcoin");
                    //mycoins.Insert(0, Bitcoin); 
                    await MarketDataAPI.FetchCoinPricesAsync(PublicExchangeList, mycoins, USDCrossRates);                      Balance.PriceDateTime = DateTime.Now;                     RefreshBalance(); //update weights,etc with latest price                     //SaveMyBalanceXML();//save balance with latest price                  }                 catch(Exception e)                 {                     System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchMarketDataFromBalanceAsync: " + e.GetType() + ": " + e.Message);                     throw;                 }              }
            else
            {                 throw new AppCoreBalanceException("Balance object is null");             }
        }          public static void SaveAppSetting()
        {             StorageAPI.SaveAppSettingXML(AppName, BaseCurrency, PublicExchangeList);         }          //private static void LoadExchangeList()
        //{         //    ExchangeAPI.FetchExchangeList(PublicExchangeList);         //}          public static void SyncLatestCoins()         {             try             {
                InstrumentList = MarketDataAPI.FetchAllCoinData();                 PublicExchangeList = ExchangeAPI.FetchExchangeList();                  if (Balance != null) Balance.AttachInstruments(InstrumentList);
                Task.Run(async () => await FetchCoinLogoTop100Async());             }
            catch (Exception e)             {                 System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": SyncLatestCoins: " + e.GetType() + ": " + e.Message);                 throw;             }         }          public static async Task FetchCoinLogoAsync(Instrument coin)         {              await MarketDataAPI.FetchCoinLogoAsync(coin.Id, false);         }          public static async Task FetchCoinLogoFromBalanceAsync()         {             foreach (var pos in Balance)             {
                await MarketDataAPI.FetchCoinLogoAsync(pos.Coin.Id, false);             }         }          public static async Task FetchCoinLogoTop100Async()         {             if (InstrumentList == null)
            {
                throw new AppCoreException("InstrumentList is null.");             }
            else
            {
                foreach (var coin in InstrumentList.Where(x=>x.rank <= 100))
                {
                    await MarketDataAPI.FetchCoinLogoAsync(coin.Id, false);
                }             }         }          public static void SavePriceSourceXML()         {             StorageAPI.SavePriceSourceXML(InstrumentList);         }          public static void SaveMyBalanceXML()
        {             StorageAPI.SaveBalanceXML(Balance);         }          public static async Task<TradeList> LoadTradeListsAsync(Exchange exchange)
        {
            //var exchange = GetExchange(ExchangeCode);             //var apikey = APIKeys.Where(x => x.ExchangeType == extype).First();              if (exchange.APIKeySaved())             {                 return await ExchangeAPI.FetchTradeListAsync(exchange);                 //exchange.AttachTradeList(await ExchangeAPI.FetchTradeListAsync(exchange));                 //PublicExchangeList.Attach(exchange); //do you need?             }else             {                 throw new AppCoreException("API Keys are not saved.");             }         }          public static async Task<List<Position>> LoadPositionAsync(Exchange exchange)         {             if (exchange.APIKeySaved())             {
                return await ExchangeAPI.FetchPositionAsync(exchange);
            }
            else             {                 throw new AppCoreException("API Keys are not saved.");             }         }          public static Exchange GetExchange(string Code)
        {             return PublicExchangeList.GetExchange(Code);         }

        public static ExchangeList GetExchangeListByInstrument(string id)
        {
            return PublicExchangeList.GetExchangesByInstrument(id);
        }          //public static TradeList GetExchangeTradeList(string exchangeCode)
        //{         //    return PublicExchangeList.GetTradelist(exchangeCode);
        //}
         public static long ToEpochSeconds(DateTime dt)         {             var dto = new DateTimeOffset(dt.Ticks, new TimeSpan(+09, 00, 00));             return dto.ToUnixTimeSeconds();         } 
        public static DateTime FromEpochSeconds(long EpochSeconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(EpochSeconds);

        }          public static void RefreshBalance()         {             //if (InstrumentList != null) InstrumentList.AttachCrossRate(USDCrossRate);             Balance.RefreshBalanceData();             CoinStorageList.RecalculateWeights();         }          public static void AttachPosition(Position position, bool DoRefreshBalance = true)         {             Balance.Attach(position);             if (DoRefreshBalance) RefreshBalance();             Task.Run(async () => await FetchCoinLogoAsync(position.Coin));         }          public static void DetachPosition(Position position, bool DoRefreshBalance = true)         {             Balance.Detach(position);             CoinStorageList.DetachPosition(position);             if (DoRefreshBalance) RefreshBalance();         }          public static void DetachPositionByCoin(string InstrumentId, bool DoRefreshBalance = true)         {             Balance.DetachPositionByCoin(InstrumentId);             CoinStorageList.DetachPositionByCoin(InstrumentId);             if (DoRefreshBalance) RefreshBalance();         }          private static void DetachPositionByStorage(CoinStorage storage)         {             Balance.DetachPositionByStorage(storage);             storage.DetachPositionByStorage(storage);             //CoinStorageList.Detach(exchange);         }          public static void AttachPositionByStorage(CoinStorage storage, List<Position> positions, bool DoRefreshBalance = true)         {                          DetachPositionByStorage(storage);              foreach (var pos in positions)             {                 AttachCoinStorage(storage.Code, storage.StorageType, pos);                 //pos.AttachCoinStorage(storage);                 //storage.AttachPosition(pos);                 AttachPosition(pos, false);             }             if (DoRefreshBalance) RefreshBalance();         } 
        public static async Task FetchMarketDataAsync(Instrument coin)
        {
            //await MarketDataAPI.FetchCoinMarketDataAsync(coin);
            var mycoins = new InstrumentList();             //if (coin.Symbol1 != "BTC") mycoins.Attach(InstrumentList.First(x => x.Symbol1 == "BTC"));             mycoins.Attach(coin);             await MarketDataAPI.FetchCoinPricesAsync(PublicExchangeList, mycoins, USDCrossRates);
        }          public static CoinStorage GetCoinStorage(string storagecode, EnuCoinStorageType storagetype)         {             return CoinStorageList.Any(x => (x.Code == storagecode && x.StorageType == storagetype))
                                  ? CoinStorageList.First(x => (x.Code == storagecode && x.StorageType == storagetype))                                       : null;         }          public static void AttachCoinStorage(string storagecode, EnuCoinStorageType storagetype, Position pos)         {             //CoinStorageList.DetachPosition(pos);             var storage = GetCoinStorage(storagecode, storagetype);
            if (storage is null)             {
                switch (storagetype)
                {
                    case EnuCoinStorageType.Exchange:                         storage = GetExchange(storagecode);                         CoinStorageList.Attach(storage);
                        break;
                    default:                         storage = new Wallet(storagecode, storagetype);                         CoinStorageList.Attach(storage);
                        break;
                }
            }              storage.AttachPosition(pos);             pos.AttachCoinStorage(storage);         }          public static CrossRate GetCrossRate(EnuBaseFiatCCY CCY)         {             return USDCrossRates is null ? null : USDCrossRates.First(x => x.Currency == CCY);         } 
		public static bool IsInternetReachable()
		{
            return Reachability.IsHostReachable(CoinbalanceAPI.coinbalance_url);
		} 
        public static string NumberFormat(double number, bool percent = false, bool digitAdjust = true, string symbol=null)
        {
            var digit = unchecked((int)Math.Log10(Math.Abs(number))) + 1;
            string strnumber;              if (Math.Abs(number) < double.Epsilon)
            {
                strnumber = "--";
            }
            else             {                 if (percent)                 {                     strnumber = String.Format("{0:n2}", number);                     strnumber = number > 0 ? "+" + strnumber : strnumber;                 }                 else if (!digitAdjust)                 {
                    strnumber = String.Format("{0:n0}", number);
                }                 else                 {

                    if (digit > 6)
                    {
                        strnumber = String.Format("{0:n2}", number / 1000000) + "MM";

                    }
                    else if (digit > 3)
                    {
                        strnumber = String.Format("{0:n0}", number);

                    }
                    else if (digit <= 1)
                    {
                        strnumber = Math.Abs(number) < double.Epsilon ? "0" : String.Format("{0:n7}", number);
                    }
                    else
                    {
                        strnumber = String.Format("{0:n2}", number);
                    } 
                }                 if (symbol != null) strnumber = symbol + " " + strnumber;             }              return strnumber;
        }          public static void RemoveAllCache()         {             //Base Currency
            baseCurrency = EnuBaseFiatCCY.USD;              //Clear API keys
            PublicExchangeList.ClearAPIKeys();              //Clear pricesource             InstrumentList.ToList().ForEach(x=>x.ClearAttributes());              //Clear Position Attributes             //CoinStorageList.Clear();             //foreach(var pos in Balance)             //{             //    pos.ClearAttributes();             //    if (pos.CoinStorage != null) AttachCoinStorage(pos.CoinStorage.Code, pos.CoinStorage.StorageType, pos);             //}             //RefreshBalance();              //Modify & Remove files             StorageAPI.SaveBalanceXML(Balance);
            StorageAPI.RemoveAllCache();         }      }  } 