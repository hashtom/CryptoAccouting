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

                ins.MarketPrice.LatestPriceBTC = (double)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["price_btc"];
                ins.MarketPrice.LatestPrice = (double)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["price_usd"];
                ins.MarketPrice.BaseCurrency = EnuBaseCCY.USD; //hardcoded temporalily
                ins.Name = (string)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["name"];
                ins.MarketPrice.DayVolume = (double)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["24h_volume_usd"] / ins.MarketPrice.LatestPriceBTC;
                ins.MarketPrice.PriceDate = ApplicationCore.FromEpochSeconds((long)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["last_updated"]).Date;
                ins.MarketPrice.Pct1h = (double)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["percent_change_1h"];
                //ins.MarketPrice.Pct1d = (double)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["percent_change_24h"];
                ins.MarketPrice.Pct7d = (double)jarray.SelectToken("[?(@.symbol == '" + ins.Symbol + "')]")["percent_change_7d"];
                //ins.MarketPrice.Pct1h = pct1h.GetType() == typeof(double) ? 0 : pct1h;
                //ins.MarketPrice.Pct1d = pct1d.GetType() == typeof(double) ? 0 : (double)pct1d;
                //ins.MarketPrice.Pct7d = pct7d.GetType() == typeof(double) ? 0 : pct7d;
            }
		}
    }
}
