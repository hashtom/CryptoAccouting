﻿using System;
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
    public static class KrakenAPI
    {
        private const string BaseUrl = "https://api.kraken.com/0/public";
        private static Exchange _kraken;
        public const string SignHeaderName = "apisign";
        private static readonly Encoding encoding = Encoding.UTF8;

        public static async Task FetchPriceAsync(Exchange kraken, InstrumentList coins)
        {
            _kraken = kraken;
            string request_symbols = null;

            foreach (var id in coins.Where(x => x.PriceSourceCode == _kraken.Code).Select(x => x.Id))
            {
                var symbol = _kraken.GetSymbolForExchange(id);
                request_symbols += id != "bitcoin" ? symbol + "XBT" : "";
            }

            var param = new Dictionary<string, string>
            {
                {"pair", "XBTUSD" + request_symbols},
            };

            try
            {

                var rawjson = await SendAsync(HttpMethod.Get, BaseUrl + "/Ticker", param, false);
                await ParsePrice(rawjson, coins);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": KrakeAPI: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        private static async Task ParsePrice(string rawjson, InstrumentList coins)
        {
            try
            {
                var jobj = await Task.Run(() => JObject.Parse(rawjson));
                if ((JArray)jobj["error"] != null)
                {
                    throw new AppCoreParseException("Kraken returned error: " + (JArray)jobj["error"].ToString());
                }
                else
                {
                    var jarray = (JArray)jobj["result"];
                    var btctoken = jarray.First(x => (string)x[0] == "XXBTZUSD");

                    foreach (var coin in coins.Where(x => x.PriceSourceCode == _kraken.Code))
                    {
                        if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);

                        if (coin.Id is "bitcoin")
                        {
                            coin.MarketPrice.LatestPriceBTC = 1;
                            coin.MarketPrice.PriceBTCBefore24h = 1;
                            coin.MarketPrice.LatestPriceUSD = (double)btctoken["last"];
                            coin.MarketPrice.PriceUSDBefore24h = (double)btctoken["open"];
                            coin.MarketPrice.DayVolume = (double)btctoken["volume"];
                            //coin.MarketPrice.PriceDate = (DateTime)btctoken["timestamp"];
                        }
                        else
                        {
                            if (jarray.Any(x => (string)x["symbol"] == _kraken.GetSymbolForExchange(coin.Id) + "BTC"))
                            {
                                var jtoken = jarray.First(x => (string)x["symbol"] == _kraken.GetSymbolForExchange(coin.Id) + "BTC");

                                coin.MarketPrice.LatestPriceBTC = (double)jtoken["last"];
                                coin.MarketPrice.PriceBTCBefore24h = (double)jtoken["open"];
                                coin.MarketPrice.LatestPriceUSD = (double)jtoken["last"] * (double)btctoken["last"];
                                coin.MarketPrice.PriceUSDBefore24h = (double)jtoken["open"] * (double)btctoken["open"];
                                coin.MarketPrice.DayVolume = (double)jtoken["volumeQuote"];
                               // coin.MarketPrice.PriceDate = (DateTime)btctoken["timestamp"];
                            }
                        }

                        coin.MarketPrice.PriceDate = DateTime.Now;
                    }
                }
            }
            catch (Exception e)
            {
                throw new AppCoreParseException(e.GetType() + ": KrakeAPI: " + e.Message);
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
            parameters.Add("apikey", _kraken.Key);
            parameters.Add("nonce", nonce.ToString());

            var parameterString = convertParameterListToString(parameters);
            var completeUri = uri + "?" + parameterString;

            var uriBytes = encoding.GetBytes(completeUri);
            using (var hmac = new HMACSHA512(encoding.GetBytes(_kraken.Secret)))
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

        private static async Task<string> SendAsync(HttpMethod httpMethod, string uri, bool includeAuthentication = true) => await SendAsync(httpMethod, uri, new Dictionary<string, string>(), includeAuthentication);
        private static async Task<string> SendAsync(HttpMethod httpMethod, string uri, IDictionary<string, string> parameters, bool includeAuthentication = true)
        {
            var request = createRequest(httpMethod, uri, parameters, includeAuthentication);
            using (var http = new HttpClient())
            {
                var response = await http.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

    }
}
