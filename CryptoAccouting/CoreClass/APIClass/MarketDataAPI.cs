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

        public static async Task FetchMarketDataFromBalanceAsync(Balance mybal)
		{
            Instrument bitcoin;

            if (mybal.positions.Any(x=>x.Coin.Symbol == "BTC")){
                bitcoin = mybal.positions.Where(x => x.Coin.Symbol == "BTC").Select(x => x.Coin).First();
            }else{
                bitcoin = new Instrument("Bitcoin", "BTC", "Bitcoin");
            }
                
            await FetchCoinMarketDataAsync(bitcoin);

            foreach (var pos in mybal.positions.Where(x => x.Coin.Symbol != "BTC"))
            {
                if (pos.Coin != null) await FetchCoinMarketDataAsync(pos.Coin, bitcoin);
            }
		}

        public static EnuAppStatus FetchAllCoinData(List<Instrument> instruments)
		{

			const string BaseUrl = "https://api.coinmarketcap.com/v1/ticker/";
			string rawjson;

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
                    coin.LogoFileName = "Images/" + (string)elem["name"] + ".png";
                    var p = new Price(coin);
                    p.SourceCurrency = ApplicationCore.BaseCurrency;
                    coin.MarketPrice = p;
                    instruments.Add(coin);
                }

                return EnuAppStatus.Success;
            }
		}

        public static async Task FetchCoinLogoAsync(string InstrumentName)
        {
            var fileName = InstrumentName + ".png";
            string TargetUri = "https://files.coinmarketcap.com/static/img/coins/64x64/" + fileName;

            var client = new HttpClient();
            HttpResponseMessage res = await client.GetAsync(TargetUri, HttpCompletionOption.ResponseContentRead);

            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = Path.Combine(path, "Images");

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, fileName);

            using (var fileStream = File.Create(path))
            using (var httpStream = await res.Content.ReadAsStreamAsync())
                httpStream.CopyTo(fileStream);


        }
    }
}
