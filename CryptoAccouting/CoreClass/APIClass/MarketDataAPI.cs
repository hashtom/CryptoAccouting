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
        public static async Task<EnuAPIStatus> FetchCoinPricesAsync(ExchangeList exchanges, InstrumentList coins, List<CrossRate> crossrates)
		{

            if (!Reachability.IsHostReachable("coinbalance.jpn.org"))
            {
                return EnuAPIStatus.FailureNetwork;
            }
            else
            {
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

            const string CoinMarketUrl = "https://api.coinmarketcap.com/v1/ticker/";
            const string CoinMarketUrl_yesterday = "http://coinbalance.jpn.org/market/market_yesterday.json";

            EnuAPIStatus status = EnuAPIStatus.Success;

            if (!Reachability.IsHostReachable(CoinMarketUrl))
            {
                return EnuAPIStatus.FailureNetwork;
            }
            else
            {
                try
                {
                    using (var http = new HttpClient())
                    {
                        //http.MaxResponseContentBufferSize = 256000;
                        rawjson = await http.GetStringAsync(CoinMarketUrl);
                    }

                    using (var http = new HttpClient())
                    {
                        rawjson_yesterday = await http.GetStringAsync(CoinMarketUrl_yesterday);
                    }
                }
                catch (HttpRequestException)
                {
                    return EnuAPIStatus.FailureNetwork;
                }

                foreach (var coin in instrumentlist.Where(x => x.PriceSourceCode == "coinmarketcap"))
                {
                    //Parse Market Data 
                    if (coin.MarketPrice == null)
                    {
                        var p = new Price(coin);
                        coin.MarketPrice = p;
                    }

                    var jarray = await Task.Run(() => JArray.Parse(rawjson));
                    var jarray_yesterday = await Task.Run(() => JArray.Parse(rawjson_yesterday));

                    coin.MarketPrice.LatestPriceBTC = (double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["price_btc"];
                    coin.MarketPrice.LatestPriceUSD = (double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["price_usd"];
                    coin.MarketPrice.PriceSource = "coinmarketcap";
                    coin.MarketPrice.DayVolume = (double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["24h_volume_usd"] / coin.MarketPrice.LatestPriceBTC;
                    coin.MarketPrice.MarketCap = (double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["market_cap_usd"];
                    coin.MarketPrice.PriceDate = DateTime.Now;//ApplicationCore.FromEpochSeconds((long)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["last_updated"]);
                    coin.MarketPrice.PriceBTCBefore24h = (double)jarray_yesterday.SelectToken("[?(@.id == '" + coin.Id + "')]")["price_btc"];
                    coin.MarketPrice.PriceUSDBefore24h = (double)jarray_yesterday.SelectToken("[?(@.id == '" + coin.Id + "')]")["price_usd"];
                    coin.MarketPrice.USDCrossRate = crossrate;

                }
            }

            return status;

        }

        public static EnuAPIStatus FetchAllCoinData(InstrumentList instrumentlist, bool UseBundleFile)
		{
            const string BaseUrl = "http://coinbalance.jpn.org/InstrumentList.json";
                // "https://api.coinmarketcap.com/v1/ticker/?limit=150";

			string rawjson;

            if (UseBundleFile)
            {
                rawjson = StorageAPI.LoadBundleFile(ApplicationCore.InstrumentsBundleFile);
            }
            else
            {
                if (!Reachability.IsHostReachable(BaseUrl))
                {
                    return EnuAPIStatus.FailureNetwork;
                }
                else
                {
                    //instrumentlist.Clear();
                    using (var http = new HttpClient())
                    {
                        //http.MaxResponseContentBufferSize = 256000;
                        rawjson = http.GetStringAsync(BaseUrl).Result;
                    }
                }
            }

            var jarray = JArray.Parse(rawjson);

            //Parse Market Data 
            foreach (var elem in jarray)
            {
                if ((bool)elem["active"])
                {
                    Instrument coin;
                    if (instrumentlist.Any(x => x.Id == (string)elem["id"]))
                    {
                        coin = instrumentlist.First(x => x.Id == (string)elem["id"]);
                        coin.Symbol1 = (string)elem["symbol"];
                        coin.Name = (string)elem["name"];
                    }
                    else
                    {
                        coin = new Instrument((string)elem["id"])
                        {
                            Symbol1 = (string)elem["symbol"],
                            Name = (string)elem["name"]
                        };//IOTA symbol注意
                    }

                    if (elem["symbol2"] != null)
                    {
                        coin.Symbol2 = (string)elem["symbol2"];
                    }
                    coin.rank = int.Parse((string)elem["rank"]);
                    instrumentlist.Attach(coin);
                }
            }

            StorageAPI.SaveInstrumentXML(instrumentlist, ApplicationCore.InstrumentsFile);

			return EnuAPIStatus.Success;
		}

        public static async Task<EnuAPIStatus> FetchCoinLogoAsync(string InstrumentID, bool ForceRefresh)
        {
            var filename = InstrumentID + ".png";
            string TargetUri = "http://coinbalance.jpn.org/images/" + filename;

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
            List<CrossRate> crossrates = new List<CrossRate>();
            //CrossRate crossrate = null;
            string rawjson_today, rawjson_yesterday;
            const string jsonfilename_today = "crossrate_today.json";
            const string jsonfilename_yesterday = "crossrate_yesterday.json";
            const string BaseUri = "http://coinbalance.jpn.org/";

            if (!Reachability.IsHostReachable(BaseUri))
            {
                rawjson_today = StorageAPI.LoadFromFile(jsonfilename_today);
                rawjson_yesterday = StorageAPI.LoadFromFile(jsonfilename_yesterday);
                if (rawjson_today is null) return null;
            }
            else
            {
                using (var http = new HttpClient())
                {
                    HttpResponseMessage response = await http.GetAsync(BaseUri + "/fxrate/fxrate_latest.json");
                    if (!response.IsSuccessStatusCode)
                    {
                        rawjson_today = StorageAPI.LoadFromFile(jsonfilename_today);
                    }
                    else
                    {
                        rawjson_today = await response.Content.ReadAsStringAsync();
                    }
                }

                using (var http = new HttpClient())
                {
                    HttpResponseMessage response = await http.GetAsync(BaseUri + "/fxrate/fxrate_yesterday.json");
                    if (!response.IsSuccessStatusCode)
                    {
                        rawjson_yesterday = StorageAPI.LoadFromFile(jsonfilename_yesterday);
                    }
                    else
                    {
                        rawjson_yesterday = await response.Content.ReadAsStringAsync();
                    }
                }
            }

            try
            {
                crossrates = parseCrossRate(rawjson_today, rawjson_yesterday);
            }
            catch (JsonException)
            {
                rawjson_today = StorageAPI.LoadFromFile(jsonfilename_today);
                rawjson_yesterday = StorageAPI.LoadFromFile(jsonfilename_yesterday);
                if (rawjson_today is null) return null;
                crossrates = parseCrossRate(rawjson_today, rawjson_yesterday);
            }

            StorageAPI.SaveFile(rawjson_today, jsonfilename_today);
            StorageAPI.SaveFile(rawjson_yesterday, jsonfilename_yesterday);

            return crossrates;
        }

        public static async Task<double> FetchPriceBTCBefore24hAsync(string instrumentId)
        {
            string rawjson_yesterday;
            const string CoinMarketUrl_yesterday = "http://coinbalance.jpn.org/market/market_yesterday.json";

            if (!Reachability.IsHostReachable(CoinMarketUrl_yesterday))
            {
                return 0;
            }
            else
            {
                using (var http = new HttpClient())
                {
                    rawjson_yesterday = await http.GetStringAsync(CoinMarketUrl_yesterday);
                }
                var jarray_yesterday = await Task.Run(() => JArray.Parse(rawjson_yesterday));
                return (double)jarray_yesterday.SelectToken("[?(@.id == '" + instrumentId + "')]")["price_btc"];
            }
        }

        private static List<CrossRate> parseCrossRate(string rawjson_today, string rawjson_yesterday)
        {
            List<CrossRate> crossrates = new List<CrossRate>();
            JObject json;

            json = JObject.Parse(rawjson_today);
            foreach (var ccy in (JArray)json["list"]["resources"])
            {
                EnuBaseFiatCCY baseccy;
                var cursymbol = (string)ccy["resource"]["fields"]["symbol"];

                if (!Enum.TryParse(cursymbol.Replace("=X", ""), out baseccy))
                    continue;

                var crossrate = new CrossRate(baseccy, (double)ccy["resource"]["fields"]["price"], DateTime.Now.Date);
                crossrates.Add(crossrate);
            }

            json = JObject.Parse(rawjson_yesterday);
            foreach (var ccy in (JArray)json["list"]["resources"])
            {
                EnuBaseFiatCCY baseccy;
                var cursymbol = (string)ccy["resource"]["fields"]["symbol"];

                if (!Enum.TryParse(cursymbol.Replace("=X", ""), out baseccy))
                    continue;

                if (crossrates.Any(x => x.Currency == baseccy))
                    crossrates.First(x => x.Currency == baseccy).RateBefore24h = (double)ccy["resource"]["fields"]["price"];

            }

            return crossrates;
        }
    }
}
