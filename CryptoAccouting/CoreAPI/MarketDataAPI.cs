using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CoinBalance.CoreModel;

namespace CoinBalance.CoreAPI
{
    public static class MarketDataAPI
    {
        const string coinmarketcap_url = "https://api.coinmarketcap.com";
        static string coinbalance_url = CoinbalanceAPI.coinbalance_url;

        public const string InstrumentListFile = "InstrumentList.json";
        public const string ExchangeListFile = "ExchangeList.json";
        public const string CrossRatefile_today = "crossrate_today.json";
        public const string CrossRatefile_yesterday = "crossrate_yesterday.json";

        public static async Task FetchCoinPricesAsync(ExchangeList exchanges, InstrumentList coins, List<CrossRate> crossrates)
        {
            try
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
                                if (exchange.HasListedCoins()) await ZaifAPI2.FetchPriceAsync(exchange, coins, usdjpy);
                            }
                            break;

                        case "CoinCheck":
                            if (exchanges.Any(x => x.Code == "CoinCheck"))
                            {
                                var exchange = exchanges.First(x => x.Code == "CoinCheck");
                                if (exchange.HasListedCoins()) await CoinCheckAPI2.FetchPriceAsync(exchange, coins, usdjpy);
                            }
                            break;

                        case "bitFlyer_l":
                            if (exchanges.Any(x => x.Code == "bitFlyer_l"))
                            {
                                var exchange = exchanges.First(x => x.Code == "bitFlyer_l");
                                if (exchange.HasListedCoins()) await BitFlyerAPI2.FetchPriceAsync(exchange, coins, usdjpy);
                            }
                            break;

                        case "Poloniex":
                            if (exchanges.Any(x => x.Code == "Poloniex"))
                            {
                                var exchange = exchanges.First(x => x.Code == "Poloniex");
                                if (exchange.HasListedCoins()) await PoloniexAPI.FetchPriceAsync(exchange, coins);
                            }
                            break;

                        case "Bitfinex":
                            if (exchanges.Any(x => x.Code == "Bitfinex"))
                            {
                                var exchange = exchanges.First(x => x.Code == "Bitfinex");
                                if (exchange.HasListedCoins()) await BitfinexAPI.FetchPriceAsync(exchange, coins);
                            }
                            break;

                        case "HitBTC":
                            if (exchanges.Any(x => x.Code == "HitBTC"))
                            {
                                var exchange = exchanges.First(x => x.Code == "HitBTC");
                                if (exchange.HasListedCoins()) await HitbtcAPIAPI.FetchPriceAsync(exchange, coins);
                            }
                            break;

                        case "Quoine":
                            if (exchanges.Any(x => x.Code == "Quoine"))
                            {
                                var exchange = exchanges.First(x => x.Code == "Quoine");
                                if (exchange.HasListedCoins()) await QuoineAPI.FetchPriceAsync(exchange, coins);
                            }
                            break;

                        case "coinmarketcap":
                            //await FetchCoinMarketCapAsync(coins, crossrate);
                            break;

                        default:
                            throw new AppCoreWarning("Please update to the newest version! (" + source + ")");
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchCoinPricesAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        private static async Task FetchCoinMarketCapAsync(InstrumentList instrumentlist, CrossRate crossrate)
        {
            string rawjson, rawjson_yesterday;

            try
            {
                using (var http = new HttpClient())
                {
                    rawjson_yesterday = await http.GetStringAsync(coinbalance_url + "/market/market_yesterday.json");
                    //btcjson = await http.GetStringAsync(coinmarketcap_url + "/v1/ticker/bitcoin");

                    rawjson = await http.GetStringAsync(coinmarketcap_url + "/v1/ticker/?limit=300");
                    await ParseAPIStrings.ParseCoinMarketCapJsonAsync(rawjson, rawjson_yesterday, instrumentlist, crossrate);

                    foreach (var coin in instrumentlist.Where(x=>x.rank is int.MaxValue))
                    {
                        //rawjson = await http.GetStringAsync(coinbalance_url + "/market/market_latest.cgi");
                        var rawcoinjson = await http.GetStringAsync(coinmarketcap_url + "/v1/ticker/" + coin.Id);

                        await ParseAPIStrings.ParseCoinMarketCapJsonAsync(rawjson, rawcoinjson, rawjson_yesterday, coin, crossrate);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchCoinMarketCapAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        //private static async Task FetchCoinMarketCapAsync(Instrument coin, CrossRate crossrate)
        //{
        //    string rawjson;
        //    string rawjson_yesterday;

        //    try
        //    {
        //        using (var http = new HttpClient())
        //        {
        //            rawjson = await http.GetStringAsync(coinmarketcap_url + "/v1/ticker/" + coin.Id);
        //        }

        //        using (var http = new HttpClient())
        //        {
        //            rawjson_yesterday = await http.GetStringAsync(coinbalance_url + "/market/market_yesterday.json");
        //        }

        //        var instrumentlist = new InstrumentList();
        //        instrumentlist.Attach(coin);

        //        await ParseAPIStrings.ParseCoinMarketCapJsonAsync(rawjson, rawjson_yesterday, instrumentlist, crossrate);

        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchCoinMarketCapAsync: " + e.GetType() + ": " + e.Message);
        //        throw;
        //    }
        //}

        public static InstrumentList FetchAllCoinData()
        {
            string rawjson;

            try
            {
                using (var http = new HttpClient())
                {
                    rawjson = http.GetStringAsync(coinbalance_url + "/InstrumentList.json").Result;
                }

                var InstrumentList = ParseAPIStrings.ParseInstrumentListJson(rawjson);

                StorageAPI.LoadPriceSource(InstrumentList);
                StorageAPI.SaveFile(rawjson, InstrumentListFile);
                return InstrumentList;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchAllCoinData: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task FetchCoinLogoAsync(string InstrumentID, bool ForceRefresh)
        {
            var filename = InstrumentID + ".png";
            string TargetUri = coinbalance_url + "/images/" + filename;

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
                    using (var http = new HttpClient())
                    {
                        var res = await http.GetAsync(TargetUri, HttpCompletionOption.ResponseContentRead);
                        res.EnsureSuccessStatusCode();

                        using (var fileStream = File.Create(path))
                        using (var httpStream = await res.Content.ReadAsStreamAsync())
                            httpStream.CopyTo(fileStream);
                    }   
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchCoinLogoAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        public static async Task<List<CrossRate>> FetchCrossRateAsync()
        {
            string rawjson_today, rawjson_yesterday;

            try
            {
                using (var http = new HttpClient())
                {
                    var res = await http.GetAsync(coinbalance_url + "/fxrate/fxrate_latest.json");
                    res.EnsureSuccessStatusCode();
                    rawjson_today = await res.Content.ReadAsStringAsync();
                }

                using (var http = new HttpClient())
                {
                    var res = await http.GetAsync(coinbalance_url + "/fxrate/fxrate_yesterday.json");
                    res.EnsureSuccessStatusCode();
                    rawjson_yesterday = await res.Content.ReadAsStringAsync();
                }
            }
            catch (HttpRequestException e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchCrossRateAsync: fetching cross rates: " + e.GetType() + ": " + e.Message);
                throw;
            }

            try
            {
                var crossrates = await ParseAPIStrings.ParseCrossRateJsonAsync(rawjson_today, rawjson_yesterday);
                StorageAPI.SaveFile(rawjson_today, CrossRatefile_today);
                StorageAPI.SaveFile(rawjson_yesterday, CrossRatefile_yesterday);
                return crossrates;
            }catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchCrossRateAsync: Parsing Cross rate: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

    }
}
