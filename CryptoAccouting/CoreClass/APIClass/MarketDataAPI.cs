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
            var status = EnuAPIStatus.FailureParameter;

            foreach (var source in coins.Select(x => x.PriceSourceCode).Distinct())
            {
                switch (source)
                {
                    case "Bittrex":
                        var bittrex = exchanges.First(x => x.Code == "Bittrex");
                        status = await BittrexAPI.FetchPriceAsync(bittrex, coins, crossrate);
                        break;

                    case "Bitstamp":
                        var bitstamp = exchanges.First(x => x.Code == "Bitstamp");
                        status = await BItstampAPI.FetchPriceAsync(bitstamp, coins, crossrate);
                        break;

                    case "Zaif":
                        break;

                    case "coinmarketcap":
                        status = await FetchCoinMarketDataAsync(coins, crossrate);
                        break;

                    default:
                        break;
                }
            }

            return status;
		}

        public static async Task<EnuAPIStatus> FetchCoinMarketDataAsync(InstrumentList instrumentlist, CrossRate crossrate)
        {
            string rawjson;



            const string CoinMarketUrl = "https://api.coinmarketcap.com/v1/ticker/";
            if (!Reachability.IsHostReachable(CoinMarketUrl))
            {
                return EnuAPIStatus.FailureNetwork;
            }
            else
            {
				using (var http = new HttpClient())
				{
                    //http.MaxResponseContentBufferSize = 256000;
                    rawjson = await http.GetStringAsync(CoinMarketUrl);
				}

                foreach (var coin in instrumentlist.Where(x=>x.PriceSourceCode=="coinmarketcap"))
                {
                    //Parse Market Data 
                    if (coin.MarketPrice == null)
                    {
                        var p = new Price(coin);
                        coin.MarketPrice = p;
                    }

                    var jarray = await Task.Run(() => JArray.Parse(rawjson));

                    coin.MarketPrice.LatestPriceBTC = (double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["price_btc"];
                    coin.MarketPrice.LatestPriceUSD = (double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["price_usd"];
                    coin.MarketPrice.PriceSource = "coinmarketcap";
                    coin.MarketPrice.DayVolume = (double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["24h_volume_usd"] / coin.MarketPrice.LatestPriceBTC;
                    coin.MarketPrice.MarketCap = (double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["market_cap_usd"];
                    coin.MarketPrice.PriceDate = ApplicationCore.FromEpochSeconds((long)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["last_updated"]).Date;
                    coin.MarketPrice.PriceBTCBefore24h = coin.MarketPrice.LatestPriceBTC / ((double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["percent_change_24h"] / 100 + 1);
                    coin.MarketPrice.PriceUSDBefore24h = coin.MarketPrice.LatestPriceUSD / ((double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["percent_change_24h"] / 100 + 1);
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

            return EnuAPIStatus.Success;

        }

   //     public static async Task<EnuAPIStatus> FetchCoinMarketDataAsync(Instrument coin, Instrument bitcoin=null )
   //     {

   //         string rawjson;
			//string CoinChartsUrl = "http://api.cryptocoincharts.info/tradingPair/";
   //         JObject json;

   //         if (coin.Symbol1 != "BTC" && bitcoin is null)
   //         {
   //             bitcoin = new Instrument("Bitcoin"){Symbol1="BTC"};
   //             await FetchCoinMarketDataAsync(bitcoin);
   //         }

   //         if (!Reachability.IsHostReachable(CoinChartsUrl))
   //         {
   //             return EnuAPIStatus.FailureNetwork;
   //         }
   //         else
   //         {
   //             if (coin.MarketPrice == null)
   //             {
   //                 var p = new Price(coin);
   //                 coin.MarketPrice = p;
   //             }

   //             CoinChartsUrl = (coin.Symbol1 == "BTC") ? CoinChartsUrl + "btc_usd" : CoinChartsUrl + coin.Symbol1.ToLower() + "_btc";

   //             using (var http = new HttpClient())
   //             {
   //                 rawjson = await http.GetStringAsync(CoinChartsUrl);
   //             }

   //             json = await Task.Run(() => JObject.Parse(rawjson));

   //             if (json["id"] != null)
   //             {
   //                 coin.MarketPrice.LatestPriceBTC = coin.Symbol1 == "BTC" ? 1 : (double)json["price"];
   //                 coin.MarketPrice.PriceBTCBefore24h = coin.Symbol1 == "BTC" ? 1 : (double)json["price_before_24h"];
   //                 coin.MarketPrice.LatestPriceUSD = coin.Symbol1 == "BTC" ? (double)json["price"] : (double)json["price"] * bitcoin.MarketPrice.LatestPriceUSD;
   //                 coin.MarketPrice.PriceUSDBefore24h = coin.Symbol1 == "BTC" ? (double)json["price_before_24h"] : (double)json["price_before_24h"] * bitcoin.MarketPrice.PriceUSDBefore24h;
   //                 coin.MarketPrice.DayVolume = (double)json["volume_btc"];
   //                 coin.MarketPrice.PriceDate = DateTime.Now;
   //             }

			//}

			//return EnuAPIStatus.Success;
        //}

        public static EnuAPIStatus FetchAllCoinData(InstrumentList instrumentlist, bool OnlineUpdate)
		{
            const string BaseUrl = "http://bridgeplace.sakura.ne.jp/cryptoticker/InstrumentList.json";
                // "https://api.coinmarketcap.com/v1/ticker/?limit=150";

			string rawjson;

            if (!OnlineUpdate)
            {
                rawjson = StorageAPI.LoadBundleJsonFile(ApplicationCore.InstrumentsBundleFile);
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

                    var p = new Price(coin);
                    coin.MarketPrice = p;
                    instrumentlist.Attach(coin);
                    FetchCoinLogo(coin.Id, false);
                }
            }

            if (OnlineUpdate) StorageAPI.SaveInstrumentXML(instrumentlist, ApplicationCore.InstrumentsFile);

			return EnuAPIStatus.Success;
		}

        private static EnuAPIStatus FetchCoinLogo(string InstrumentID, bool ForceRefresh)
        {
            var filename = InstrumentID + ".png";
            string TargetUri = "http://bridgeplace.sakura.ne.jp/cryptoticker/images/" + filename;

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
                    HttpResponseMessage res = client.GetAsync(TargetUri, HttpCompletionOption.ResponseContentRead).Result;

                    using (var fileStream = File.Create(path))
                    using (var httpStream = res.Content.ReadAsStreamAsync().Result)
                        httpStream.CopyTo(fileStream);
                }
                return EnuAPIStatus.Success;
            }
        }

        public static EnuAPIStatus FetchExchangeList(ExchangeList exlist)
        {
            const string jsonfilename = "ExchangeList.json";
            string rawjson;
            string BaseUri = "http://bridgeplace.sakura.ne.jp/cryptoticker/ExchangeList.json";

            if (!Reachability.IsHostReachable(BaseUri))
            {
                rawjson = StorageAPI.LoadFromFile(jsonfilename);
                if (rawjson == null) rawjson = StorageAPI.LoadBundleJsonFile(jsonfilename);
                //rawjson = File.ReadAllText("Json/ExchangeList.json"); //Bundle file
                //return EnuAPIStatus.FailureNetwork;
            }
            else
            {
                using (var http = new HttpClient())
                {
                    HttpResponseMessage response = http.GetAsync(BaseUri).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        return EnuAPIStatus.FailureNetwork;
                    }
                    rawjson = response.Content.ReadAsStringAsync().Result;
                }
                StorageAPI.SaveFile(rawjson, jsonfilename);
            }

            var json = JObject.Parse(rawjson);

            foreach (var market in (JArray)json["exchanges"])
            {

                //EnuExchangeType code;
                //if (!Enum.TryParse((string)market["code"], out code))
                //	code = EnuExchangeType.NotSelected;

                //if (code != EnuExchangeType.NotSelected)
                //{
                var exchange = exlist.GetExchange((string)market["code"]);
                exchange.Name = (string)market["name"];

                var listing = (JArray)market["listing"];

                if (listing.ToList().Count() == 0)
                {
                    ApplicationCore.InstrumentList.ToList().ForEach(x => exchange.AttachListedCoin(x));
                }
                else
                {
                    foreach (var symbol in listing)
                    {
                        Instrument coin = null;
                        if (symbol["symbol"] != null)
                        {
                            coin = ApplicationCore.InstrumentList.GetBySymbol1((string)symbol["symbol"]);
                        }
                        else if (symbol["symbol2"] != null)
                        {
                            coin = ApplicationCore.InstrumentList.GetBySymbol2((string)symbol["symbol2"]);
                            if (coin != null) exchange.AttachSymbolMap(coin.Id, EnuSymbolMapType.Symbol2);
                        }

                        if (coin != null) exchange.AttachListedCoin(coin);
                    }
                }
                exchange.APIReady = (bool)market["api"];
                //}
            }

            return EnuAPIStatus.Success;
        }

        public static async Task<CrossRate> FetchUSDCrossRateAsync(EnuCCY BaseCurrency)
		{
            CrossRate crossrate = null;
			string rawjson_today, rawjson_yesterday;
            const string jsonfilename_today = "crossrate_today.json";
            const string jsonfilename_yesterday = "crossrate_yesterday.json";
            const string BaseUri = "http://bridgeplace.sakura.ne.jp/cryptoticker";

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
                        return null;
                    }
                    rawjson_today = await response.Content.ReadAsStringAsync();
                }

				using (var http = new HttpClient())
				{
					HttpResponseMessage response = await http.GetAsync(BaseUri + "/fxrate/fxrate_yesterday.json");
					if (!response.IsSuccessStatusCode)
					{
						return null;
					}
                    rawjson_yesterday = await response.Content.ReadAsStringAsync();
				}
            }

            var json = JObject.Parse(rawjson_today);

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

    }
}
