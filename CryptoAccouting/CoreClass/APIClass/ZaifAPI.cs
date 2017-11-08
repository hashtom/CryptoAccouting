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
    public static class ZaifAPI
    {
        private const string BaseUrl = "https://api.zaif.jp/";
        private static Exchange _zaif;
        private static CrossRate _crossrate;
        private static CrossRate _USDJPYrate;

        public static async Task FetchPriceAsync(Exchange zaif, InstrumentList coins, CrossRate crossrate, CrossRate USDJPYrate)
        {
            string rawjson;
            _zaif = zaif;
            _crossrate = crossrate;
            _USDJPYrate = USDJPYrate;

            try
            {
                foreach (var coin in coins.Where(x => x.PriceSourceCode == "Zaif"))
                {
                    //using (var http = new HttpClient())
                    //{
                        //http.BaseAddress = new Uri(BaseUrl);
                        Uri path = new Uri("api/1/ticker/" + zaif.GetSymbolForExchange(coin.Id).ToLower() + "_jpy", UriKind.Relative);
                        rawjson = await SendAsync(path);
                    //}

                    await ParsePrice(rawjson, coin);
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPriceAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<List<Position>> FetchPositionAsync(Exchange zaif)
        {
            _zaif = zaif;

            try
            {
                Uri path = new Uri("tapi", UriKind.Relative);

                var rawjson = await SendAsync(path, "get_info2");
                return ParsePosition(rawjson);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPositionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<TradeList> FetchTransactionAsync(Exchange zaif, string calendarYear = "ALL")
        {
            _zaif = zaif;

            try
            {
                //var http = new HttpClient();
                var from = calendarYear == "ALL" ? new DateTime(2012, 1, 1) : new DateTime(int.Parse(calendarYear), 1, 1);
                var to = calendarYear == "ALL" ? DateTime.Now : new DateTime(int.Parse(calendarYear), 12, 31);

                //http.BaseAddress = new Uri(BaseUrl);
                Uri path = new Uri("tapi", UriKind.Relative);

                var param = new Dictionary<string, string>
                {
                    //{ "currency_pair", "btc_jpy" },
                    //{ "count", "15"},
                    //{ "action", "bid" },
                    { "since", AppCore.ToEpochSeconds(from).ToString() },
                    { "end", AppCore.ToEpochSeconds(to).ToString() },
                    {"order", "ASC"}
                };

                var rawjson = await SendAsync(path, "trade_history", param);
                var tradelist = ParseTransaction(rawjson);
                return tradelist.Any() ? tradelist : throw new AppCoreWarning("No data returned from the Exchange.");

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
                    coin.MarketPrice.LatestPriceUSD = (double)jobj["last"] / _USDJPYrate.Rate;
                    coin.MarketPrice.PriceBTCBefore24h = 1;
                    coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD; //tmp
                }
                else
                {
                    var btcprice = AppCore.Bitcoin.MarketPrice;
                    if (btcprice != null)
                    {
                        coin.MarketPrice.LatestPriceUSD = (double)jobj["last"] / _USDJPYrate.Rate;
                        coin.MarketPrice.LatestPriceBTC = coin.MarketPrice.LatestPriceUSD / btcprice.LatestPriceUSD;
                        coin.MarketPrice.PriceBTCBefore24h = price_yesterday.LatestPriceBTC; //tmp
                        coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD; //tmp
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

        private static List<Position> ParsePosition(string rawjson)
        {
            JObject json;
            List<Position> positions;

            try
            {
                json = JObject.Parse(rawjson);
                if ((int)json.SelectToken("$.success") != 1)
                {
                    throw new AppCoreParseException("API returned error: " + rawjson);
                }
                else
                {
                    positions = new List<Position>();
                    foreach (JProperty x in (JToken)json["return"]["funds"])
                    {
                        var instrumentId = _zaif.GetIdForExchange(x.Name.ToUpper());
                        var coin = AppCore.InstrumentList.GetByInstrumentId(instrumentId);
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
            catch (Exception e)
            {
                throw new AppCoreParseException(e.GetType() + ": " + e.Message);
            }

        }

        private static TradeList ParseTransaction(string rawjson)
        {
            JObject json;

            try
            {
                json = JObject.Parse(rawjson);
                if ((int)json.SelectToken("$.success") != 1)
                {
                    throw new AppCoreParseException("API returned error: " + rawjson);
                }
                else
                {
                    var tradelist = new TradeList() { SettlementCCY = EnuCCY.JPY, TradedExchange = _zaif };
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

                        tradelist.AggregateTransaction(AppCore.InstrumentList.GetByInstrumentId(instrumentId),
                                                      "Zaif",
                                                      ebuysell,
                                                      (double)json["return"][x.Name]["amount"],
                                                       (double)json["return"][x.Name]["price"],
                                                       settleccy,
                                                      AppCore.FromEpochSeconds((long)json["return"][x.Name]["timestamp"]).Date,
                                                       (double)json["return"][x.Name]["fee"]
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


        private static async Task<string> SendAsync(Uri path, string postmethod = null, Dictionary<string, string> parameters = null)
        {

            if (!Reachability.IsHostReachable(BaseUrl))
            {
                throw new AppCoreNetworkException("Host is not reachable: " + BaseUrl);
            }
            else
            {
                double nonce = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                HttpResponseMessage res;

                using (var http = new HttpClient())
                {
                    http.BaseAddress = new Uri(BaseUrl);

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
                }

                var rawjson = await res.Content.ReadAsStringAsync();
                if (!res.IsSuccessStatusCode)
                    throw new AppCoreNetworkException("http response error. status code: " + res.StatusCode);

                return rawjson;
            }
        }
    }
}