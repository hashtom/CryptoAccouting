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
		//private static string _apiKey;
		//private static string _apiSecret;

		public static async Task<EnuAPIStatus> FetchPriceAsync(InstrumentList coins, CrossRate crossrate)
		{
			string rawjson;

            foreach (var coin in coins.Where(x=>x.PriceSourceCode=="Bitstamp"))
            {
                var currency_pair = coin.Symbol == "BTC" ? "btcusd" : coin.Symbol.ToLower() + "btc";

                using (var http = new HttpClient())
                {
                    http.BaseAddress = new Uri(BaseUrl);
                    Uri path = new Uri(currency_pair, UriKind.Relative);

                    HttpResponseMessage response = await http.GetAsync(path);
                    if (!response.IsSuccessStatusCode)
                    {
                        return EnuAPIStatus.FailureNetwork;
                    }
                    rawjson = await response.Content.ReadAsStringAsync();
                }

                var jobj = await Task.Run(() => JObject.Parse(rawjson));

                if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);

                if (coin.Symbol == "BTC")
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
                    var btcprice = coins.Where(x => x.Symbol == "BTC").Select(x => x.MarketPrice).First();
                    if (btcprice != null)
                    {
                        coin.MarketPrice.LatestPriceUSD = (double)jobj["Last"] * btcprice.LatestPriceUSD;
                        coin.MarketPrice.PriceUSDBefore24h = (double)jobj["open"] * btcprice.PriceBTCBefore24h;
                    }
                }

				coin.MarketPrice.DayVolume = (double)jobj["volume"];
                coin.MarketPrice.PriceDate = ApplicationCore.FromEpochSeconds((long)jobj["timestamp"]).Date;
				coin.MarketPrice.USDCrossRate = crossrate;
            }

			return EnuAPIStatus.Success;
		}
    }
}
