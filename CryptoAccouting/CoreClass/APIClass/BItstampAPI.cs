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

namespace CoinBalance.CoreClass.APIClass
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
                //return ParsePosition(rawjson);
                return null;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": BitstampAPI: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<TradeList> FetchTransactionAsync(Exchange bitstamp)
        {
            _bitstamp = bitstamp;

            try
            {
                var rawjson = await SendAsync(HttpMethod.Post, BaseUrl + "user_transactions/");
                //var tradelist = ParseTransaction(rawjson);
                //return tradelist.Any() ? tradelist : throw new AppCoreWarning("No data returned from the Exchange.");
                return null;
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
