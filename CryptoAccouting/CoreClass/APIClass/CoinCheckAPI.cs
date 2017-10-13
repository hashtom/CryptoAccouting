using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class CoinCheckAPI
    {
        private const string BaseUrl = "https://coincheck.com";
        private static Exchange _coincheck;
        //private static string _apiKey = "";
        //private static string _apiSecret = "";

        public static async Task<EnuAPIStatus> FetchPriceAsync(Exchange coincheck, InstrumentList coins, CrossRate crossrate)
        {
            string rawjson;
            Price btcprice;
            _coincheck = coincheck;

            if (!Reachability.IsHostReachable(BaseUrl))
            {
                return EnuAPIStatus.FailureNetwork;
            }
            else
            {
                foreach (var coin in coins.Where(x => x.PriceSourceCode == "CoinCheck"))
                {
                    using (var http = new HttpClient())
                    {
                        http.BaseAddress = new Uri(BaseUrl);
                        Uri path = new Uri("/api/rate/" + coincheck.GetSymbolForExchange(coin.Id).ToLower() + "_jpy", UriKind.Relative);
                        rawjson = await SendAsync(http, path, HttpMethod.Get);
                    }

                    try
                    {
                        if (rawjson != null)
                        {
                            var jobj = await Task.Run(() => JObject.Parse(rawjson));

                            if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);

                            if (coin.Id is "bitcoin")
                            {
                                coin.MarketPrice.LatestPriceBTC = 1;
                                coin.MarketPrice.LatestPriceUSD = (double)jobj["rate"] / crossrate.Rate;
                                //coin.MarketPrice.PriceBTCBefore24h = (double)jobj["PrevDay"];
                            }
                            else
                            {
                                btcprice = ApplicationCore.Bitcoin().MarketPrice;
                                if (btcprice != null)
                                {
                                    coin.MarketPrice.LatestPriceUSD = (double)jobj["rate"] / crossrate.Rate;
                                    coin.MarketPrice.LatestPriceBTC = coin.MarketPrice.LatestPriceUSD / btcprice.LatestPriceUSD;
                                    coin.MarketPrice.PriceBTCBefore24h = await MarketDataAPI.FetchPriceBTCBefore24hAsync(coin.Id); //tmp
                                    coin.MarketPrice.PriceUSDBefore24h = coin.MarketPrice.PriceBTCBefore24h * btcprice.LatestPriceUSD;//tmp
                                }
                            }

                            coin.MarketPrice.PriceDate = DateTime.Now;
                            coin.MarketPrice.USDCrossRate = crossrate;
                        }
                    }
                    catch (JsonException)
                    {
                        return EnuAPIStatus.FatalError;
                    }
                }

                return EnuAPIStatus.Success;
            }
        }

        public static async Task<List<Position>> FetchPositionAsync(Exchange coincheck)
        {
            List<Position> positions = null;
            _coincheck = coincheck;

            string filename = coincheck.Name + "Position" + ".json";

            if (!Reachability.IsHostReachable(BaseUrl))
            {
                return null;
            }
            else
            {
                var http = new HttpClient
                {
                    BaseAddress = new Uri(BaseUrl)
                };

                Uri path = new Uri("/api/accounts/balance", UriKind.Relative);

                var rawjson = await SendAsync(http, path, HttpMethod.Get);
                if (rawjson != null)
                {
                    positions = ParsePosition(rawjson, coincheck);
                    if (positions != null) StorageAPI.SaveFile(rawjson, filename);
                }

                return positions;
            }
        }

        public static async Task<TradeList> FetchTransactionAsync(Exchange coincheck)
        {
            _coincheck = coincheck;

            if (!Reachability.IsHostReachable(BaseUrl))
            {
                return null;
            }
            else
            {
                var http = new HttpClient
                {
                    BaseAddress = new Uri(BaseUrl)
                };

                Uri path = new Uri("/api/exchange/orders/transactions", UriKind.Relative);
                var rawjson = await SendAsync(http, path, HttpMethod.Get, true);
                return ParseTrade(rawjson);
            }
        }

        public static async Task<TradeList> FetchTransaction2Async(Exchange coincheck, string calendarYear = "ALL")
        {
            _coincheck = coincheck;
            string filename = coincheck.Name + "Transaction_" + calendarYear + ".json";
            var rawjson = StorageAPI.LoadFromFile(filename);

            if (rawjson is null || calendarYear == DateTime.Now.Year.ToString() || calendarYear is "ALL")
            {
                if (!Reachability.IsHostReachable(BaseUrl))
                {
                    return null;
                }
                else
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
                    rawjson = await SendAsync(http, path, HttpMethod.Get, true, param);
                }
            }

            return ParseTrade(rawjson);
            //if (tradelist != null) StorageAPI.SaveFile(rawjson, filename);

        }

        private static async Task<string> SendAsync(HttpClient http, Uri path, HttpMethod method, bool requireAuth = false, Dictionary<string, string> parameters = null)
        {
            HttpResponseMessage res;

            if (!Reachability.IsHostReachable(BaseUrl))
            {
                return null;
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
                else if(method == HttpMethod.Get)
                {
                    res = await http.GetAsync(path);
                }
                else
                {
                    return null;
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

                foreach (JProperty x in (JToken)json)
                {
                    var instrumentId = _coincheck.GetIdForExchange(x.Name.ToUpper());
                    var coin = ApplicationCore.InstrumentList.GetByInstrumentId(instrumentId);

                    if (coin != null)
                    {
                        var qty = (double)json[x.Name];
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

        public static TradeList ParseTrade(string rawjson, bool IsPagenation = false)
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

                    tradelist.AggregateTransaction(ApplicationCore.InstrumentList.GetByInstrumentId(instrumentId),
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
    }
}



