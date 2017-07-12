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
        
        public static async Task FetchCoinMarketData(List<Instrument> instrments)
		{

            const string BaseUrl = "https://api.coinmarketcap.com/v1/ticker/";
            string rawjson;
            using(var http = new HttpClient()){
                //http.MaxResponseContentBufferSize = 256000;
                rawjson = await http.GetStringAsync(BaseUrl);
            }

            var jarray = await Task.Run(() => JArray.Parse(rawjson));

            //Parse Market Data 
            foreach (Instrument ins in instrments)
            {
                if (ins.MarketPrice == null)
                {
                    var p = new Price(ins);
                    p.BaseCurrency = EnuBaseCCY.JPY;
                    ins.MarketPrice = p;
                }

                ins.MarketPrice.LatestPrice = (double)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["price_btc"];
                ins.Name = (string)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["name"];
                ins.MarketPrice.DayVolume = (double)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["24h_volume_usd"];
                //ins.MarketPrice.PriceDate = (DateTime)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["last_updated"];
                //ApplicationCore.FromEpochSeconds(DateTime.Now(), (long)json["return"][x.Name]["timestamp"]).Date,
            }

		}
    }
}
