using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinBalance.CoreClass.APIClass
{
    public static class MarketDataAPI
    {
        //const string coinmarketcap_url = "https://api.coinmarketcap.com";
        static string coinbalance_url = CoinbalanceAPI.coinbalance_url;

        public const string InstrumentListFile = "InstrumentList.json";
        public const string ExchangeListFile = "ExchangeList.json";
        public const string CrossRatefile_today = "crossrate_today.json";
        public const string CrossRatefile_yesterday = "crossrate_yesterday.json";

        public static async Task FetchCoinPricesAsync(ExchangeList exchanges, InstrumentList coins, List<CrossRate> crossrates)
        {

            if (!Reachability.IsHostReachable(coinbalance_url))
            {
                throw new AppCoreNetworkException("Host is not reachable: " + coinbalance_url);
            }
            else
            {
                if (crossrates is null) crossrates = await FetchCrossRateAsync();
                var crossrate = crossrates.Any(x => x.Currency == AppCore.BaseCurrency) ?
                                          crossrates.First(x => x.Currency == AppCore.BaseCurrency) :
                                          null;
                if (crossrate is null) throw new AppCoreException("Crossrate is null");
                var usdjpy = crossrates.Any(x => x.Currency == EnuBaseFiatCCY.JPY) ? crossrates.First(x => x.Currency == EnuBaseFiatCCY.JPY) : null;

                // Go coinmarket for all instruments to fill in all market data
                await FetchCoinMarketCapAsync(coins, crossrate);

                foreach (var source in coins.Select(x => x.PriceSourceCode).Distinct())
                {

                    switch (source)
                    {
                        //Bitcoin must go first
                        case "Bittrex":
                            if (exchanges.Any(x => x.Code == "Bittrex"))
                            {
                                var exchange = exchanges.First(x => x.Code == "Bittrex");
                                if (exchange.HasListedCoins()) await BittrexAPI.FetchPriceAsync(exchange, coins);
                            }
                            break;

                        case "Bitstamp":
                            if (exchanges.Any(x => x.Code == "Bitstamp"))
                            {
                                var exchange = exchanges.First(x => x.Code == "Bitstamp");
                                if (exchange.HasListedCoins()) await BItstampAPI.FetchPriceAsync(exchange, coins);
                            }
                            break;

                        case "Zaif":
                            if (exchanges.Any(x => x.Code == "Zaif"))
                            {
                                var exchange = exchanges.First(x => x.Code == "Zaif");
                                if (exchange.HasListedCoins()) await ZaifAPI.FetchPriceAsync(exchange, coins, usdjpy);
                            }
                            break;

                        case "CoinCheck":
                            if (exchanges.Any(x => x.Code == "CoinCheck"))
                            {
                                var exchange = exchanges.First(x => x.Code == "CoinCheck");
                                if (exchange.HasListedCoins()) await CoinCheckAPI.FetchPriceAsync(exchange, coins, usdjpy);
                            }
                            break;

                        case "bitFlyer_l":
                            if (exchanges.Any(x => x.Code == "bitFlyer_l"))
                            {
                                var exchange = exchanges.First(x => x.Code == "bitFlyer_l");
                                if (exchange.HasListedCoins()) await BitFlyerAPI.FetchPriceAsync(exchange, coins, usdjpy);
                            }
                            break;

                        case "Poloniex":
                            if (exchanges.Any(x => x.Code == "Poloniex"))
                            {
                                var exchange = exchanges.First(x => x.Code == "Poloniex");
                                if (exchange.HasListedCoins()) await PoloniexAPI.FetchPriceAsync(exchange, coins);
                            }
                            break;

                        case "coinmarketcap":
                            //await FetchCoinMarketCapAsync(coins, crossrate);
                            break;

                        default:
                            throw new AppCoreNetworkException("Price API call error. Exchange: " + source);
                    }
                }
            }
        }

        private static async Task FetchCoinMarketCapAsync(InstrumentList instrumentlist, CrossRate crossrate)
        {
            string rawjson;
            string rawjson_yesterday;

            try
            {
                using (var http = new HttpClient())
                {
                    rawjson = await http.GetStringAsync(coinbalance_url + "/market/market_latest.cgi");
                        //await http.GetStringAsync(coinmarketcap_url + "/v1/ticker/?limit=0");
                }

                using (var http = new HttpClient())
                {
                    rawjson_yesterday = await http.GetStringAsync(coinbalance_url + "/market/market_yesterday.json");
                }

                await ParseAPIStrings.ParseCoinMarketCapJsonAsync(rawjson, rawjson_yesterday, instrumentlist, crossrate);

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchCoinMarketCapAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static InstrumentList FetchAllCoinData()
        {
            string rawjson;

            if (!Reachability.IsHostReachable(coinbalance_url))
            {
                throw new AppCoreNetworkException("Host is not reachable: " + coinbalance_url);
            }
            else
            {
                try
                {
                    using (var http = new HttpClient())
                    {
                        rawjson = http.GetStringAsync(coinbalance_url + "/InstrumentList.json").Result;
                    }
                    //if (rawjson is null) return (null, "json data is null");

                    StorageAPI.SaveFile(rawjson, InstrumentListFile);
                    return ParseAPIStrings.ParseInstrumentListJson(rawjson);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchAllCoinData: " + e.GetType() + ": " + e.Message);
                    throw;
                }
            }

        }

        public static async Task FetchCoinLogoAsync(string InstrumentID, bool ForceRefresh)
        {
            var filename = InstrumentID + ".png";
            string TargetUri = coinbalance_url + "/images/" + filename;

            if (!Reachability.IsHostReachable(TargetUri))
            {
                throw new AppCoreNetworkException("Host is not reachable: " + coinbalance_url);
            }
            else
            {
                try
                {
                    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Images");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    path = Path.Combine(path, filename);

                    if (!File.Exists(path) || ForceRefresh)
                    {
                        var client = new HttpClient();
                        HttpResponseMessage res = await client.GetAsync(TargetUri, HttpCompletionOption.ResponseContentRead);

                        using (var fileStream = File.Create(path))
                        using (var httpStream = await res.Content.ReadAsStreamAsync())
                            httpStream.CopyTo(fileStream);
                    }

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchCoinLogoAsync: " + e.GetType() + ": " + e.Message);
                    throw;
                }

            }
        }

        public static async Task<List<CrossRate>> FetchCrossRateAsync()
        {
            string rawjson_today, rawjson_yesterday;

            if (!Reachability.IsHostReachable(coinbalance_url))
            {
                throw new AppCoreNetworkException("Host is not reachable: " + coinbalance_url);
            }
            else
            {
                using (var http = new HttpClient())
                {
                    HttpResponseMessage res = await http.GetAsync(coinbalance_url + "/fxrate/fxrate_latest.json");
                    if (!res.IsSuccessStatusCode)
                    {
                        throw new AppCoreNetworkException("http response error. status code: " + res.StatusCode);
                    }
                    else
                    {
                        rawjson_today = await res.Content.ReadAsStringAsync();
                    }
                }

                using (var http = new HttpClient())
                {
                    HttpResponseMessage res = await http.GetAsync(coinbalance_url + "/fxrate/fxrate_yesterday.json");
                    if (!res.IsSuccessStatusCode)
                    {
                        throw new AppCoreNetworkException("http response error. status code: " + res.StatusCode);
                    }
                    else
                    {
                        rawjson_yesterday = await res.Content.ReadAsStringAsync();
                    }
                }

            }

            var crossrates = await ParseAPIStrings.ParseCrossRateJsonAsync(rawjson_today, rawjson_yesterday);
            StorageAPI.SaveFile(rawjson_today, CrossRatefile_today);
            StorageAPI.SaveFile(rawjson_yesterday, CrossRatefile_yesterday);

            return crossrates;

        }

        //public static async Task<Price> FetchPriceBefore24Async(string instrumentid)
        //{
        //    string rawjson_yesterday;

        //    try
        //    {
        //        using (var http = new HttpClient())
        //        {
        //            rawjson_yesterday = await http.GetStringAsync(coinbalance_url + "/market/market_yesterday.json");
        //        }
        //    }
        //    catch (HttpRequestException e)
        //    {
        //        throw new AppCoreNetworkException("Http connection error: " + e.Message);
        //    }

        //    var price = new Price(AppCore.InstrumentList.GetByInstrumentId(instrumentid));
        //    try
        //    {
        //        var jarray_yesterday = await Task.Run(() => JArray.Parse(rawjson_yesterday));

        //        price.LatestPriceBTC = (double)jarray_yesterday.SelectToken("[?(@.id == '" + instrumentid + "')]")["price_btc"];
        //        price.LatestPriceUSD = (double)jarray_yesterday.SelectToken("[?(@.id == '" + instrumentid + "')]")["price_usd"];
        //        price.PriceSource = "coinmarketcap";
        //        price.DayVolume = 0; 
        //        price.MarketCap = (double)jarray_yesterday.SelectToken("[?(@.id == '" + instrumentid + "')]")["market_cap_usd"];
        //        price.PriceDate = DateTime.Now;
        //        //price.PriceBTCBefore24h = (double)jarray_yesterday.SelectToken("[?(@.id == '" + coin.Id + "')]")["price_btc"];
        //        //price.PriceUSDBefore24h = (double)jarray_yesterday.SelectToken("[?(@.id == '" + coin.Id + "')]")["price_usd"];
        //        //price.USDCrossRate = crossrate;

        //        return price;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new AppCoreParseException("Exception during parsing coinmarketcap Json: " + e.Message);
        //    }

        //}
    }
}
