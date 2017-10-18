using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class BItstampAPI
    {
		private const string BaseUrl = "https://www.bitstamp.net/api/v2/ticker/";

        public static async Task<EnuAPIStatus> FetchPriceAsync(Exchange bitstamp, InstrumentList coins, CrossRate crossrate)
        {
            string rawjson;

            if (!Reachability.IsHostReachable(BaseUrl))
            {
                return EnuAPIStatus.FailureNetwork;
            }
            else
            {
                foreach (var coin in coins.Where(x => x.PriceSourceCode == "Bitstamp"))
                {
                    var currency_pair = coin.Symbol1 == "BTC" ? "btcusd" : bitstamp.GetSymbolForExchange(coin.Id).ToLower() + "btc";

                    using (var http = new HttpClient())
                    {
                        http.BaseAddress = new Uri(BaseUrl);
                        Uri path = new Uri(currency_pair, UriKind.Relative);

                        HttpResponseMessage response = null;
                        while (response == null)
                        {
                            try
                            {
                                response = await http.GetAsync(path);
                            }
                            catch (Exception)
                            {
                                response = null;
                            }
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            return EnuAPIStatus.FailureNetwork;
                        }
                        rawjson = await response.Content.ReadAsStringAsync();
                    }
                    try
                    {
                        var jobj = await Task.Run(() => JObject.Parse(rawjson));

                        if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);

                        if (coin.Symbol1 == "BTC")
                        {
                            coin.MarketPrice.LatestPriceBTC = 1;
                            coin.MarketPrice.PriceBTCBefore24h = 1;
                            coin.MarketPrice.LatestPriceUSD = (double)jobj["last"];
                            coin.MarketPrice.PriceUSDBefore24h = (double)jobj["open"];
                        }
                        else
                        {
                            coin.MarketPrice.LatestPriceBTC = (double)jobj["last"];
                            coin.MarketPrice.PriceBTCBefore24h = (double)jobj["open"];
                            var btcprice = ApplicationCore.Bitcoin.MarketPrice;
                            if (btcprice != null)
                            {
                                coin.MarketPrice.LatestPriceUSD = (double)jobj["Last"] * btcprice.LatestPriceUSD;
                                coin.MarketPrice.PriceUSDBefore24h = (double)jobj["open"] * btcprice.PriceBTCBefore24h;
                            }
                        }

                        coin.MarketPrice.DayVolume = (double)jobj["volume"];
                        coin.MarketPrice.PriceDate = DateTime.Now;//ApplicationCore.FromEpochSeconds((long)jobj["timestamp"]);
                        coin.MarketPrice.USDCrossRate = crossrate;
                    }
                    catch (JsonException)
                    {
                        return EnuAPIStatus.FatalError;
                    }
                }

                return EnuAPIStatus.Success;
            }
        }
    }
}
