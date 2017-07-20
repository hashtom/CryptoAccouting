﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CryptoAccouting.UIClass;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class MarketDataAPI
    {

        //Obtain market data from coinmarketcap API
        public static async Task FetchCoinMarketDataAsync(Instrument coin)
        {

            const string BaseUrl = "https://api.coinmarketcap.com/v1/ticker/";
            string rawjson;

            //Parse Market Data 
            if (coin.MarketPrice == null)
            {
                var p = new Price(coin);
                coin.MarketPrice = p;
            }

            using (var http = new HttpClient())
            {
                //http.MaxResponseContentBufferSize = 256000;
                rawjson = await http.GetStringAsync(BaseUrl + coin.Id);
            }

            var jarray = await Task.Run(() => JArray.Parse(rawjson));

            coin.MarketPrice.LatestPriceBTC = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["price_btc"];
            coin.MarketPrice.LatestPrice = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["price_usd"];
            coin.MarketPrice.BaseCurrency = EnuCCY.USD; //hardcoded
                                                     //c.Name = (string)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["name"];
            coin.MarketPrice.DayVolume = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["24h_volume_usd"] / coin.MarketPrice.LatestPriceBTC;
            coin.MarketPrice.PriceDate = ApplicationCore.FromEpochSeconds((long)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["last_updated"]).Date;
            coin.MarketPrice.Pct1h = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["percent_change_1h"];
            //ins.MarketPrice.Pct1d = (double)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["percent_change_24h"];
            coin.MarketPrice.Pct7d = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["percent_change_7d"];
            //ins.MarketPrice.Pct1h = pct1h.GetType() == typeof(double) ? 0 : pct1h;
            //ins.MarketPrice.Pct1d = pct1d.GetType() == typeof(double) ? 0 : (double)pct1d;
            //ins.MarketPrice.Pct7d = pct7d.GetType() == typeof(double) ? 0 : pct7d;

        }
    

        public static async Task FetchMarketDataFromBalance(Balance mybal)
		{
            foreach (var pos in mybal)
            {
                await FetchCoinMarketDataAsync(pos.Coin);
            }
		}

		public static void FetchAllCoinData(List<Instrument> instruments)
		{

			const string BaseUrl = "https://api.coinmarketcap.com/v1/ticker/";
			string rawjson;

			using (var http = new HttpClient())
			{
				//http.MaxResponseContentBufferSize = 256000;
                rawjson = http.GetStringAsync(BaseUrl).Result;
			}
            var jarray = JArray.Parse(rawjson);

            //Parse Market Data 
            foreach (var elem in jarray){
                var coin = new Instrument((string)elem["id"], (string)elem["symbol"], (string)elem["name"]);
				var p = new Price(coin);
                p.BaseCurrency = AppSetting.BaseCurrency;
				coin.MarketPrice = p;
                instruments.Add(coin);
            }

		}
    }
}
