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
    public static class CoinCheckAPI
    {
        private const string BaseUrl = "https://coincheck.com";
        private static Exchange _coincheck;
        private static CrossRate _crossrate;
        private static CrossRate _USDJPYrate;
        //private static string _apiKey = "";
        //private static string _apiSecret = "";

        public static async Task FetchPriceAsync(Exchange coincheck, InstrumentList coins, CrossRate crossrate, CrossRate USDJPYrate)
        {
            string rawjson;
            Price btcprice;
            _coincheck = coincheck;
            _crossrate = crossrate;
            _USDJPYrate = USDJPYrate;

            try
            {
                foreach (var coin in coins.Where(x => x.PriceSourceCode == "CoinCheck"))
                {
                    using (var http = new HttpClient())
                    {
                        http.BaseAddress = new Uri(BaseUrl);
                        Uri path = new Uri("/api/rate/" + _coincheck.GetSymbolForExchange(coin.Id).ToLower() + "_jpy", UriKind.Relative);
                        rawjson = await SendAsync(http, path, HttpMethod.Get);
                    }


                    if (rawjson != null)
                    {
                        var jobj = await Task.Run(() => JObject.Parse(rawjson));

                        if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);

                        if (coin.Id is "bitcoin")
                        {
                            coin.MarketPrice.LatestPriceBTC = 1;
                            coin.MarketPrice.LatestPriceUSD = (double)jobj["rate"] / USDJPYrate.Rate;
                            coin.MarketPrice.PriceBTCBefore24h = 1;
                            coin.MarketPrice.PriceUSDBefore24h = await MarketDataAPI.FetchBTCUSDPriceBefore24hAsync(); //tmp
                        }
                        else
                        {
                            btcprice = AppCore.Bitcoin.MarketPrice;
                            if (btcprice != null)
                            {
                                coin.MarketPrice.LatestPriceUSD = (double)jobj["rate"] / USDJPYrate.Rate;
                                coin.MarketPrice.LatestPriceBTC = coin.MarketPrice.LatestPriceUSD / btcprice.LatestPriceUSD;
                                coin.MarketPrice.PriceBTCBefore24h = await MarketDataAPI.FetchPriceBTCBefore24hAsync(coin.Id); //tmp
                                coin.MarketPrice.PriceUSDBefore24h = coin.MarketPrice.PriceBTCBefore24h * btcprice.LatestPriceUSD;//tmp
                            }
                        }
                        coin.MarketPrice.DayVolume = 0;
                        coin.MarketPrice.PriceDate = DateTime.Now;
                        coin.MarketPrice.USDCrossRate = crossrate;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": FetchPriceAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<List<Position>> FetchPositionAsync(Exchange coincheck)
        {
            _coincheck = coincheck;
            //string filename = coincheck.Name + "Position" + ".json";

            try
            {
                var http = new HttpClient
                {
                    BaseAddress = new Uri(BaseUrl)
                };

                Uri path = new Uri("/api/accounts/balance", UriKind.Relative);

                var rawjson = await SendAsync(http, path, HttpMethod.Get, true);
                return ParsePosition(rawjson);

            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": FetchPositionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        public static async Task<TradeList> FetchTransactionAsync(Exchange coincheck)
        {
            _coincheck = coincheck;

            try
            {
                var http = new HttpClient
                {
                    BaseAddress = new Uri(BaseUrl)
                };

                Uri path = new Uri("/api/exchange/orders/transactions", UriKind.Relative);
                var rawjson = await SendAsync(http, path, HttpMethod.Get, true);
                return ParseTrade(rawjson);
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": FetchTransactionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        public static async Task<TradeList> FetchTransaction2Async(Exchange coincheck, string calendarYear = "ALL")
        {
            _coincheck = coincheck;
            //string filename = coincheck.Name + "Transaction_" + calendarYear + ".json";
            //var rawjson = StorageAPI.LoadFromFile(filename);

            try
            {
                var from = calendarYear == "ALL" ? new DateTime(2012, 1, 1) : new DateTime(int.Parse(calendarYear), 1, 1);
                var to = calendarYear == "ALL" ? DateTime.Now : new DateTime(int.Parse(calendarYear), 12, 31);
                var http = new HttpClient
                {
                    BaseAddress = new Uri(BaseUrl)
                };

                Uri path = new Uri("/api/exchange/orders/transactions_pagination?limit=100?order=desc", UriKind.Relative);

                var param = new Dictionary<string, string>
                {
                    { "limit", "100" },
                    { "order", "desc" }
                    //{ "starting_after", "1" }
                    //{"ending_before", "null"}
                };

                //rawjson = await SendAsync(http, path, HttpMethod.Get);
                var rawjson = await SendAsync(http, path, HttpMethod.Get, true, param);

                return ParseTrade(rawjson);
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": FetchTransaction2Async: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        private static async Task<string> SendAsync(HttpClient http, Uri path, HttpMethod method, bool requireAuth = false, Dictionary<string, string> parameters = null)
        {
            HttpResponseMessage res;

            if (!Reachability.IsHostReachable(BaseUrl))
            {
                throw new AppCoreNetworkException("Host is not reachable: " + BaseUrl);
            }
            else
            {
                if (parameters == null)
                    parameters = new Dictionary<string, string>();

                var content = new FormUrlEncodedContent(parameters);
                string param = await content.ReadAsStringAsync();
                string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

                if (requireAuth)
                {
                    var uri = new Uri(http.BaseAddress, path);
                    string message = nonce + uri + param;

                    byte[] hash = new HMACSHA256(Encoding.UTF8.GetBytes(_coincheck.Secret)).ComputeHash(Encoding.UTF8.GetBytes(message));
                    string sign = BitConverter.ToString(hash).ToLower().Replace("-", "");

                    http.DefaultRequestHeaders.Clear();
                    http.DefaultRequestHeaders.Add("ACCESS-KEY", _coincheck.Key);
                    http.DefaultRequestHeaders.Add("ACCESS-NONCE", nonce);
                    http.DefaultRequestHeaders.Add("ACCESS-SIGNATURE", sign);
                }

                if (method == HttpMethod.Post)
                {
                    res = await http.PostAsync(path, content);
                }
                else if (method == HttpMethod.Get)
                {
                    res = await http.GetAsync(path);
                }
                else
                {
                    throw new AppCoreException("HttpMethod error: " + method);
                }

                var rawjson = await res.Content.ReadAsStringAsync();
                if (!res.IsSuccessStatusCode)
                    throw new AppCoreNetworkException("http response error. status code: " + res.StatusCode);

                return rawjson;
            }
        }

        private static List<Position> ParsePosition(string rawjson)
        {
            JObject json;
            List<Position> positions;

            try
            {
                json = JObject.Parse(rawjson);
                if ((bool)json.SelectToken("$.success") != true)
                {
                    throw new AppCoreParseException("Coincheck returned error: " + rawjson);
                }
                else
                {
                    positions = new List<Position>();

                    foreach (JProperty x in (JToken)json)
                    {
                        var instrumentId = _coincheck.GetIdForExchange(x.Name.ToUpper());
                        var coin = AppCore.InstrumentList.GetByInstrumentId(instrumentId);

                        if (coin != null)
                        {
                            var qty = (double)json[x.Name];
                            if (qty > 0)
                            {
                                var pos = new Position(coin)
                                {
                                    Amount = qty,
                                    BookedExchange = _coincheck
                                };
                                positions.Add(pos);
                            }
                        }
                    }

                    return positions;
                }
            }
            catch (Exception e)
            {
                throw new AppCoreParseException(e.GetType() + ": " + e.Message);
            }

        }

        private static TradeList ParseTrade(string rawjson, bool IsPagenation = false)
        {
            JObject json;

            try
            {
                json = JObject.Parse(rawjson);
                if ((bool)json.SelectToken("$.success") != true)
                {
                    throw new AppCoreParseException("Coincheck returned error: " + rawjson);
                }
                else
                {
                    var tradelist = new TradeList() { SettlementCCY = EnuCCY.JPY };
                    var jarray = IsPagenation ? (JArray)json["data"] : (JArray)json["transactions"];

                    foreach (var elem in jarray)
                    {
                        EnuBuySell ebuysell;

                        switch ((string)elem["side"])
                        {
                            case "buy":
                                ebuysell = EnuBuySell.Buy;
                                break;
                            case "sell":
                                ebuysell = EnuBuySell.Sell;
                                break;
                            default:
                                ebuysell = EnuBuySell.Check;
                                break;
                        }

                        var symbol = (string)elem["pair"];
                        symbol = symbol.Replace("_jpy", "").ToUpper();
                        var instrumentId = _coincheck.GetIdForExchange(symbol);

                        tradelist.AggregateTransaction(AppCore.InstrumentList.GetByInstrumentId(instrumentId),
                                                      "CoinCheck",
                                                      ebuysell,
                                                       Math.Abs((double)elem["funds"][symbol.ToLower()]),
                                                       (double)elem["rate"],
                                                       EnuCCY.JPY,
                                                       DateTime.Parse((string)elem["created_at"]),
                                                       (double)elem["fee"]
                                                      );
                    }

                    return tradelist;
                }
            }
            catch (Exception e)
            {
                throw new AppCoreParseException(e.GetType() + ": " + e.Message);
            }

        }
    }
}



