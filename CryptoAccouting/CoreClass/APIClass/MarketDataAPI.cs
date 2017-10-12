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
        public static async Task<EnuAPIStatus> FetchCoinPricesAsync(ExchangeList exchanges, InstrumentList coins, CrossRate crossrate)
		{
            //var status = EnuAPIStatus.Success;

            foreach (var source in coins.Select(x => x.PriceSourceCode).Distinct())
            {
                Exchange exchange;

                switch (source)
                {
                    //Bitcoin must go first
                    case "Bittrex":
                        exchange = exchanges.First(x => x.Code == "Bittrex");
                        await BittrexAPI.FetchPriceAsync(exchange, coins, crossrate);
                        break;

                    case "Bitstamp":
                        exchange = exchanges.First(x => x.Code == "Bitstamp");
                        await BItstampAPI.FetchPriceAsync(exchange, coins, crossrate);
                        break;

                    case "Zaif":
                        exchange = exchanges.First(x => x.Code == "Zaif");
                        await ZaifAPI.FetchPriceAsync(exchange, coins, crossrate);
                        break;

                    case "CoinCheck":
                        exchange = exchanges.First(x => x.Code == "CoinCheck");
                        await CoinCheckAPI.FetchPriceAsync(exchange, coins, crossrate);
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

            //         const string BaseUrl = "http://api.cryptocoincharts.info/";
            //var pairs = "btc_usd";

            //        if (!Reachability.IsHostReachable(BaseUrl))
            //        {
            //            return EnuAPIStatus.FailureNetwork;
            //        }
            //        else
            //        {
            //            if (instrumentlist.Any(x => x.Symbol1 == "BTC"))
            //            {
            //                var bitcoin = instrumentlist.Where(x => x.Symbol1 == "BTC").First();
            //                instrumentlist.Detach(bitcoin);
            //                instrumentlist.Insert(0, bitcoin);
            //            }
            //            else{
            //                instrumentlist.Insert(0, ApplicationCore.InstrumentList.GetByInstrumentId("bitcoin"));
            //            }

            //            foreach (var paircoin in instrumentlist.Where(x=>x.Symbol1!="BTC").Where(xx=>xx.PriceSourceCode == "cryptocoincharts"))
            //{
            //                pairs = pairs + "," + paircoin.Symbol1.ToLower() + "_btc";
            //}

            //var parameters = new Dictionary<string, string>();
            //parameters.Add("pairs", pairs);

            //    using (var http = new HttpClient())
            //    {
            //        http.BaseAddress = new Uri(BaseUrl);
            //        Uri path = new Uri("tradingPairs", UriKind.Relative);

            //        var content = new FormUrlEncodedContent(parameters);

            //        HttpResponseMessage res = await http.PostAsync(path, content);
            //        rawjson = await res.Content.ReadAsStringAsync();

            //        if (!res.IsSuccessStatusCode)
            //            return EnuAPIStatus.FailureNetwork;
            //    }

            //    var jarray = await Task.Run(() => JArray.Parse(rawjson));

            //    foreach (var jrow in jarray)
            //    {
            //        var idstring = (string)jrow["id"];
            //        string symobl;

            //        if (idstring.Substring(0, 3) == "btc")
            //        {
            //            symobl = "BTC";
            //        }
            //        else
            //        {
            //            symobl = idstring.Replace("btc", "").Replace("/", "").Replace("¥", "").ToUpper();
            //        }

            //        if (instrumentlist.Any(x => x.Symbol1 == symobl))
            //        {
            //            var coin = instrumentlist.Where(x => x.Symbol1 == symobl).First();

            //            if (coin.MarketPrice == null)
            //            {
            //                var p = new Price(coin);
            //                coin.MarketPrice = p;
            //            }

            //            //coin.MarketPrice.SourceCurrency = coin.Symbol == "BTC" ? EnuCCY.USD : EnuCCY.BTC;
            //            coin.MarketPrice.LatestPriceBTC = coin.Symbol1 == "BTC" ? 1 : (double)jrow["price"];
            //            coin.MarketPrice.PriceBTCBefore24h = coin.Symbol1 == "BTC" ? 1 : (double)jrow["price_before_24h"];
            //            coin.MarketPrice.LatestPriceUSD = coin.Symbol1 == "BTC" ? (double)jrow["price"] : (double)jrow["price"] * (double)jarray[0]["price"];
            //            coin.MarketPrice.PriceUSDBefore24h = coin.Symbol1 == "BTC" ? (double)jrow["price_before_24h"] : (double)jrow["price_before_24h"] * (double)jarray[0]["price_before_24h"];
            //            coin.MarketPrice.DayVolume = (double)jrow["volume_btc"];
            //            coin.MarketPrice.PriceDate = DateTime.Now;
            //            coin.MarketPrice.USDCrossRate = crossrate;
            //        }

            //    }
            //}

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

                    //var p = new Price(coin);
                    //coin.MarketPrice = p;
                    instrumentlist.Attach(coin);
                    //FetchCoinLogo(coin.Id, false);
                }
            }

            if (!UseBundleFile) StorageAPI.SaveInstrumentXML(instrumentlist, ApplicationCore.InstrumentsFile);

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

        public static async Task<CrossRate> FetchUSDCrossRateAsync(EnuCCY BaseCurrency)
        {
            CrossRate crossrate = null;
            string rawjson_today, rawjson_yesterday;
            JObject json;
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
                json = JObject.Parse(rawjson_today);
                foreach (var ccy in (JArray)json["list"]["resources"])
                {
                    EnuCCY baseccy;
                    var cursymbol = (string)ccy["resource"]["fields"]["symbol"];

                    if (!Enum.TryParse(cursymbol.Replace("=X", ""), out baseccy))
                        continue;

                    if (baseccy == BaseCurrency)
                    {
                        crossrate = new CrossRate(baseccy, (double)ccy["resource"]["fields"]["price"], DateTime.Now.Date);
                        break;
                    }
                }

                json = JObject.Parse(rawjson_yesterday);
                foreach (var ccy in (JArray)json["list"]["resources"])
                {
                    EnuCCY baseccy;
                    var cursymbol = (string)ccy["resource"]["fields"]["symbol"];

                    if (!Enum.TryParse(cursymbol.Replace("=X", ""), out baseccy))
                        continue;

                    if (baseccy == BaseCurrency)
                    {
                        crossrate.RateBefore24h = (double)ccy["resource"]["fields"]["price"];
                        break;
                    }
                }
            }
            catch (JsonException)
            {
                return null;
            }

            StorageAPI.SaveFile(rawjson_today, jsonfilename_today);
            StorageAPI.SaveFile(rawjson_yesterday, jsonfilename_yesterday);

            return crossrate;
        }

        //      public static EnuAppStatus FetchExchangeListTemp(Instrument coin, ExchangeList exlist)
        //{

        //	string rawjson;

        //	string BaseUri = "https://api.cryptonator.com/api/full/";

        //	if (!Reachability.IsHostReachable(BaseUri))
        //	{
        //		return EnuAppStatus.FailureNetwork;
        //	}
        //	else
        //	{

        //		BaseUri = (coin.Symbol == "BTC") ? BaseUri + "btc-usd" : BaseUri + coin.Symbol.ToLower() + "-btc";

        //              using (var http = new HttpClient())
        //              {
        //                  HttpResponseMessage response = http.GetAsync(BaseUri).Result;
        //                  if (!response.IsSuccessStatusCode)
        //                  {
        //                      return EnuAppStatus.FailureNetwork;
        //                  }
        //                  rawjson = response.Content.ReadAsStringAsync().Result;
        //              }

        //		var json = JObject.Parse(rawjson);

        //              if (!(bool)json["success"]) return EnuAppStatus.FailureParameter;

        //              foreach (var market in (JArray)json["ticker"]["markets"])
        //              {
        //                  var name = (string)market["market"];

        //                  EnuExchangeType tradedexchange;
        //                  if (!Enum.TryParse(name.Replace("-","").Replace(".","").Replace(" ",""), out tradedexchange))
        //                      tradedexchange = EnuExchangeType.NotSelected;

        //                  if (tradedexchange != EnuExchangeType.NotSelected)
        //                  {
        //                      var exchange = exlist.GetExchange(tradedexchange);
        //                      exchange.AttachListedCoin(coin);
        //                  }
        //              }

        //	}

        //	return EnuAppStatus.Success;
        //}


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
    }
}
