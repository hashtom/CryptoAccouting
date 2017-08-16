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

                var jarray = await Task.Run(() => JArray.Parse(rawjson));

                foreach (var jrow in jarray)
                {
                    var symbol = (string)jrow["id"];
                    if (symbol.Substring(0, 3) == "btc")
                    {
                        symbol = "BTC";
                    }
                    else
                    {
                        symbol = symbol.Replace("btc", "").Replace("/", "").Replace("¥", "").ToUpper();
                    }

                    var coin = coins.Where(x => x.Symbol == symbol).First();

                    if (coin.MarketPrice == null)
                    {
                        var p = new Price(coin);
                        coin.MarketPrice = p;
                    }

                    //coin.MarketPrice.SourceCurrency = coin.Symbol == "BTC" ? EnuCCY.USD : EnuCCY.BTC;
                    coin.MarketPrice.LatestPriceBTC = coin.Symbol == "BTC" ? 1 : (double)jrow["price"];
                    coin.MarketPrice.PriceBTCBefore24h = coin.Symbol == "BTC" ? 1 : (double)jrow["price_before_24h"];
                    coin.MarketPrice.LatestPriceUSD = coin.Symbol == "BTC" ? (double)jrow["price"] : (double)jrow["price"] * (double)jarray[0]["price"];
                    coin.MarketPrice.PriceUSDBefore24h = coin.Symbol == "BTC" ? (double)jrow["price_before_24h"] : (double)jrow["price_before_24h"] * (double)jarray[0]["price_before_24h"];
                    coin.MarketPrice.DayVolume = (double)jrow["volume_btc"];
                    coin.MarketPrice.PriceDate = DateTime.Now;

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

                //coin.MarketPrice.SourceCurrency = coin.Symbol == "BTC" ? EnuCCY.USD : EnuCCY.BTC;
				coin.MarketPrice.LatestPriceBTC = coin.Symbol == "BTC" ? 1 : (double)json["price"];
				coin.MarketPrice.PriceBTCBefore24h = coin.Symbol == "BTC" ? 1 : (double)json["price_before_24h"];
                coin.MarketPrice.LatestPriceUSD = coin.Symbol == "BTC" ? (double)json["price"] : (double)json["price"] * bitcoin.MarketPrice.LatestPriceUSD;
                coin.MarketPrice.PriceUSDBefore24h = coin.Symbol == "BTC" ? (double)json["price_before_24h"] : (double)json["price_before_24h"] * bitcoin.MarketPrice.PriceUSDBefore24h;
				coin.MarketPrice.DayVolume = (double)json["volume_btc"];
				coin.MarketPrice.PriceDate = DateTime.Now;

			}

			return EnuAppStatus.Success;
        }

        public static EnuAppStatus FetchAllCoinData(List<Instrument> instruments)
		{
            
			const string BaseUrl = "https://api.coinmarketcap.com/v1/ticker/?limit=150";
			string rawjson;

            //if (instruments is null) instruments = new List<Instrument>();

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

		public static EnuAppStatus FetchExchangeList(ExchangeList exlist)
		{

			string rawjson;
			string BaseUri = "http://bridgeplace.sakura.ne.jp/cryptoticker/ExchangeList.json";

			if (!Reachability.IsHostReachable(BaseUri))
			{
                rawjson = File.ReadAllText("Json/ExchangeList.json"); //Bundle file
                return EnuAppStatus.FailureNetwork;
			}
			else
			{
				using (var http = new HttpClient())
				{
					HttpResponseMessage response = http.GetAsync(BaseUri).Result;
					if (!response.IsSuccessStatusCode)
					{
						return EnuAppStatus.FailureNetwork;
					}
					rawjson = response.Content.ReadAsStringAsync().Result;
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

                    foreach (var symbol in (JArray)market["listing"])
                    {
                        var coin = ApplicationCore.GetInstrument((string)symbol["symbol"]);
                        if (coin != null) exchange.AttachListedCoin(coin);
                    }

                    exchange.APIReady = (bool)market["api"];
                    //}
                }

			}

			return EnuAppStatus.Success;
		}

        public static async Task<CrossRate> FetchUSDCrossRateAsync(EnuCCY BaseCurrency)
		{
            CrossRate crossrate = null;
			string rawjson;
            const string BaseUri = "https://finance.yahoo.com/webservice/v1/symbols/allcurrencies/quote?format=json";

			if (!Reachability.IsHostReachable(BaseUri))
			{
                return null;
                //return EnuAppStatus.FailureNetwork;
			}
			else
			{
				using (var http = new HttpClient())
				{
                    HttpResponseMessage response = await http.GetAsync(BaseUri);
					if (!response.IsSuccessStatusCode)
					{
                        return null;
                        //return EnuAppStatus.FailureNetwork;
					}
                    rawjson = await response.Content.ReadAsStringAsync();
				}

				var json = JObject.Parse(rawjson);

                foreach (var ccy in (JArray)json["list"]["resources"])
				{
                    EnuCCY baseccy;
                    var cursymbol = (string)ccy["resource"]["fields"]["symbol"];

					if (!Enum.TryParse(cursymbol.Replace("=X", ""), out baseccy))
                        continue;

                    if (baseccy == ApplicationCore.BaseCurrency)
                    {
                        crossrate = new CrossRate(baseccy, (double)ccy["resource"]["fields"]["price"], DateTime.Now.Date);
                        break;
                    }
				}

			}

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
