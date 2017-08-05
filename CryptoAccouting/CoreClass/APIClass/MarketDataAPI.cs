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
        //Obtain market data from coinmarketcap API

        public static async Task<EnuAppStatus> FetchCoinMarketDataAsync(List<Instrument> coins)
        {
            const string BaseUrl = "http://api.cryptocoincharts.info/";
            string rawjson;

            var pairs = "btc_usd";

            if (!Reachability.IsHostReachable(BaseUrl))
            {
                return EnuAppStatus.FailureNetwork;
            }
            else
            {
                if (coins.Any(x => x.Symbol == "BTC"))
                {
                    var bitcoin = coins.Where(x => x.Symbol == "BTC").First();
                    coins.Remove(bitcoin);
                    coins.Insert(0, bitcoin);
                }
                else{
                    coins.Insert(0, ApplicationCore.GetInstrument("BTC"));
                }

                foreach (var paircoin in coins.Where(x=>x.Symbol!="BTC"))
				{
                    pairs = pairs + "," + paircoin.Symbol.ToLower() + "_btc";
				}

				var parameters = new Dictionary<string, string>();
				parameters.Add("pairs", pairs);

                using (var http = new HttpClient())
                {
                    http.BaseAddress = new Uri(BaseUrl);
                    Uri path = new Uri("tradingPairs", UriKind.Relative);

                    var content = new FormUrlEncodedContent(parameters);

                    HttpResponseMessage res = await http.PostAsync(path, content);
                    rawjson = await res.Content.ReadAsStringAsync();

                    if (!res.IsSuccessStatusCode)
                        return EnuAppStatus.FailureNetwork;
                }

                int i = 0;
                foreach (var coin in coins)
                {

                    if (coin.MarketPrice == null)
                    {
                        var p = new Price(coin);
                        coin.MarketPrice = p;
                    }

                    var jarray = await Task.Run(() => JArray.Parse(rawjson));

                    coin.MarketPrice.SourceCurrency = coin.Symbol == "BTC" ? EnuCCY.USD : EnuCCY.BTC;
                    coin.MarketPrice.LatestPriceBTC = coin.Symbol == "BTC" ? 1 : (double)jarray[i]["price"];
                    coin.MarketPrice.PriceBTCBefore24h = coin.Symbol == "BTC" ? 1 : (double)jarray[i]["price_before_24h"];
                    coin.MarketPrice.LatestPrice = coin.Symbol == "BTC" ? (double)jarray[i]["price"] : (double)jarray[i]["price"] * (double)jarray[0]["price"];
                    coin.MarketPrice.PriceBefore24h = coin.Symbol == "BTC" ? (double)jarray[i]["price_before_24h"] : (double)jarray[i]["price_before_24h"] * (double)jarray[0]["price_before_24h"];
                    coin.MarketPrice.DayVolume = (double)jarray[i]["volume_btc"];
                    coin.MarketPrice.PriceDate = DateTime.Now;

                    i++;
                }
            }

            return EnuAppStatus.Success;

        }

        public static async Task<EnuAppStatus> FetchCoinMarketDataAsync(Instrument coin, Instrument bitcoin=null )
        {

            string rawjson;
            //const string CoinMarketUrl = "https://api.coinmarketcap.com/v1/ticker/";

   //         if (!Reachability.IsHostReachable(CoinMarketUrl))
   //         {
   //             return EnuAppStatus.FailureNetwork;
   //         }
   //         else
   //         {

   //             //Parse Market Data 
   //             if (coin.MarketPrice == null)
   //             {
   //                 var p = new Price(coin);
   //                 coin.MarketPrice = p;
   //             }

   //             using (var http = new HttpClient())
   //             {
   //                 //http.MaxResponseContentBufferSize = 256000;
   //                 rawjson = await http.GetStringAsync(CoinMarketUrl + coin.Id);
   //             }

   //             var jarray = await Task.Run(() => JArray.Parse(rawjson));

   //             coin.MarketPrice.LatestPriceBTC = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["price_btc"];
   //             coin.MarketPrice.LatestPrice = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["price_usd"];
   //             coin.MarketPrice.SourceCurrency = coin.Symbol == "BTC" ? EnuCCY.USD : EnuCCY.BTC;
   //             coin.MarketPrice.DayVolume = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["24h_volume_usd"] / coin.MarketPrice.LatestPriceBTC;
   //             coin.MarketPrice.MarketCap = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["market_cap_usd"];
   //             coin.MarketPrice.PriceDate = ApplicationCore.FromEpochSeconds((long)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["last_updated"]).Date;
   //             coin.MarketPrice.SourceRet1h = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["percent_change_1h"];
   //             coin.MarketPrice.SourceRet1d = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["percent_change_24h"];
   //             coin.MarketPrice.SourceRet7d = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["percent_change_7d"];
   //             coin.MarketPrice.PriceBefore24h = coin.MarketPrice.LatestPrice / (coin.MarketPrice.SourceRet1d / 100 + 1);

			//}

			string CoinChartsUrl = "http://api.cryptocoincharts.info/tradingPair/";
            JObject json;

            if (coin.Symbol != "BTC" && bitcoin is null)
            {
                bitcoin = new Instrument("Bitcoin", "BTC", "BitCoin");
                await FetchCoinMarketDataAsync(bitcoin);
            }

			if (!Reachability.IsHostReachable(CoinChartsUrl))
			{
			    return EnuAppStatus.FailureNetwork;
			}
			else
			{
				if (coin.MarketPrice == null)
				{
					var p = new Price(coin);
					coin.MarketPrice = p;
				}

			    CoinChartsUrl = (coin.Symbol == "BTC") ? CoinChartsUrl + "btc_usd" : CoinChartsUrl + coin.Symbol.ToLower() + "_btc";

			    using (var http = new HttpClient())
			    {
			        rawjson = await http.GetStringAsync(CoinChartsUrl);
			    }

			    json = await Task.Run(() => JObject.Parse(rawjson));

                coin.MarketPrice.SourceCurrency = coin.Symbol == "BTC" ? EnuCCY.USD : EnuCCY.BTC;
				coin.MarketPrice.LatestPriceBTC = coin.Symbol == "BTC" ? 1 : (double)json["price"];
				coin.MarketPrice.PriceBTCBefore24h = coin.Symbol == "BTC" ? 1 : (double)json["price_before_24h"];
                coin.MarketPrice.LatestPrice = coin.Symbol == "BTC" ? (double)json["price"] : (double)json["price"] * bitcoin.MarketPrice.LatestPrice;
                coin.MarketPrice.PriceBefore24h = coin.Symbol == "BTC" ? (double)json["price_before_24h"] : (double)json["price_before_24h"] * bitcoin.MarketPrice.PriceBefore24h;
				coin.MarketPrice.DayVolume = (double)json["volume_btc"];
				coin.MarketPrice.PriceDate = DateTime.Now;

			}

			return EnuAppStatus.Success;
        }

        public static EnuAppStatus FetchAllCoinData(List<Instrument> instruments)
		{
            
			const string BaseUrl = "https://api.coinmarketcap.com/v1/ticker/?limit=100";
			string rawjson;

            if (instruments is null) instruments = new List<Instrument>();

            if (!Reachability.IsHostReachable(BaseUrl))
            {
                return EnuAppStatus.FailureNetwork;
            }
            else
            {
                using (var http = new HttpClient())
                {
                    //http.MaxResponseContentBufferSize = 256000;
                    rawjson = http.GetStringAsync(BaseUrl).Result;
                }
                var jarray = JArray.Parse(rawjson);

                //Parse Market Data 
                foreach (var elem in jarray)
                {
                    var coin = new Instrument((string)elem["id"], (string)elem["symbol"], (string)elem["name"]);
                    coin.rank = int.Parse((string)elem["rank"]);
                    //coin.LogoFileName = ((string)elem["id"] + ".png");
                    var p = new Price(coin);
                    p.SourceCurrency = ApplicationCore.BaseCurrency;
                    coin.MarketPrice = p;
                    instruments.Add(coin);
                    FetchCoinLogo(coin.Id, false);
                }

                return EnuAppStatus.Success;
            }
		}

        private static void FetchCoinLogo(string InstrumentID, bool ForceRefresh)
        {
            var filename = InstrumentID + ".png";
            string TargetUri = "https://files.coinmarketcap.com/static/img/coins/32x32/" + filename;

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Images");

            if (!File.Exists(path))
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
        }
    }
}
