using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoinBalance.CoreModel;

namespace CoinBalance.CoreAPI
{
    public static class ExchangeAPI
    {

        const string ExchangeListfile = MarketDataAPI.ExchangeListFile;

        public static ExchangeList FetchExchangeList()
        {
            string coinbalance_url = CoinbalanceAPI.coinbalance_url;
            string rawjson;

            try
            {
                using (var http = new HttpClient())
                {
                    var res = http.GetAsync(coinbalance_url + "/" + ExchangeListfile).Result;
                    res.EnsureSuccessStatusCode();
                    rawjson = res.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchExchangeList: " + e.GetType() + ": " + e.Message);
                throw;
                //rawjson = StorageAPI.LoadFromFile(ExchangeListfile);
                //if (rawjson == null) rawjson = StorageAPI.LoadBundleFile(ExchangeListfile);
            }

            try
            {
                var exchangelist = ParseAPIStrings.ParseExchangeListJson(rawjson);
                StorageAPI.SaveFile(rawjson, ExchangeListfile);
                return exchangelist;
            }
            catch(AppCoreInstrumentException e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchExchangeList(Process terminated): " + e.GetType() + ": " + e.Message);
                throw;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchExchangeList(Fatal Error): " + e.GetType() + ": " + e.Message);
                throw new AppCoreExchangeException(e.GetType() + ": " + e.Message);
                //try
                //{
                //    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchExchangeList(process continued with file): " + e.GetType() + ": " + e.Message);
                //    ParseAPIStrings.ParseExchangeListJson(StorageAPI.LoadBundleFile(ExchangeListfile), exlist);
                //}
                //catch (Exception ex)
                //{
                //    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchExchangeList(Fatal Error): " + e.GetType() + ": " + e.Message);
                //    throw new AppCoreExchangeException(ex.GetType() + ": " + ex.Message);
                //}
            }

        }

        internal static async Task<TradeList> FetchTradeListAsync(Exchange exchange, int CalendarYear = 0)
        {

            try
            {
                switch (exchange.Code)
                {
                    case "Zaif":
                        return await ZaifAPI2.FetchTransactionAsync(exchange, CalendarYear);

                    case "CoinCheck":
                        return await CoinCheckAPI2.FetchTransactionAsync(exchange, CalendarYear);

                    case "Bittrex":
                        return await BittrexAPI.FetchTransactionAsync(exchange, CalendarYear);

                    case "Bitstamp":
                        return await BItstampAPI.FetchTransactionAsync(exchange, CalendarYear);

                    case "Poloniex":
                        return await PoloniexAPI.FetchTransactionAsync(exchange, CalendarYear);

                    case "bitFlyer_l":
                        return await BitFlyerAPI2.FetchTransactionAsync(exchange, CalendarYear);

                    case "Quoine":
                        return await QuoineAPI.FetchExecutionAsync(exchange, CalendarYear);
                        //return await QuoineAPI.FetchTransactionAsync(exchange);
                    case "Binance":
                        return await BinanceAPI.FetchTransactionAsync(exchange, CalendarYear);

                    default:
                        throw new AppCoreWarning($"Please update to the newest version to use {exchange.Name}");
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchTradeListAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        internal static async Task<List<Position>> FetchPositionAsync(Exchange exchange)
        {
            
            try
            {
                switch (exchange.Code)
                {
                    case "Zaif":
                        return await ZaifAPI2.FetchPositionAsync(exchange);

                    case "CoinCheck":
                        return await CoinCheckAPI2.FetchPositionAsync(exchange);

                    case "Bittrex":
                        return  await BittrexAPI.FetchPositionAsync(exchange);

                    case "Bitstamp":
                        return await BItstampAPI.FetchPositionAsync(exchange);

                    case "Poloniex":
                        return await PoloniexAPI.FetchPositionAsync(exchange);

                    case "bitFlyer_l":
                        return await BitFlyerAPI2.FetchPositionAsync(exchange);

                    case "Quoine":
                        return await QuoineAPI.FetchPositionAsync(exchange);

                    case "Binance":
                        return await BinanceAPI.FetchPositionAsync(exchange);

                    default:
                        throw new AppCoreWarning($"Please update to the newest version to use {exchange.Name}");
                }

            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPositionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

    }

}
