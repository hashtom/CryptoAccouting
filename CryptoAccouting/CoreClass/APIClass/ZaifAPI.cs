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
    public static class ZaifAPI
    {
        private const string BaseUrl = "https://api.zaif.jp/";
        private static Exchange _zaif;
        private static CrossRate _crossrate;
        private static CrossRate _USDJPYrate;

        public static async Task<EnuAPIStatus> FetchPriceAsync(Exchange zaif, InstrumentList coins, CrossRate crossrate, CrossRate USDJPYrate)
        {
            string rawjson;
            Price btcprice;
            _zaif = zaif;
            _crossrate = crossrate;
            _USDJPYrate = USDJPYrate;

            if (!Reachability.IsHostReachable(BaseUrl))
            {
                return EnuAPIStatus.FailureNetwork;
            }
            else
            {
                foreach (var coin in coins.Where(x => x.PriceSourceCode == "Zaif"))
                {
                    using (var http = new HttpClient())
                    {
                        http.BaseAddress = new Uri(BaseUrl);
                        Uri path = new Uri("api/1/ticker/" + zaif.GetSymbolForExchange(coin.Id).ToLower() + "_jpy", UriKind.Relative);
                        rawjson = await SendAsync(http, path);
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
                                coin.MarketPrice.LatestPriceUSD = (double)jobj["last"] / USDJPYrate.Rate;
                                coin.MarketPrice.PriceBTCBefore24h = 1;
                                coin.MarketPrice.PriceUSDBefore24h = await MarketDataAPI.FetchBTCUSDPriceBefore24hAsync(); //tmp
                            }
                            else
                            {
                                btcprice = ApplicationCore.Bitcoin.MarketPrice;
                                if (btcprice != null)
                                {
                                    coin.MarketPrice.LatestPriceUSD = (double)jobj["last"] / USDJPYrate.Rate;
                                    coin.MarketPrice.LatestPriceBTC = coin.MarketPrice.LatestPriceUSD / btcprice.LatestPriceUSD;
                                    coin.MarketPrice.PriceBTCBefore24h = await MarketDataAPI.FetchPriceBTCBefore24hAsync(coin.Id); //tmp
                                    coin.MarketPrice.PriceUSDBefore24h = coin.MarketPrice.PriceBTCBefore24h * btcprice.LatestPriceUSD; //tmp
                                }
                            }

                            coin.MarketPrice.DayVolume = (double)jobj["volume"];
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

        public static async Task<List<Position>> FetchPositionAsync(Exchange zaif)
        {
            _zaif = zaif;
            List<Position> positions = null;
            //string filename = zaif.Name + "Position" + ".json";

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

                Uri path = new Uri("tapi", UriKind.Relative);

                var rawjson = await SendAsync(http, path, "get_info2");
                if (rawjson != null)
                {
                    positions = ZaifAPI.ParsePosition(rawjson);
                    //if (positions != null) StorageAPI.SaveFile(rawjson, filename);
                }

                return positions;
            }
        }

        public static async Task<TradeList> FetchTransactionAsync(Exchange zaif, string calendarYear = "ALL")
        {
            _zaif = zaif;
            string rawjson;

            //string filename = zaif.Name + "Transaction_" + calendarYear + ".json";
            //rawjson = StorageAPI.LoadFromFile(filename);

            //if (rawjson is null || calendarYear == DateTime.Now.Year.ToString() || calendarYear is "ALL")
            //{
                if (!Reachability.IsHostReachable(BaseUrl))
                {
                    return null;
                }
                else
                {
                    var http = new HttpClient();

                    var from = calendarYear == "ALL" ? new DateTime(2012, 1, 1) : new DateTime(int.Parse(calendarYear), 1, 1);
                    var to = calendarYear == "ALL" ? DateTime.Now : new DateTime(int.Parse(calendarYear), 12, 31);

                    http.BaseAddress = new Uri(BaseUrl);
                    Uri path = new Uri("tapi", UriKind.Relative);

                    var param = new Dictionary<string, string>
                {
                    //{ "currency_pair", "btc_jpy" },
                    //{ "count", "15"},
                    //{ "action", "bid" },
                    { "since", ApplicationCore.ToEpochSeconds(from).ToString() },
                    { "end", ApplicationCore.ToEpochSeconds(to).ToString() },
                    {"order", "ASC"}
                };

                    rawjson = await SendAsync(http, path, "trade_history", param);
                }
            //}

            var tradelist = ParseTrade(rawjson);
            //if (tradelist != null) StorageAPI.SaveFile(rawjson, filename);

            return tradelist;

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

                    byte[] hash = new HMACSHA512(Encoding.UTF8.GetBytes(_zaif.Secret)).ComputeHash(Encoding.UTF8.GetBytes(message));
                    string sign = BitConverter.ToString(hash).ToLower().Replace("-", "");

                    http.DefaultRequestHeaders.Clear();
                    http.DefaultRequestHeaders.Add("key", _zaif.Key);
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

        public static List<Position> ParsePosition(string rawjson)
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

            if ((int)json.SelectToken("$.success") != 1)
            {
                return null;
            }
            else
            {
                positions = new List<Position>();

                foreach (JProperty x in (JToken)json["return"]["funds"])
                {
                    var instrumentId = _zaif.GetIdForExchange(x.Name.ToUpper());
                    var coin = ApplicationCore.InstrumentList.GetByInstrumentId(instrumentId);
                    if (coin != null)
                    {
                        var qty = (double)json["return"]["funds"][x.Name];
                        if (qty > 0)
                        {
                            var pos = new Position(coin)
                            {
                                Amount = qty,
                                BookedExchange = _zaif
                            };
                            positions.Add(pos);
                        }
                    }
                }

                return positions;
            }
        }

        private static TradeList ParseTrade(string rawjson)
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

            if ((int)json.SelectToken("$.success") != 1)
            {
                return null;
            }
            else
            {
                var tradelist = new TradeList(EnuBaseFiatCCY.JPY);
                foreach (JProperty x in (JToken)json["return"])
                {
                    //Transaction Date Order must be ascending by design...
                    EnuBuySell ebuysell;

                    switch ((string)json["return"][x.Name]["your_action"])
                    {
                        case "bid":
                            ebuysell = EnuBuySell.Buy;
                            break;
                        case "ask":
                            ebuysell = EnuBuySell.Sell;
                            break;
                        default:
                            ebuysell = EnuBuySell.Check;
                            break;
                    }

                    var symbol = (string)json["return"][x.Name]["currency_pair"];
                    EnuCCY settleccy;
                    if (symbol.Contains("_jpy"))
                    {
                        settleccy = EnuCCY.JPY;
                    }
                    else if (symbol.Contains("_btc"))
                    {
                        settleccy = EnuCCY.BTC;
                    }
                    else
                    {
                        continue;
                    }

                    symbol = symbol.Replace("_jpy", "").Replace("_btc", "").ToUpper();
                    var instrumentId = _zaif.GetIdForExchange(symbol);

                    tradelist.AggregateTransaction(ApplicationCore.InstrumentList.GetByInstrumentId(instrumentId),
                                                  "Zaif",
                                                  ebuysell,
                                                  (double)json["return"][x.Name]["amount"],
                                                   (double)json["return"][x.Name]["price"],
                                                   settleccy,
                                                  ApplicationCore.FromEpochSeconds((long)json["return"][x.Name]["timestamp"]).Date,
                                                   (double)json["return"][x.Name]["fee"]
                                                  );
                }

                return tradelist;
            }
        }
    }
}
