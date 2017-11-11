using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        //private static CrossRate _crossrate;

        public static async Task FetchPriceAsync(Exchange poloniex, InstrumentList coins)
        {
            _poloniex = poloniex;
            //_crossrate = crossrate;
            string rawjson;

            try
            {
                Uri path = new Uri("public?command=returnTicker", UriKind.Relative);
                rawjson = await SendAsync(path, HttpMethod.Get);
                await ParsePrice(rawjson, coins);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPriceAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<List<Position>> FetchPositionAsync(Exchange poloniex)
        {
            _poloniex = poloniex;

            try
            {

                Uri path = new Uri("tradingApi", UriKind.Relative);

                var rawjson = await SendAsync(path, HttpMethod.Post, "command=returnBalances");
                return ParsePosition(rawjson);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPositionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<TradeList> FetchTransactionAsync(Exchange poloniex)
        {
            _poloniex = poloniex;

            try
            {
                var from = AppCore.ToEpochSeconds(new DateTime(2013, 1, 1)).ToString();
                var to = AppCore.ToEpochSeconds(DateTime.Now).ToString();

                Uri path = new Uri("tradingApi", UriKind.Relative);

                var rawjson = await SendAsync(path, HttpMethod.Post, "command=returnTradeHistory&currencyPair=all" + "&start=" + from + "&end=" + to);
                var tradelist = ParseTransaction(rawjson);
                return tradelist.Any() ? tradelist : throw new AppCoreWarning("No data returned from the Exchange.");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchTransactionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }


        private static async Task ParsePrice(string rawjson, InstrumentList coins)
        {
            try
            {
                var jobj = await Task.Run(() => JObject.Parse(rawjson));
                var btcjrow = jobj["USDT_BTC"];

                foreach (var coin in coins.Where(x => x.PriceSourceCode == _poloniex.Code))
                {
                    if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);
                    //var price_yesterday = await MarketDataAPI.FetchPriceBefore24Async(coin.Id);

                    if (coin.Id is "bitcoin")
                    {
                        coin.MarketPrice.LatestPriceBTC = 1;
                        coin.MarketPrice.PriceBTCBefore24h = 1;
                        coin.MarketPrice.LatestPriceUSD = (double)btcjrow["last"];
                        //coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD;
                        //coin.MarketPrice.USDCrossRate = _crossrate;
                        coin.MarketPrice.DayVolume = (double)btcjrow["quoteVolume"];
                    }
                    else if (coin.Id is "tether")
                    {
                        coin.MarketPrice.LatestPriceBTC = 1 / (double)btcjrow["last"];
                        //coin.MarketPrice.PriceBTCBefore24h = 1 / price_yesterday.LatestPriceUSD;
                        coin.MarketPrice.LatestPriceUSD = 1;
                        coin.MarketPrice.PriceUSDBefore24h = 1;
                        //coin.MarketPrice.USDCrossRate = _crossrate;
                        coin.MarketPrice.DayVolume = (double)btcjrow["quoteVolume"];
                    }
                    else
                    {
                        //var btcprice = AppCore.Bitcoin.MarketPrice;

                        var jrow = jobj["BTC_" + _poloniex.GetSymbolForExchange(coin.Id)];

                        coin.MarketPrice.LatestPriceBTC = (double)jrow["last"];
                        //coin.MarketPrice.PriceBTCBefore24h = price_yesterday.LatestPriceBTC;
                        //coin.MarketPrice.USDCrossRate = _crossrate;
                        coin.MarketPrice.LatestPriceUSD = (double)jrow["last"] * (double)btcjrow["last"];
                        //coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD;
                        coin.MarketPrice.DayVolume = (double)jrow["baseVolume"];

                    }

                    coin.MarketPrice.PriceDate = DateTime.Now;
                }
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
                var json = JObject.Parse(rawjson);

                foreach (JProperty x in (JToken)json)
                {
                    var instrumentId = _poloniex.GetIdForExchange(x.Name.ToUpper());
                    var coin = AppCore.InstrumentList.GetByInstrumentId(instrumentId);

                    if (coin != null)
                    {
                        var qty = (double)json[x.Name];
                        if (qty > 0)
                        {
                            var pos = new Position(coin)
                            {
                                Amount = qty,
                                BookedExchange = _poloniex
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
                var tradelist = new TradeList() { SettlementCCY = EnuCCY.BTC, TradedExchange = _poloniex };
                var json = JObject.Parse(rawjson);

                foreach (JProperty x in (JToken)json)
                {
                    var symbol = (string)x.Name;
                    EnuCCY settleccy;
                    if (symbol.Contains("BTC_"))
                    {
                        settleccy = EnuCCY.BTC;
                    }
                    else if (symbol.Contains("ETH_"))
                    {
                        settleccy = EnuCCY.ETH;
                    }
                    else if (symbol.Contains("USDT_"))
                    {
                        settleccy = EnuCCY.USDT;
                    }
                    else
                    {
                        continue;
                    }

                    var jarray = (JArray)json[x.Name];
                    foreach (var elem in jarray)
                    {
                        //Transaction Date Order must be ascending by design...
                        EnuBuySell ebuysell;
                        var buysell = (string)elem["type"];

                        if (buysell.Contains("buy"))
                        {
                            ebuysell = EnuBuySell.Buy;
                        }
                        else if (buysell.Contains("sell"))
                        {
                            ebuysell = EnuBuySell.Sell;
                        }
                        else
                        {
                            ebuysell = EnuBuySell.Check;
                        }

                        symbol = symbol.Replace("BTC_", "").Replace("ETH_", "").Replace("USDT_", "");
                        //var instrumentId = _poloniex.GetIdForExchange(symbol);

                        tradelist.AggregateTransaction(symbol,
                                                      ebuysell,
                                                       (double)elem["amount"],
                                                       (double)elem["rate"],
                                                       settleccy,
                                                       DateTime.Parse((string)elem["date"]),
                                                       (double)elem["fee"]
                                                       ,_poloniex
                                                      );
                    }
                }

                return tradelist;
            }
            catch (Exception e)
            {
                throw new AppCoreParseException(e.GetType() + ": " + e.Message);
            }
        }

        private static async Task<string> SendAsync(Uri path, HttpMethod method, string parameters = null)
        {
            HttpResponseMessage res;
            string rawjson;

            using (var request = new HttpRequestMessage(method, path))
            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(BaseUrl);

                if (method == HttpMethod.Post)
                {
                    string nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                    string myParam = parameters + "&nonce=" + nonce;

                    request.Content = new StringContent(myParam);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    byte[] hash = new HMACSHA512(Encoding.ASCII.GetBytes(_poloniex.Secret)).ComputeHash(Encoding.ASCII.GetBytes(myParam));
                    string sign = BitConverter.ToString(hash).Replace("-", "").ToLower();

                    request.Headers.Add("Key", _poloniex.Key);
                    request.Headers.Add("Sign", sign);
                }

                res = await http.SendAsync(request);
            }

            res.EnsureSuccessStatusCode();
            rawjson = await res.Content.ReadAsStringAsync();
            //if (!res.IsSuccessStatusCode)
                //throw new AppCoreNetworkException("http response error. status code: " + res.StatusCode);
            return rawjson;
        }
    }
}
