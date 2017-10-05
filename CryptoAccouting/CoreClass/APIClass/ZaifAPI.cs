using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class ZaifAPI
    {
        private const string BaseUrl = "https://api.zaif.jp/";
        private static string _apiKey;
        private static string _apiSecret;

        public static async Task<string> FetchPositionAsync(string apikey, string secret)
        {

            _apiKey = apikey;
            _apiSecret = secret;
            var http = new HttpClient();

            http.BaseAddress = new Uri(BaseUrl);
            Uri path = new Uri("tapi", UriKind.Relative);

            return await SendAsync(http, path, "get_info2");
        }

        public static async Task<string> FetchPriceAsync(string apikey, string secret)
        {

            _apiKey = apikey;
            _apiSecret = secret;
            var http = new HttpClient();

            http.BaseAddress = new Uri(BaseUrl);
            Uri path = new Uri("api/1/ticker/btc_jpy", UriKind.Relative);

            return await SendAsync(http, path, "ticker");
        }

        public static async Task<string> FetchTransactionAsync(string apikey, string secret, string calendarYear = null)
        {
            _apiKey = apikey;
            _apiSecret = secret;
            var http = new HttpClient();


            var from = calendarYear == null ? new DateTime(2012, 1, 1) : new DateTime(int.Parse(calendarYear), 1, 1);
            var to = calendarYear == null ? DateTime.Now : new DateTime(int.Parse(calendarYear), 12, 31);

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

            return await SendAsync(http, path, "trade_history", param);

        }


        private static async Task<string> SendAsync(HttpClient http, Uri path, string method, Dictionary<string, string> parameters = null)
        {

            string json;
            double nonce = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            if (!Reachability.IsHostReachable(BaseUrl))
            {
                json = null;
            }
            else
            {
                //パラメータなしの場合
                if (parameters == null)
                    parameters = new Dictionary<string, string>();

                //ノンスとメソッドを追加
                parameters.Add("nonce", nonce.ToString());
                parameters.Add("method", method);

                // POSTするメッセージを作成
                var content = new FormUrlEncodedContent(parameters);
                string message = await content.ReadAsStringAsync();

                // メッセージをHMACSHA512で署名
                byte[] hash = new HMACSHA512(Encoding.UTF8.GetBytes(_apiSecret)).ComputeHash(Encoding.UTF8.GetBytes(message));
                string sign = BitConverter.ToString(hash).ToLower().Replace("-", "");

                // HTTPヘッダをセット
                http.DefaultRequestHeaders.Clear();
                http.DefaultRequestHeaders.Add("key", _apiKey);
                http.DefaultRequestHeaders.Add("Sign", sign);

                HttpResponseMessage res = await http.PostAsync(path, content);
                json = await res.Content.ReadAsStringAsync();

                //通信上の失敗
                if (!res.IsSuccessStatusCode)
                    json = null;

            }

            return json;
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

            if ((int)json.SelectToken("$.success") != 1)
            {
                return null;
            }
            else
            {
                var tradelist = new TradeList(ApplicationCore.BaseCurrency);
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
                    symbol = symbol.Replace("_jpy", "").Replace("_btc", "").ToUpper();

                    tradelist.AggregateTransaction(ApplicationCore.InstrumentList.GetBySymbol1(symbol),
                                                  "Zaif",
                                                  ebuysell,
                                                  (double)json["return"][x.Name]["amount"],
                                                  (double)json["return"][x.Name]["price"],
                                                  ApplicationCore.FromEpochSeconds((long)json["return"][x.Name]["timestamp"]).Date,
                                                  (int)json["return"][x.Name]["fee"]
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

            if ((int)json.SelectToken("$.success") != 1)
            {
                return null;
            }
            else
            {
                positions = new List<Position>();

                foreach (JProperty x in (JToken)json["return"]["funds"])
                {
                    var coin = ApplicationCore.InstrumentList.GetBySymbol1(x.Name.ToUpper());
                    if (coin != null)
                    {
                        var qty = (double)json["return"]["funds"][x.Name];
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
