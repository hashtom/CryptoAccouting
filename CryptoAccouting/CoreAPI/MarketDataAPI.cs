using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CoinBalance.CoreModel;
using RestSharp;

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

        static List<CMCTicker> data_yesterday;
        static List<CMCTicker> rankData;


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

                        case "Binance":
                            if (exchanges.Any(x => x.Code == "Binance"))
                            {
                                var exchange = exchanges.First(x => x.Code == "Binance");
                                if (exchange.HasListedCoins()) await BinanceAPI.FetchPriceAsync(exchange, coins);
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
            int rank = 200;

            try
            {
                
                if (data_yesterday is null)
                {
                    data_yesterday = await GetMarketDataYesterday();
                }
                else
                {
                    //if(Util.FromEpochSeconds(data_yesterday.First().last_updated) < DateTime.Now.AddSeconds(-1800))
                    if (data_yesterday.First().last_updated < DateTime.Now.AddSeconds(-1800))
                    {
                        data_yesterday = await GetMarketDataYesterday();
                    }
                }

                if (rankData is null)
                {
                    rankData = await GetMarketDataRankLatest(rank);
                }
                else
                {
                    //if (Util.FromEpochSeconds(rankData.First().last_updated) < DateTime.Now.AddSeconds(-60))
                    if (rankData.First().last_updated.ToLocalTime() < DateTime.Now.AddSeconds(-600))
                    {
                        rankData = await GetMarketDataRankLatest(rank);
                    }
                    else
                    {
                        return;
                    }

                }

                var data_btc = rankData.First(x => x.id == "bitcoin");

                foreach (var coin in instrumentlist)
                {
                    var data_latest = rankData.Any(x => x.id == coin.Id) ? rankData.First(x => x.id == coin.Id) : await GetMarketDataLatest(coin.Id);
                    //var data_latest = rankData.Any(x => x.id == coin.Id) ? rankData.First(x => x.id == coin.Id) : null;

                    if (data_latest != null)
                    {
                        if (coin.MarketPrice == null)
                        {
                            var p = new Price(coin);
                            coin.MarketPrice = p;
                        }

                        coin.MarketPrice.LatestPriceBTC = (double)data_latest.price_btc;
                        coin.MarketPrice.LatestPriceUSD = (double)data_latest.price_usd;
                        coin.MarketPrice.PriceSource = "coinmarketcap";
                        coin.MarketPrice.DayVolume = data_btc != null ? (double)data_latest._24h_volume_usd / (double)data_btc.price_usd : 0;
                        coin.MarketPrice.MarketCap = data_latest.market_cap_usd != null ? (double)data_latest.market_cap_usd : 0;

                        if (data_yesterday.Any(x=>x.id == coin.Id))
                        {
                            coin.MarketPrice.PriceBTCBefore24h = (double)data_yesterday.First(x=>x.id == coin.Id).price_btc;
                            coin.MarketPrice.PriceUSDBefore24h = (double)data_yesterday.First(x => x.id == coin.Id).price_usd;
                        }

                        //coin.MarketPrice.PriceDate = Util.UnixTimeStampToDateTime(data_latest.last_updated);
                        coin.MarketPrice.PriceDate = data_latest.last_updated;
                        coin.MarketPrice.USDCrossRate = crossrate;
                        coin.rank = data_latest.rank;
                    }
                    else
                    {
                        coin.rank = int.MaxValue;
                        System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " :FetchCoinMarketCapAsync: Failed to update " + coin.Name + " price.");
                        //throw new AppCoreParseException("Failed to update " + coin.Name + " price.");
                    }
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchCoinMarketCapAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        //private static async Task FetchCoinMarketCapAsync_old(InstrumentList instrumentlist, CrossRate crossrate)
        //{
        //    string rawjson, rawjson_yesterday;

        //    try
        //    {
        //        using (var http = new HttpClient())
        //        {
        //            rawjson_yesterday = await http.GetStringAsync(coinbalance_url + "/market/market_yesterday.json");
        //            //btcjson = await http.GetStringAsync(coinmarketcap_url + "/v1/ticker/bitcoin");

        //            rawjson = await http.GetStringAsync(coinmarketcap_url + "/v1/ticker/?limit=20");
        //            await ParseAPIStrings.ParseCoinMarketCapJsonAsync(rawjson, rawjson_yesterday, instrumentlist, crossrate);

        //            foreach (var coin in instrumentlist.Where(x=>x.rank is int.MaxValue))
        //            {
        //                //rawjson = await http.GetStringAsync(coinbalance_url + "/market/market_latest.cgi");
        //                var rawcoinjson = await http.GetStringAsync(coinmarketcap_url + "/v1/ticker/" + coin.Id);

        //                await ParseAPIStrings.ParseCoinMarketCapJsonAsync(rawjson, rawcoinjson, rawjson_yesterday, coin, crossrate);
        //            }
        //        }
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

        private static async Task<CMCTicker> GetMarketDataLatest(string InstrumrntId)
        {
            var client = new RestClient(new Uri(coinmarketcap_url));

            try
            {
                var path = $"/v1/ticker/{InstrumrntId}";
                var req = RestUtil.CreateJsonRestRequest(path);

                var results = await client.ExecuteTaskAsync<List<CMCTicker>>(req);
                if (results.ErrorException != null)
                {
                    throw results.ErrorException;
                }

                return results.Data.First();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": GetMarketDataLatest: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        private static async Task<List<CMCTicker>> GetMarketDataRankLatest(int rank)
        {
            var client = new RestClient(new Uri(coinmarketcap_url));

            try
            {
                var path = $"/v1/ticker/?limit={rank}";
                var req = RestUtil.CreateJsonRestRequest(path);

                var results = await client.ExecuteTaskAsync<List<CMCTicker>>(req);
                if (results.ErrorException != null)
                {
                    throw results.ErrorException;
                }

                return results.Data;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": GetMarketDataLatest: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        private static async Task<List<CMCTicker>> GetMarketDataYesterday()
        {
            var client = new RestClient(new Uri(coinbalance_url));

            try
            {
                var path = "/market/market_yesterday.json";
                var req = RestUtil.CreateJsonRestRequest(path);

                var results = await client.ExecuteTaskAsync<List<CMCTicker>>(req);
                if (results.ErrorException != null)
                {
                    throw results.ErrorException;
                }

                var data = results.Data;
                //data.ForEach(x => x.last_updated = Util.ToEpochSeconds(DateTime.Now));
                data.ForEach(x => x.last_updated = DateTime.Now);

                return data;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": GetMarketDataYesterday: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

    }
}
