﻿﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class MarketDataAPI
    {
        //Obtain market data from coinmarketcap API
        public static async Task<EnuAppStatus> FetchCoinMarketDataAsync(Instrument coin)
        {

            const string CoinMarketUrl = "https://api.coinmarketcap.com/v1/ticker/";
            string rawjson;

            if (!Reachability.IsHostReachable(CoinMarketUrl))
            {
                return EnuAppStatus.FailureNetwork;
            }
            else
            {

                //Parse Market Data 
                if (coin.MarketPrice == null)
                {
                    var p = new Price(coin);
                    coin.MarketPrice = p;
                }

                using (var http = new HttpClient())
                {
                    //http.MaxResponseContentBufferSize = 256000;
                    rawjson = await http.GetStringAsync(CoinMarketUrl + coin.Id);
                }

                var jarray = await Task.Run(() => JArray.Parse(rawjson));

                coin.MarketPrice.LatestPriceBTC = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["price_btc"];
                coin.MarketPrice.LatestPrice = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["price_usd"];
                coin.MarketPrice.SourceCurrency = coin.Symbol == "BTC" ? EnuCCY.USD : EnuCCY.BTC;
                coin.MarketPrice.DayVolume = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["24h_volume_usd"] / coin.MarketPrice.LatestPriceBTC;
                coin.MarketPrice.MarketCap = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["market_cap_usd"];
                coin.MarketPrice.PriceDate = ApplicationCore.FromEpochSeconds((long)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["last_updated"]).Date;
                coin.MarketPrice.SourceRet1h = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["percent_change_1h"];
                coin.MarketPrice.SourceRet1d = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["percent_change_24h"];
                coin.MarketPrice.SourceRet7d = (double)jarray.SelectToken("[?(@.symbol == '" + coin.Symbol + "')]")["percent_change_7d"];
                coin.MarketPrice.PriceBefore24h = coin.MarketPrice.LatestPrice / (coin.MarketPrice.SourceRet1d / 100 + 1);

			}

			//string CoinChartsUrl = "http://api.cryptocoincharts.info/tradingPair/";

            //if (!Reachability.IsHostReachable(CoinChartsUrl))
            //{
            //    return EnuAppStatus.FailureNetwork;
            //}
            //else
            //{

            //    //Parse Market Data 
            //    if (coin.MarketPrice == null)
            //    {
            //        var p = new Price(coin);
            //        coin.MarketPrice = p;
            //    }

            //    CoinChartsUrl = (coin.Symbol == "BTC") ? CoinChartsUrl + "btc_usd" : CoinChartsUrl + coin.Symbol.ToLower() + "_btc";

            //    using (var http = new HttpClient())
            //    {
            //        rawjson = await http.GetStringAsync(CoinChartsUrl);
            //    }

            //    var json = await Task.Run(() => JObject.Parse(rawjson));

            //    coin.MarketPrice.LatestPriceBTC = coin.Symbol == "BTC" ? 1 : (double)json["price"];
            //    coin.MarketPrice.PriceBefore24h = (double)json["price_before_24h"];
            //    coin.MarketPrice.DayVolume = (double)json["volume_btc"];
            //    coin.MarketPrice.PriceDate = DateTime.Now;

            //}

            return EnuAppStatus.Success;
        }


        public static async Task FetchMarketDataFromBalance(Balance mybal)
		{
            foreach (var pos in mybal.positions)
            {
                if (pos.Coin != null) await FetchCoinMarketDataAsync(pos.Coin);
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
                    coin.LogoFileName = "Images/" + (string)elem["name"] + ".png";
                    var p = new Price(coin);
                    p.SourceCurrency = ApplicationCore.BaseCurrency;
                    coin.MarketPrice = p;
                    instruments.Add(coin);
                }

                return EnuAppStatus.Success;
            }
		}
    }
}
