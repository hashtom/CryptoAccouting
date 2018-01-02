using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CoinBalance.CoreModel;

namespace CoinBalance.CoreAPI
{
    public static class BItstampAPI
    {
        private const string BaseUrl = "https://www.bitstamp.net/api/v2/";
        private static Exchange _bitstamp;
        private static readonly Encoding encoding = Encoding.UTF8;

        public static async Task FetchPriceAsync(Exchange bitstamp, InstrumentList coins)
        {
            _bitstamp = bitstamp;

            try
            {
                foreach (var coin in coins.Where(x => x.PriceSourceCode == bitstamp.Code))
                {
                    var currency_pair = coin.Symbol1 == "BTC" ? "btcusd" : bitstamp.GetSymbolForExchange(coin.Id).ToLower() + "btc";
                    var rawjson = await SendAsync(HttpMethod.Get, BaseUrl + "ticker/" + currency_pair, false);
                    await ParsePrice(rawjson, coin);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": BitstampAPI: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        public static async Task<List<Position>> FetchPositionAsync(Exchange bitstamp)
        {
            _bitstamp = bitstamp;

            try
            {
                var rawjson = await SendAsync(HttpMethod.Post, BaseUrl + "balance/");
                return ParsePosition(rawjson);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": BitstampAPI: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<TradeList> FetchTransactionAsync(Exchange bitstamp, int calendarYear = 0)
        {
            _bitstamp = bitstamp;
            JArray trades = new JArray();
            var path = BaseUrl + "user_transactions/";
            int limit = 500;
            int offset = 0;
            var from = calendarYear == 0 ? new DateTime(2012, 1, 1) : new DateTime(calendarYear, 1, 1);
            var to = new DateTime(DateTime.Now.Year, 12, 31);

            var param = new Dictionary<string, string>();
            param.Add("limit", limit.ToString());
       
            try
            {
                var rawjson = await SendAsync(HttpMethod.Post, path, param);

                while (true)
                {
                    var jarray = JArray.Parse(rawjson);
                    foreach (var token in jarray)
                    {
                        if (from < DateTime.Parse((string)token["datetime"]) &&
                            to >= DateTime.Parse((string)token["datetime"]))
                        {
                            trades.Add(token);
                        }
                    }

                    if (jarray.Count() == 0 || limit > jarray.Count())
                    {
                        break;
                    }

                    if(DateTime.Parse((string)jarray.Last["datetime"]) < from)
                    {
                        break;
                    }

                    param.Clear();
                    param.Add("limit", limit.ToString());
                    param.Add("offset", offset.ToString());
                    rawjson = await SendAsync(HttpMethod.Post, path, param);

                    offset += limit;
                }

                return ParseTransaction(trades);

                //return tradelist.Any() ? tradelist : throw new AppCoreWarning("No data returned from the Exchange.");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": BitstampAPI: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        private static async Task ParsePrice(string rawjson, Instrument coin)
        {
            try
            {
                var jobj = await Task.Run(() => JObject.Parse(rawjson));

                if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);
                //var price_yesterday = await MarketDataAPI.FetchPriceBefore24Async(coin.Id);

                if (coin.Id is "bitcoin")
                {
                    coin.MarketPrice.LatestPriceBTC = 1;
                    coin.MarketPrice.PriceBTCBefore24h = 1;
                    coin.MarketPrice.LatestPriceUSD = (double)jobj["last"];
                    //coin.MarketPrice.PriceUSDBefore24h = (double)jobj["open"];
                    //coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD; //tmp
                }
                else
                {
                    coin.MarketPrice.LatestPriceBTC = (double)jobj["last"];
                    //coin.MarketPrice.PriceBTCBefore24h = price_yesterday.LatestPriceBTC;
                    var btcprice = AppCore.Bitcoin.MarketPrice;
                    if (btcprice != null)
                    {
                        coin.MarketPrice.LatestPriceUSD = (double)jobj["last"] * btcprice.LatestPriceUSD;
                        //coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD; //(double)jobj["open"] * btcprice.PriceBTCBefore24h;
                    }
                }

                coin.MarketPrice.DayVolume = (double)jobj["volume"] * coin.MarketPrice.LatestPriceBTC;
                coin.MarketPrice.PriceDate = DateTime.Now;//ApplicationCore.FromEpochSeconds((long)jobj["timestamp"]);
                                                          //coin.MarketPrice.USDCrossRate = crossrate;
            }
            catch (Exception e)
            {
                throw new AppCoreParseException(e.GetType() + ": BitstampAPI: " + e.Message);
            }
        }

        private static List<Position> ParsePosition(string rawjson)
        {
            try
            {
                List<Position> positions;
                var json = JObject.Parse(rawjson);
                positions = new List<Position>();

                foreach(var coin in _bitstamp.ListedCoins)
                {
                    var qty = (double)json[_bitstamp.GetSymbolForExchange(coin.Id).ToLower() + "_balance"];
                    if(qty > 0)
                    {
                        var pos = new Position(coin)
                        {
                            Amount = qty,
                            BookedExchange = _bitstamp
                        };
                        positions.Add(pos);
                    }
                }

                return positions;
            }
            catch (Exception e)
            {
                throw new AppCoreParseException(e.GetType() + " :BitstampAPI: " + e.Message);
            }

        }

        private static TradeList ParseTransaction(JArray jarray)
        {
            string symbol;
            EnuSide ebuysell;
            EnuCCY settleCur;
            double price;

            try
            {
                var tradelist = new TradeList() { SettlementCCY = EnuCCY.USD, TradedExchange = _bitstamp };
                //var jarray = JArray.Parse(rawjson);

                foreach (var elem in jarray)
                {

                    if ((string)elem["type"] == "2") // MarketTrade
                    {
                        if ((string)elem["btc"] != null)
                        {
                            symbol = "BTC";
                        }
                        else if((string)elem["xrp"] != null)
                        {
                            symbol = "XRP";
                        }
                        else
                        {
                            continue;
                        }

                        if(elem["usd"] != null)
                        {
                            ebuysell = (double)elem["usd"] > 0 ? EnuSide.Sell : EnuSide.Buy;
                            settleCur = EnuCCY.USD;
                            price = (double)elem["btc_usd"];
                        }
                        else if (elem["eur"] != null)
                        {
                            ebuysell = (double)elem["eur"] > 0 ? EnuSide.Sell : EnuSide.Buy;
                            settleCur = EnuCCY.EUR;
                            price = (double)elem["btc_eur"];
                        }
                        else
                        {
                            continue;
                        }

                        tradelist.AggregateTransaction(symbol,
                                                       AssetType.Cash,
                                                       ebuysell,
                                                       (double)elem["btc"],
                                                       price,
                                                       settleCur,
                                                       DateTime.Parse((string)elem["datetime"]).Date,
                                                       (double)elem["fee"],
                                                       _bitstamp
                                                          );
                    }
                }

                return tradelist;
            }
            catch (Exception e)
            {
                throw new AppCoreParseException(e.GetType() + ": BitstampAPI: " + e.Message);
            }
        }

        private static string convertParameterListToString(IDictionary<string, string> parameters)
        {
            if (parameters.Count == 0) return "";
            return parameters.Select(param => WebUtility.UrlEncode(param.Key) + "=" + WebUtility.UrlEncode(param.Value)).Aggregate((l, r) => l + "&" + r);
        }

        private static async Task<string> SendAsync(HttpMethod httpMethod, string uri, bool includeAuthentication = true) => await SendAsync(httpMethod, uri, new Dictionary<string, string>(), includeAuthentication);
        private static async Task<string> SendAsync(HttpMethod httpMethod, string uri, IDictionary<string, string> parameters, bool includeAuthentication = true)
        {
            HttpRequestMessage request;

            if (includeAuthentication)
            {
                var nonce = DateTime.Now.Ticks;
                var msg = string.Format("{0}{1}{2}", nonce.ToString(), _bitstamp.CustomerID, _bitstamp.Key);
                var signature = ByteArrayToString(SignHMACSHA256(_bitstamp.Secret, StringToByteArray(msg))).ToUpper();

                parameters.Add("key", _bitstamp.Key);
                parameters.Add("signature", signature);
                parameters.Add("nonce", nonce.ToString());

                request = new HttpRequestMessage(httpMethod, uri);
                request.Content = new FormUrlEncodedContent(parameters);
            }
            else
            {
                var parameterString = convertParameterListToString(parameters);
                var completeUri = uri + "?" + parameterString;
                request = new HttpRequestMessage(httpMethod, completeUri);
            }

            using (var http = new HttpClient())
            {
                var response = await http.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }

        }

        private static byte[] SignHMACSHA256(string key, byte[] data)
        {
            var hashMaker = new HMACSHA256(Encoding.ASCII.GetBytes(key));
            return hashMaker.ComputeHash(data);
        }

        private static byte[] StringToByteArray(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        private static string ByteArrayToString(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

    }
}
