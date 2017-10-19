﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class MarketDataAPI
    {
        const string coinbalance_url = "https://coinbalance.jp";
        const string coinmarketcap_url = "https://api.coinmarketcap.com";

        public const string InstrumentListFile = "InstrumentList.json";
        public const string ExchangeListFile = "ExchangeList.json";
        public const string CrossRatefile_today = "crossrate_today.json";
        public const string CrossRatefile_yesterday = "crossrate_yesterday.json";

        public static async Task<EnuAPIStatus> FetchCoinPricesAsync(ExchangeList exchanges, InstrumentList coins, List<CrossRate> crossrates)
		{

            if (!Reachability.IsHostReachable(coinbalance_url))
            {
                return EnuAPIStatus.FailureNetwork;
            }
            else
            {
                if (crossrates is null) crossrates = await FetchCrossRateAsync();
                var crossrate = crossrates.Any(x => x.Currency == ApplicationCore.BaseCurrency) ?
                                          crossrates.First(x => x.Currency == ApplicationCore.BaseCurrency) :
                                          null;
                if (crossrate is null) return EnuAPIStatus.FatalError;

                var usdjpy = crossrates.Any(x => x.Currency == EnuBaseFiatCCY.JPY) ?
                                       crossrates.First(x => x.Currency == EnuBaseFiatCCY.JPY) :
                                       null;
                
                foreach (var source in coins.Select(x => x.PriceSourceCode).Distinct())
                {
                    
                    switch (source)
                    {
                        //Bitcoin must go first
                        case "Bittrex":
                            if(exchanges.Any(x => x.Code == "Bittrex")) 
                                await BittrexAPI.FetchPriceAsync(exchanges.First(x => x.Code == "Bittrex"), coins, crossrate);
                            break;

                        case "Bitstamp":
                            if(exchanges.Any(x => x.Code == "Bitstamp")) 
                            await BItstampAPI.FetchPriceAsync(exchanges.First(x => x.Code == "Bitstamp"), coins, crossrate);
                            break;

                        case "Zaif":
                            if (exchanges.Any(x => x.Code == "Zaif"))
                                await ZaifAPI.FetchPriceAsync(exchanges.First(x => x.Code == "Zaif"), coins, crossrate, usdjpy);
                            break;

                        case "CoinCheck":
                            if (exchanges.Any(x => x.Code == "CoinCheck"))
                                await CoinCheckAPI.FetchPriceAsync(exchanges.First(x => x.Code == "CoinCheck"), coins, crossrate, usdjpy);
                            break;

                        case "coinmarketcap":
                            await FetchCoinMarketCapAsync(coins, crossrate);
                            break;

                        default:
                            break;
                    }
                }

                return EnuAPIStatus.Success;
            }
		}

        public static async Task<EnuAPIStatus> FetchCoinMarketCapAsync(InstrumentList instrumentlist, CrossRate crossrate)
        {
            string rawjson;
            string rawjson_yesterday;

            if (!Reachability.IsHostReachable(coinmarketcap_url))
            {
                return EnuAPIStatus.FailureNetwork;
            }
            else
            {
                try
                {
                    using (var http = new HttpClient())
                    {
                        rawjson = await http.GetStringAsync(coinbalance_url + "/v1/ticker/");
                    }

                    using (var http = new HttpClient())
                    {
                        rawjson_yesterday = await http.GetStringAsync(coinbalance_url + "/market/market_yesterday.json");
                    }
                }
                catch (HttpRequestException)
                {
                    return EnuAPIStatus.FailureNetwork;
                }

                return await ParseMarketData.ParseCoinMarketCapJsonAsync(rawjson, rawjson_yesterday, instrumentlist, crossrate);

            }

        }

        public static InstrumentList FetchAllCoinData()
		{
            string rawjson;

            if (!Reachability.IsHostReachable(coinbalance_url))
            {
                return null;
            }
            else
            {
                using (var http = new HttpClient())
                {
                    rawjson = http.GetStringAsync(coinbalance_url + "/InstrumentList.json").Result;
                }
            }

            if (rawjson is null)
            {
                return null;
            }
            else
            {
                StorageAPI.SaveFile(rawjson, InstrumentListFile);

                try
                {
                    return ParseMarketData.ParseInstrumentListJson(rawjson);
                }
                catch (Exception)
                {
                    Console.WriteLine("Parse error: ParseInstrumentListJson");
                    return null;
                }
            }
		}

        public static async Task<EnuAPIStatus> FetchCoinLogoAsync(string InstrumentID, bool ForceRefresh)
        {
            var filename = InstrumentID + ".png";
            string TargetUri = coinbalance_url + "/images/" + filename;

            if (!Reachability.IsHostReachable(TargetUri))
            {
                return EnuAPIStatus.FailureNetwork;
            }
            else
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
                return EnuAPIStatus.Success;
            }
        }

        public static async Task<List<CrossRate>> FetchCrossRateAsync()
        {
            string rawjson_today, rawjson_yesterday;

            if (!Reachability.IsHostReachable(coinbalance_url))
            {
                return null;
            }
            else
            {
                using (var http = new HttpClient())
                {
                    HttpResponseMessage response = await http.GetAsync(coinbalance_url + "/fxrate/fxrate_latest.json");
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }
                    else
                    {
                        rawjson_today = await response.Content.ReadAsStringAsync();
                    }
                }

                using (var http = new HttpClient())
                {
                    HttpResponseMessage response = await http.GetAsync(coinbalance_url + "/fxrate/fxrate_yesterday.json");
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }
                    else
                    {
                        rawjson_yesterday = await response.Content.ReadAsStringAsync();
                    }
                }
            }

            try
            {
                var crossrates = await ParseMarketData.ParseCrossRateJsonAsync(rawjson_today, rawjson_yesterday);
                StorageAPI.SaveFile(rawjson_today, CrossRatefile_today);
                StorageAPI.SaveFile(rawjson_yesterday, CrossRatefile_yesterday);
                return crossrates;
            }
            catch (JsonException)
            {
                return null;
            }

        }

        public static async Task<double> FetchPriceBTCBefore24hAsync(string instrumentId)
        {
            string rawjson_yesterday;

            if (!Reachability.IsHostReachable(coinbalance_url))
            {
                return 0;
            }
            else
            {
                using (var http = new HttpClient())
                {
                    rawjson_yesterday = await http.GetStringAsync(coinbalance_url + "/market/market_yesterday.json");
                }
                var jarray_yesterday = await Task.Run(() => JArray.Parse(rawjson_yesterday));
                return (double)jarray_yesterday.SelectToken("[?(@.id == '" + instrumentId + "')]")["price_btc"];
            }
        }

    }
}
