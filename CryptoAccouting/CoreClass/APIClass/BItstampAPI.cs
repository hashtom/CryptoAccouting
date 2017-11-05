using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
//using System.Security.Cryptography;
//using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinBalance.CoreClass.APIClass
{
    public static class BItstampAPI
    {
        private const string BaseUrl = "https://www.bitstamp.net/api/v2/ticker/";

        public static async Task FetchPriceAsync(Exchange bitstamp, InstrumentList coins, CrossRate crossrate)
        {
            string rawjson;

            try
            {
                foreach (var coin in coins.Where(x => x.PriceSourceCode == "Bitstamp"))
                {
                    var currency_pair = coin.Symbol1 == "BTC" ? "btcusd" : bitstamp.GetSymbolForExchange(coin.Id).ToLower() + "btc";

                    using (var http = new HttpClient())
                    {
                        http.BaseAddress = new Uri(BaseUrl);
                        Uri path = new Uri(currency_pair, UriKind.Relative);

                        HttpResponseMessage response = null;
                        while (response == null)
                        {
                            try
                            {
                                response = await http.GetAsync(path);
                            }
                            catch (Exception)
                            {
                                response = null;
                            }
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new AppCoreNetworkException("http response error. status code: " + response.StatusCode);
                        }
                        rawjson = await response.Content.ReadAsStringAsync();
                    }

                    var jobj = await Task.Run(() => JObject.Parse(rawjson));

                    if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);

                    if (coin.Symbol1 == "BTC")
                    {
                        coin.MarketPrice.LatestPriceBTC = 1;
                        coin.MarketPrice.PriceBTCBefore24h = 1;
                        coin.MarketPrice.LatestPriceUSD = (double)jobj["last"];
                        //coin.MarketPrice.PriceUSDBefore24h = (double)jobj["open"];
                        coin.MarketPrice.PriceUSDBefore24h = await MarketDataAPI.FetchBTCUSDPriceBefore24hAsync(); //tmp
                    }
                    else
                    {
                        coin.MarketPrice.LatestPriceBTC = (double)jobj["last"];
                        coin.MarketPrice.PriceBTCBefore24h = (double)jobj["open"];
                        var btcprice = AppCore.Bitcoin.MarketPrice;
                        if (btcprice != null)
                        {
                            coin.MarketPrice.LatestPriceUSD = (double)jobj["last"] * btcprice.LatestPriceUSD;
                            coin.MarketPrice.PriceUSDBefore24h = (double)jobj["open"] * btcprice.PriceBTCBefore24h;
                        }
                    }

                    coin.MarketPrice.DayVolume = (double)jobj["volume"] * coin.MarketPrice.LatestPriceBTC;
                    coin.MarketPrice.PriceDate = DateTime.Now;//ApplicationCore.FromEpochSeconds((long)jobj["timestamp"]);
                    coin.MarketPrice.USDCrossRate = crossrate;

                }

                //return (EnuAPIStatus.Success, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": FetchPriceAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }
    }
}
