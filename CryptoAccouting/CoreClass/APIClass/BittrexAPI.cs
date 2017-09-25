using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class BittrexAPI
    {
        private const string BaseUrl = "https://bittrex.com/api/v1.1/";
        //private static string _apiKey;
        //private static string _apiSecret;

        public static async Task<EnuAPIStatus> FetchPriceAsync(Exchange bittrex, InstrumentList coins, CrossRate crossrate)
        {
            string rawjson;

            if (!Reachability.IsHostReachable(BaseUrl))
            {
                return EnuAPIStatus.FailureNetwork;
            }
            else
            {
                using (var http = new HttpClient())
                {
                    http.BaseAddress = new Uri(BaseUrl);
                    Uri path = new Uri("public/getmarketsummaries", UriKind.Relative);

                    HttpResponseMessage response = await http.GetAsync(path);
                    if (!response.IsSuccessStatusCode)
                    {
                        return EnuAPIStatus.FailureNetwork;
                    }
                    rawjson = await response.Content.ReadAsStringAsync();
                }


                var jobj = await Task.Run(() => JObject.Parse(rawjson));
                var jarray = (JArray)jobj["result"];

                var btcprice = coins.First(x => x.Symbol1 == "BTC").MarketPrice;

                foreach (var coin in coins.Where(x => x.PriceSourceCode == "Bittrex"))
                {
                    if (jarray.Any(x => (string)x["MarketName"] == "BTC-" + bittrex.GetSymbolForExchange(coin.Id)))
                    {
                        var jrow = jarray.First(x => (string)x["MarketName"] == "BTC-" + bittrex.GetSymbolForExchange(coin.Id));
                        if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);

                        coin.MarketPrice.LatestPriceBTC = (double)jrow["Last"];
                        coin.MarketPrice.PriceBTCBefore24h = (double)jrow["PrevDay"];
                        coin.MarketPrice.DayVolume = (double)jrow["Volume"];
                        coin.MarketPrice.PriceDate = (DateTime)jrow["TimeStamp"];

                        coin.MarketPrice.USDCrossRate = crossrate;
                        if (btcprice != null)
                        {
                            coin.MarketPrice.LatestPriceUSD = (double)jrow["Last"] * btcprice.LatestPriceUSD;
                            coin.MarketPrice.PriceUSDBefore24h = (double)jrow["PrevDay"] * btcprice.PriceUSDBefore24h;
                        }
                    }

                }

                return EnuAPIStatus.Success;
            }
        }

    }
}
