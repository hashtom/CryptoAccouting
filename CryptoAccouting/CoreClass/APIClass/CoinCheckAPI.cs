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
        private static string _apiKey = "";
        private static string _apiSecret = "";

        public static async Task<EnuAPIStatus> FetchPriceAsync(Exchange coincheck, InstrumentList coins, CrossRate crossrate)
        {
            string rawjson;
            Price btcprice;

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

        public static async Task<List<Position>> FetchPositionAsync(Exchange coincheck)
        {
            List<Position> positions = null;
            string filename = coincheck.Name + "Position" + ".json";
            _apiKey = coincheck.Key;
            _apiSecret = coincheck.Secret;

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

        public static async Task<TradeList> FetchTransactionAsync(Exchange coincheck, string calendarYear = null)
        {
            string rawjson;
            _apiKey = coincheck.Key;
            _apiSecret = coincheck.Secret;
            string filename = coincheck.Name + "Transaction_" + calendarYear + ".json";

            rawjson = StorageAPI.LoadFromFile(filename);

            if (rawjson is null || calendarYear == DateTime.Now.Year.ToString() || calendarYear is null)
            {

                var from = calendarYear == null ? new DateTime(2012, 1, 1) : new DateTime(int.Parse(calendarYear), 1, 1);
                var to = calendarYear == null ? DateTime.Now : new DateTime(int.Parse(calendarYear), 12, 31);
                var http = new HttpClient
                {
                    BaseAddress = new Uri(BaseUrl)
                };

                Uri path = new Uri("/api/exchange/orders/transactions_pagination", UriKind.Relative);

                var param = new Dictionary<string, string>
                {
                    //{ "limit", "5" },
                    //{ "order", "desc" }
                    //{ "starting_after", "null" },
                    //{"ending_before", "null"}
                };

                rawjson = await SendAsync(http, path, HttpMethod.Get);
                //rawjson = await SendAsync(http, path, "POST", param);
            }

            return ParseTrade(rawjson);
            //if (tradelist != null) StorageAPI.SaveFile(rawjson, filename);

        }

        private static async Task<string> SendAsync(HttpClient http, Uri path, HttpMethod method, Dictionary<string, string> parameters = null)
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

                var uri = new Uri(http.BaseAddress, path);
                string message = nonce + uri + param;

                byte[] hash = new HMACSHA256(Encoding.UTF8.GetBytes(_apiSecret)).ComputeHash(Encoding.UTF8.GetBytes(message));
                string sign = BitConverter.ToString(hash).ToLower().Replace("-", "");

                http.DefaultRequestHeaders.Clear();
                http.DefaultRequestHeaders.Add("ACCESS-KEY", _apiKey);
                http.DefaultRequestHeaders.Add("ACCESS-NONCE", nonce);
                http.DefaultRequestHeaders.Add("ACCESS-SIGNATURE", sign);

                if (method == HttpMethod.Post)
                {
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

                var jarray = (JArray)json["data"];

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
                    symbol = symbol.Replace("_jpy", "").Replace("_btc", "").ToUpper();

                    tradelist.AggregateTransaction(ApplicationCore.InstrumentList.GetBySymbol1(symbol),
                                                  "CoinCheck",
                                                  ebuysell,
                                                   Math.Abs((double)elem["funds"][symbol.ToLower()]),
                                                   (double)elem["rate"],
                                                   DateTime.Parse((string)elem["created_at"]),
                                                   (double)elem["fee"]
                                                  );
                }

                return tradelist;
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
                    var coin = ApplicationCore.InstrumentList.GetBySymbol1(x.Name.ToUpper());
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
    }
}



