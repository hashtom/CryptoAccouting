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

        private const string BaseUrl = "https://api.bitflyer.jp";
        private const string apiver = "v1";
        private static Exchange _bitflyer;
        private static CrossRate _crossrate;
        private static CrossRate _USDJPYrate;

        public static async Task FetchPriceAsync(Exchange bitflyer, InstrumentList coins, CrossRate crossrate, CrossRate USDJPYrate)
        {
            string rawjson;
            _bitflyer = bitflyer;
            _crossrate = crossrate;
            _USDJPYrate = USDJPYrate;

            try
            {
                foreach (var coin in coins.Where(x => x.PriceSourceCode == "BitFlyer"))
                {
                    Uri path = new Uri(apiver + "/" + "ticker", UriKind.Relative);

                        var param = new Dictionary<string, string>
                        {
                            { "product_code", _bitflyer.GetSymbolForExchange(coin.Id) + "_JPY"}
                        };
                        rawjson = await SendAsync(path, HttpMethod.Get, param);

                    await ParsePrice(rawjson, coin);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPriceAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<List<Position>> FetchPositionAsync(Exchange bitflyer)
        {
            _bitflyer = bitflyer;

            try
            {
                Uri path = new Uri("/" + apiver + "/me/getbalance", UriKind.Relative);

                var rawjson = await SendAsync(path, HttpMethod.Get);
                return ParsePosition(rawjson);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPositionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<TradeList> FetchTransactionAsync(Exchange bitflyer)
        {
            _bitflyer = bitflyer;

            try
            {
                Uri path = new Uri("/" + apiver + "/me/getexecutions", UriKind.Relative);

                var param = new Dictionary<string, string>
                {
                    //Bitcoin only. to be enhanced
                    { "product_code", _bitflyer.GetSymbolForExchange("bitcoin") + "_JPY"}
                };

                var rawjson = await SendAsync(path, HttpMethod.Get, param);
                return ParseTransaction(rawjson);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchTransactionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        private static async Task ParsePrice(string rawjson, Instrument coin)
        {

            try
            {
                var jobj = await Task.Run(() => JObject.Parse(rawjson));

                if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);
                var price_yesterday = await MarketDataAPI.FetchPriceBefore24Async(coin.Id);

                if (coin.Id is "bitcoin")
                {
                    coin.MarketPrice.LatestPriceBTC = 1;
                    coin.MarketPrice.LatestPriceUSD = (double)jobj["ltp"] / _USDJPYrate.Rate;
                    coin.MarketPrice.PriceBTCBefore24h = 1;
                    coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD; //tmp
                }
                else
                {
                    var btcprice = AppCore.Bitcoin.MarketPrice;
                    if (btcprice != null)
                    {
                        coin.MarketPrice.LatestPriceUSD = (double)jobj["ltp"] / _USDJPYrate.Rate;
                        coin.MarketPrice.LatestPriceBTC = coin.MarketPrice.LatestPriceUSD / btcprice.LatestPriceUSD;
                        coin.MarketPrice.PriceBTCBefore24h = price_yesterday.LatestPriceUSD; //tmp
                        coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD; //tmp
                    }
                }

                coin.MarketPrice.DayVolume = (double)jobj["volume_by_product"];
                coin.MarketPrice.PriceDate = DateTime.Now;
                coin.MarketPrice.USDCrossRate = _crossrate;

            }
            catch (Exception e)
            {
                throw new AppCoreParseException(e.GetType() + ": " + e.Message);
            }
        }

        private static List<Position> ParsePosition(string rawjson)
        {
            try
            {
                var positions = new List<Position>();
                var jarray = JArray.Parse(rawjson);

                foreach (var elem in jarray)
                {
                    var instrumentId = _bitflyer.GetIdForExchange((string)elem["currency_code"]);
                    var coin = AppCore.InstrumentList.GetByInstrumentId(instrumentId);
                    if (coin != null)
                    {
                        var qty = (double)elem["amount"];
                        if (qty > 0)
                        {
                            var pos = new Position(coin)
                            {
                                Amount = qty,
                                BookedExchange = _bitflyer
                            };
                            positions.Add(pos);
                        }
                    }
                }

                return positions;
            }
            catch (Exception e)
            {
                throw new AppCoreParseException(e.GetType() + ": " + e.Message);
            }
        }

        private static TradeList ParseTransaction(string rawjson)
        {
            try
            {
                var tradelist = new TradeList() { SettlementCCY = EnuCCY.JPY };
                var jarray = JArray.Parse(rawjson);

                foreach (var elem in jarray)
                {

                    EnuBuySell ebuysell;
                    var buysell = (string)elem["side"];

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

                    tradelist.AggregateTransaction(AppCore.InstrumentList.GetByInstrumentId("bitcoin"),
                                                  "BitFlyer",
                                                   ebuysell,
                                                   (double)elem["size"],
                                                   (double)elem["price"],
                                                   EnuCCY.JPY,
                                                   DateTime.Parse((string)elem["exec_date"]).Date,
                                                   (double)elem["commission"]
                                                      );
                }

                return tradelist;
            }
            catch (Exception e)
            {
                throw new AppCoreParseException(e.GetType() + ": " + e.Message);
            }
        }

        internal static async Task<string> SendAsync(Uri path, HttpMethod method, Dictionary<string, string> parameters = null, object body = null)
        {

            if (!Reachability.IsHostReachable(BaseUrl))
            {
                throw new AppCoreNetworkException("Host is not reachable: " + BaseUrl);
            }
            else
            {
                HttpResponseMessage res;

                if (parameters != null && parameters.Any())
                {
                    var content = new FormUrlEncodedContent(parameters);
                    string q = await content.ReadAsStringAsync();

                    path = new Uri(path + "?" + q, UriKind.Relative);
                }

                using (var request = new HttpRequestMessage(method, path))
                using (var http = new HttpClient())
                {
                    http.BaseAddress = new Uri(BaseUrl);

                    string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                    string jsonBody = body == null ? "" : JsonConvert.SerializeObject(body);
                    string message = timestamp + method + path + jsonBody;

                    byte[] hash = new HMACSHA256(Encoding.UTF8.GetBytes(_bitflyer.Secret)).ComputeHash(Encoding.UTF8.GetBytes(message));
                    string sign = BitConverter.ToString(hash).ToLower().Replace("-", "");

                    request.Headers.Add("ACCESS-KEY", _bitflyer.Key);
                    request.Headers.Add("ACCESS-TIMESTAMP", timestamp);
                    request.Headers.Add("ACCESS-SIGN", sign);

                    res = await http.SendAsync(request);

                }

                var rawjson = await res.Content.ReadAsStringAsync();
                if (!res.IsSuccessStatusCode)
                    throw new AppCoreNetworkException("http response error. status code: " + res.StatusCode);

                return rawjson;
            }

        }
    }
}

