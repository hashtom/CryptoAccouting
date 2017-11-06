using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinBalance.CoreClass.APIClass
{
    public static class PoloniexAPI
    {
        private const string BaseUrl = "https://poloniex.com/";
        private static Exchange _poloniex;

        public static async Task FetchPriceAsync(Exchange poloniex, InstrumentList coins, CrossRate crossrate)
        {
            _poloniex = poloniex;
            string rawjson;

            try
            {
                using (var http = new HttpClient())
                {
                    http.BaseAddress = new Uri(BaseUrl);
                    Uri path = new Uri("public?command=returnTicker", UriKind.Relative);

                    var res = await http.GetAsync(path);
                    if (!res.IsSuccessStatusCode)
                        throw new AppCoreNetworkException("http response error. status code: " + res.StatusCode);
                    
                    rawjson = await res.Content.ReadAsStringAsync();
                }

                var jobj = await Task.Run(() => JObject.Parse(rawjson));

                foreach (var coin in coins.Where(x => x.PriceSourceCode == "Poloniex"))
                {
                    if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);
                    JToken jrow;

                    var price_yesterday = await MarketDataAPI.FetchPriceBefore24Async(coin.Id);

                    if (coin.Id is "bitcoin")
                    {
                        jrow = jobj["USDT_BTC"];

                        coin.MarketPrice.LatestPriceBTC = 1;
                        coin.MarketPrice.PriceBTCBefore24h = 1;
                        //coin.MarketPrice.DayVolume = (double)jrow["baseVolume"];
                        //coin.MarketPrice.PriceDate = (DateTime)jrow["TimeStamp"];
                        coin.MarketPrice.LatestPriceUSD = (double)jrow["last"];
                        coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD;
                        coin.MarketPrice.USDCrossRate = crossrate;
                    }
                    else
                    {
                        var btcprice = AppCore.Bitcoin.MarketPrice;

                        jrow = jobj["BTC_" + _poloniex.GetSymbolForExchange(coin.Id)];

                        coin.MarketPrice.LatestPriceBTC = (double)jrow["last"];
                        coin.MarketPrice.PriceBTCBefore24h = price_yesterday.LatestPriceBTC;
                        //coin.MarketPrice.DayVolume = (double)jrow["baseVolume"];
                        //coin.MarketPrice.PriceDate = (DateTime)jrow["TimeStamp"];

                        coin.MarketPrice.USDCrossRate = crossrate;
                        if (btcprice != null)
                        {
                            coin.MarketPrice.LatestPriceUSD = (double)jrow["last"] * btcprice.LatestPriceUSD;
                            coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD;
                        }

                    }

                    coin.MarketPrice.DayVolume = (double)jrow["baseVolume"];
                    coin.MarketPrice.PriceDate = DateTime.Now;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPriceAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

    }
}
