using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptoAccouting.CoreClass
{
    public static class MarketDataAPI
    {

        //Obtain market data from coinmarketcap API
        public static async Task FetchCoinMarketData(List<Instrument> instrments)
		{

            const string BaseUrl = "https://api.coinmarketcap.com/v1/ticker/";
            string rawjson;

            //Parse Market Data 
            foreach (Instrument ins in instrments)
            {
                if (ins.MarketPrice == null)
                {
                    var p = new Price(ins);
                    p.BaseCurrency = EnuBaseCCY.JPY;
                    ins.MarketPrice = p;
                }

				using (var http = new HttpClient())
				{
					//http.MaxResponseContentBufferSize = 256000;
                    rawjson = await http.GetStringAsync(BaseUrl + ins.Id);
				}

				var jarray = await Task.Run(() => JArray.Parse(rawjson));

                ins.MarketPrice.LatestPrice = (double)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["price_btc"];
                ins.Name = (string)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["name"];
                ins.MarketPrice.DayVolume = (double)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["24h_volume_usd"] / ins.MarketPrice.LatestPrice;
                ins.MarketPrice.PriceDate = ApplicationCore.FromEpochSeconds((long)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["last_updated"]).Date;
                ins.MarketPrice.Pct1h = (double)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["percent_change_1h"];
                ins.MarketPrice.Pct1d = (double)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["percent_change_24h"];
                ins.MarketPrice.Pct7d = (double)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["percent_change_7d"];
            }

		}
    }
}
