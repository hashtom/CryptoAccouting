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
        private const string BaseUrl = "https://bittrex.com";
        private static string _apiKey;
        private static string _apiSecret;

        public static async Task<string> FetchPositionAsync(string apikey, string secret)
        {

            _apiKey = apikey;
            _apiSecret = secret;
            var http = new HttpClient();

            http.BaseAddress = new Uri(BaseUrl);
            Uri path = new Uri("/api/v1.1/account/", UriKind.Relative);

            return await SendAsync(http, path, "getbalances");
        }

        public static async Task<EnuAPIStatus> FetchPriceAsync(Exchange bittrex, InstrumentList coins, CrossRate crossrate)
        {
            string rawjson;

            //if (!Reachability.IsHostReachable(BaseUrl))
            //{
            //    return EnuAPIStatus.FailureNetwork;
            //}
            //else
            //{
            //using (var http = new HttpClient())
            //{
            //    http.BaseAddress = new Uri(BaseUrl);
            //    Uri path = new Uri("/public/getmarketsummaries", UriKind.Relative);

            //    HttpResponseMessage response = await http.GetAsync(path);
            //    if (!response.IsSuccessStatusCode)
            //    {
            //        return EnuAPIStatus.FailureNetwork;
            //    }
            //    rawjson = await response.Content.ReadAsStringAsync();
            //}

            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(BaseUrl);
                Uri path = new Uri("/api/v1.1/public/getmarketsummaries", UriKind.Relative);
                rawjson = await SendAsync(http, path);
            }

            var jobj = await Task.Run(() => JObject.Parse(rawjson));
            var jarray = (JArray)jobj["result"];

            var btcprice = ApplicationCore.Bitcoin().MarketPrice;

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
            //}
        }

        public static async Task<string> FetchTransactionAsync(string apikey, string secret, string calendarYear = null)
        {
            _apiKey = apikey;
            _apiSecret = secret;
            var http = new HttpClient();

            var from = calendarYear == null ? new DateTime(2012, 1, 1) : new DateTime(int.Parse(calendarYear), 1, 1);
            var to = calendarYear == null ? DateTime.Now : new DateTime(int.Parse(calendarYear), 12, 31);

            http.BaseAddress = new Uri(BaseUrl);
            Uri path = new Uri("/api/v1.1/account", UriKind.Relative);

            return await SendAsync(http, path, "getorderhistory");

        }

        private static async Task<string> SendAsync(HttpClient http, Uri path, string postmethod = null, Dictionary<string, string> parameters = null)
        {
            double nonce = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            HttpResponseMessage res;

            if (!Reachability.IsHostReachable(BaseUrl))
            {
                return null;
            }
            else
            {
                if (postmethod != null)
                {
                    if (parameters == null)
                        parameters = new Dictionary<string, string>();

                    parameters.Add("nonce", nonce.ToString());
                    parameters.Add("method", postmethod);

                    var content = new FormUrlEncodedContent(parameters);
                    string message = await content.ReadAsStringAsync();

                    byte[] hash = new HMACSHA512(Encoding.UTF8.GetBytes(_apiSecret)).ComputeHash(Encoding.UTF8.GetBytes(message));
                    string sign = BitConverter.ToString(hash).ToLower().Replace("-", "");

                    http.DefaultRequestHeaders.Clear();
                    http.DefaultRequestHeaders.Add("key", _apiKey);
                    http.DefaultRequestHeaders.Add("Sign", sign);

                    res = await http.PostAsync(path, content);
                }
                else
                {
                    res = await http.GetAsync(path);
                }

                var rawjson = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                    return null;

                return rawjson;
            }
        }

        public static List<Position> ParsePosition(string rawjson, Exchange exchange)
        {
            JObject json;
            List<Position> positions;

            try
            {
                json = JObject.Parse(rawjson);
            }
            catch (JsonException)
            {
                return null;
            }

            if ((bool)json.SelectToken("$.success") != true)
            {
                return null;
            }
            else
            {
                positions = new List<Position>();

                JArray jarray = JArray.Parse((string)json["result"]);

                foreach (var elem in jarray)
                {
                    var coin = ApplicationCore.InstrumentList.GetBySymbol1((string)elem["Currency"]);
                    if (coin != null)
                    {
                        var qty = (double)elem["Balance"];
                        if (qty > 0)
                        {
                            var pos = new Position(coin)
                            {
                                Amount = qty,
                                BookedExchange = exchange
                            };
                            positions.Add(pos);
                        }
                    }
                }

                return positions;
            }

        }

        public static TradeList ParseTrade(string rawjson)
        {
            JObject json;

            try
            {
                json = JObject.Parse(rawjson);
            }
            catch (JsonException)
            {
                return null;
            }

            if ((bool)json.SelectToken("$.success") != true)
            {
                return null;
            }
            else
            {
                var tradelist = new TradeList(ApplicationCore.BaseCurrency);

                JArray jarray = JArray.Parse((string)json["result"]);

                foreach (var elem in jarray)
                {
                    var comm = (double)elem["Commission"];
                    if (comm > 0)
                    {
                        //Transaction Date Order must be ascending by design...
                        EnuBuySell ebuysell;
                        var buysell = (string)elem["OrderType"];

                        if (buysell.Contains("BUY"))
                        {
                            ebuysell = EnuBuySell.Buy;
                        }
                        else if (buysell.Contains("SELL"))
                        {
                            ebuysell = EnuBuySell.Sell;
                        }
                        else
                        {
                            ebuysell = EnuBuySell.Check;
                        }

                        var symbol = (string)elem["Exchange"];
                        symbol = symbol.Replace("BTC-", "");

                        tradelist.AggregateTransaction(ApplicationCore.InstrumentList.GetBySymbol1(symbol),
                                                      "Bittrex",
                                                      ebuysell,
                                                       (double)elem["Quantity"],
                                                       (double)elem["PricePerUnitv"],
                                                       DateTime.Parse((string)elem["TimeStamp"]),
                                                      comm
                                                      );
                    }
                }

                return tradelist;
            }
        }

    }
}
