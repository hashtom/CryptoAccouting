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

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class BittrexAPI
    {
        private const string BaseUrl = "https://bittrex.com";
        private static Exchange _bittrex;
        public const string SignHeaderName = "apisign";
        private static readonly Encoding encoding = Encoding.UTF8;

        public static async Task<EnuAPIStatus> FetchPriceAsync(Exchange bittrex, InstrumentList coins, CrossRate crossrate)
        {
            _bittrex = bittrex;

            if (!Reachability.IsHostReachable("bittrex.com"))
            {
                    return EnuAPIStatus.FatalError;
            }
            else
            {
                var rawjson = await request(HttpMethod.Get, BaseUrl + "/api/v1.1/public/getmarketsummaries", false);

                try
                {
                    var jobj = await Task.Run(() => JObject.Parse(rawjson));
                    var jarray = (JArray)jobj["result"];

                    var btcprice = ApplicationCore.Bitcoin().MarketPrice;

                    foreach (var coin in coins.Where(x => x.PriceSourceCode == "Bittrex"))
                    {
                        if (jarray.Any(x => (string)x["MarketName"] == "BTC-" + bittrex.GetSymbolForExchange(coin.Id)))
                        {
                            var jrow = jarray.First(x => (string)x["MarketName"] == "BTC-" + _bittrex.GetSymbolForExchange(coin.Id));
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
                }
                catch (JsonException)
                {
                    return EnuAPIStatus.FatalError;
                }

                return EnuAPIStatus.Success;
            }
        }

        public static async Task<List<Position>> FetchPositionAsync(Exchange bittrex)
        {
            List<Position> positions = null;
            //string filename = bittrex.Name + "Position" + ".json";
            _bittrex = bittrex;

            if (!Reachability.IsHostReachable("bittrex.com"))
            {
                return null;
            }
            else
            {
                var rawjson = await request(HttpMethod.Get, BaseUrl + "/api/v1.1/account/getbalances");
                if (rawjson != null)
                {
                    positions = ParsePosition(rawjson);
                    //if (positions != null) StorageAPI.SaveFile(rawjson, filename);
                }

                return positions;
            }

        }

        public static async Task<TradeList> FetchTransactionAsync(Exchange bittrex)
        {
            _bittrex = bittrex;

            if (!Reachability.IsHostReachable("bittrex.com"))
            {
                return null;
            }
            else
            {
                //var from = calendarYear == null ? new DateTime(2012, 1, 1) : new DateTime(int.Parse(calendarYear), 1, 1);
                //var to = calendarYear == null ? DateTime.Now : new DateTime(int.Parse(calendarYear), 12, 31);
                var rawjson = await request(HttpMethod.Get, BaseUrl + "/api/v1.1/account/getorderhistory");
                return ParseTrade(rawjson);
            }
        }


        private static List<Position> ParsePosition(string rawjson)
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

                var jarray = (JArray)json["result"];

                foreach (var elem in jarray)
                {
                    var instrumentId = _bittrex.GetIdForExchange((string)elem["Currency"]);
                    var coin = ApplicationCore.InstrumentList.GetByInstrumentId(instrumentId);
                    if (coin != null)
                    {
                        var qty = (double)elem["Balance"];
                        if (qty > 0)
                        {
                            var pos = new Position(coin)
                            {
                                Amount = qty,
                                BookedExchange = _bittrex
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

            if ((bool)json.SelectToken("$.success") != true)
            {
                return null;
            }
            else
            {
                var tradelist = new TradeList(ApplicationCore.BaseCurrency);

                var jarray = (JArray)json["result"];

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

                        EnuCCY settleccy;
                        if (symbol.Contains("BTC-"))
                        {
                            settleccy = EnuCCY.BTC;
                        }
                        else if (symbol.Contains("ETH-"))
                        {
                            settleccy = EnuCCY.ETH;
                        }
                        else if (symbol.Contains("USDT-"))
                        {
                            settleccy = EnuCCY.USDT;
                        }
                        else
                        {
                            continue;
                        }

                        symbol = symbol.Replace("BTC-", "").Replace("ETH-","").Replace("USDT-","");
                        var instrumentId = _bittrex.GetIdForExchange(symbol);

                        tradelist.AggregateTransaction(ApplicationCore.InstrumentList.GetByInstrumentId(instrumentId),
                                                      "Bittrex",
                                                      ebuysell,
                                                       (double)elem["Quantity"],
                                                       (double)elem["PricePerUnit"],
                                                       settleccy,
                                                       DateTime.Parse((string)elem["TimeStamp"]),
                                                      comm
                                                      );
                    }
                }
                return tradelist;
            }
        }

        private static string byteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("X2"); /* hex format */
            return sbinary;
        }

        private static string convertParameterListToString(IDictionary<string, string> parameters)
        {
            if (parameters.Count == 0) return "";
            return parameters.Select(param => WebUtility.UrlEncode(param.Key) + "=" + WebUtility.UrlEncode(param.Value)).Aggregate((l, r) => l + "&" + r);
        }

        private static (string uri, string hash) createRequestAuthentication(string uri) => createRequestAuthentication(uri, new Dictionary<string, string>());
        private static (string uri, string hash) createRequestAuthentication(string uri, IDictionary<string, string> parameters)
        {
            parameters = new Dictionary<string, string>(parameters);

            var nonce = DateTime.Now.Ticks;
            parameters.Add("apikey", _bittrex.Key);
            parameters.Add("nonce", nonce.ToString());

            var parameterString = convertParameterListToString(parameters);
            var completeUri = uri + "?" + parameterString;

            var uriBytes = encoding.GetBytes(completeUri);
            using (var hmac = new HMACSHA512(encoding.GetBytes(_bittrex.Secret)))
            {
                var hash = hmac.ComputeHash(uriBytes);
                var hashText = byteToString(hash);
                return (completeUri, hashText);
            }
        }

        private static HttpRequestMessage createRequest(HttpMethod httpMethod, string uri, bool includeAuthentication = true) => createRequest(httpMethod, uri, new Dictionary<string, string>(), includeAuthentication);
        private static HttpRequestMessage createRequest(HttpMethod httpMethod, string uri, IDictionary<string, string> parameters, bool includeAuthentication)
        {
            if (includeAuthentication)
            {
                (var completeUri, var hash) = createRequestAuthentication(uri, parameters);
                var request = new HttpRequestMessage(httpMethod, completeUri);
                request.Headers.Add(SignHeaderName, hash);
                return request;
            }
            else
            {
                var parameterString = convertParameterListToString(parameters);
                var completeUri = uri + "?" + parameterString;
                return new HttpRequestMessage(httpMethod, completeUri);
            }
        }

        private static async Task<string> request(HttpMethod httpMethod, string uri, bool includeAuthentication = true) => await request(httpMethod, uri, new Dictionary<string, string>(), includeAuthentication);
        private static async Task<string> request(HttpMethod httpMethod, string uri, IDictionary<string, string> parameters, bool includeAuthentication = true)
        {
            var request = createRequest(httpMethod, uri, parameters, includeAuthentication);
            var httpClient = new HttpClient();

            HttpResponseMessage response = null;
            while (response == null)
            {
                try
                {
                    response = await httpClient.SendAsync(request);
                }
                catch (Exception)
                {
                    response = null;
                }
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
