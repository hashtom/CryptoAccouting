using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace CoinBalance.CoreClass.APIClass
{
    public static class BitFlyerAPI
    {

        private const string BaseUrl = "https://api.bitflyer.jp/v1/";
        private static Exchange _bitflyer;
        private static CrossRate _crossrate;
        private static CrossRate _USDJPYrate;

        public static async Task FetchPriceAsync(Exchange zaif, InstrumentList coins, CrossRate crossrate, CrossRate USDJPYrate)
        {
            string rawjson;
            _bitflyer = zaif;
            _crossrate = crossrate;
            _USDJPYrate = USDJPYrate;

            try
            {
                foreach (var coin in coins.Where(x => x.PriceSourceCode == "Bitflyer"))
                {
                    using (var http = new HttpClient())
                    {
                        http.BaseAddress = new Uri(BaseUrl);
                        Uri path = new Uri("api/1/ticker/" + zaif.GetSymbolForExchange(coin.Id).ToLower() + "_jpy", UriKind.Relative);
                        rawjson = await SendAsync(http, path, HttpMethod.Get);
                    }

                    await ParsePrice(rawjson, coin);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": FetchPriceAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        private static async Task ParsePrice(string rawjson, Instrument coin)
        {

            try
            {
                var jobj = await Task.Run(() => JObject.Parse(rawjson));

                if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);

                if (coin.Id is "bitcoin")
                {
                    coin.MarketPrice.LatestPriceBTC = 1;
                    coin.MarketPrice.LatestPriceUSD = (double)jobj["last"] / _USDJPYrate.Rate;
                    coin.MarketPrice.PriceBTCBefore24h = 1;
                    coin.MarketPrice.PriceUSDBefore24h = await MarketDataAPI.FetchBTCUSDPriceBefore24hAsync(); //tmp
                }
                else
                {
                    var btcprice = AppCore.Bitcoin.MarketPrice;
                    if (btcprice != null)
                    {
                        coin.MarketPrice.LatestPriceUSD = (double)jobj["last"] / _USDJPYrate.Rate;
                        coin.MarketPrice.LatestPriceBTC = coin.MarketPrice.LatestPriceUSD / btcprice.LatestPriceUSD;
                        coin.MarketPrice.PriceBTCBefore24h = await MarketDataAPI.FetchPriceBTCBefore24hAsync(coin.Id); //tmp
                        coin.MarketPrice.PriceUSDBefore24h = coin.MarketPrice.PriceBTCBefore24h * btcprice.LatestPriceUSD; //tmp
                    }
                }

                coin.MarketPrice.DayVolume = (double)jobj["volume"] * coin.MarketPrice.LatestPriceBTC;
                coin.MarketPrice.PriceDate = DateTime.Now;
                coin.MarketPrice.USDCrossRate = _crossrate;

            }
            catch (Exception e)
            {
                throw new AppCoreParseException(e.GetType() + ": " + e.Message);
            }
        }

        internal static async Task<string> SendAsync(HttpClient http, Uri path, HttpMethod method, Dictionary<string, string> query = null, object body = null)
        {
            // 相対URLにクエリパラメータを追加
            if (query != null && query.Any())
            {
                var content = new FormUrlEncodedContent(query);
                string q = await content.ReadAsStringAsync();

                path = new Uri(path.ToString() + "?" + q, UriKind.Relative);
            }

            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            string jsonBody = body == null ? "" : JsonConvert.SerializeObject(body);
            string message = timestamp + method + path.ToString() + jsonBody;

            byte[] hash = new HMACSHA256(Encoding.UTF8.GetBytes(_bitflyer.Secret)).ComputeHash(Encoding.UTF8.GetBytes(message));
            string sign = BitConverter.ToString(hash).ToLower().Replace("-", "");//バイト配列をを16進文字列へ

            http.DefaultRequestHeaders.Clear();
            http.DefaultRequestHeaders.Add("ACCESS-KEY", _bitflyer.Key);
            http.DefaultRequestHeaders.Add("ACCESS-TIMESTAMP", timestamp);
            http.DefaultRequestHeaders.Add("ACCESS-SIGN", sign);

            HttpResponseMessage res;
            if (method == HttpMethod.Post)
            {
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                res = await http.PostAsync(path, content);
            }
            else if (method == HttpMethod.Get)
            {
                res = await http.GetAsync(path);
            }
            else
            {
                throw new AppCoreNetworkException("http method error: " + method);
            }

            string text = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new AppCoreNetworkException("http response error. status code: " + res.StatusCode);

            return text;
        }
    }
}

