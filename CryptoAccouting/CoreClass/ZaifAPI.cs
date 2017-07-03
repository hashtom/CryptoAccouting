using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace CryptoAccouting
{
    public static class ZaifAPI
    {
        private const string BaseUrl = "https://api.zaif.jp/";
        private static string _apiKey;
        private static string _apiSecret;

        public static async Task<JObject> FetchPriceAsync(HttpClient http, string apikey, string secret){
            
            _apiKey = apikey;
            _apiSecret = secret;

            http.BaseAddress = new Uri(BaseUrl);
            Uri path = new Uri("api/1/ticker/btc_jpy", UriKind.Relative);

            return await SendAsync(http, path, "ticker");

		}

        public static DateTime FromEpochSeconds(this DateTime date, long EpochSeconds)
		{
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return epoch.AddSeconds(EpochSeconds);

		}

        public static async Task<JObject> FetchTransaction(HttpClient http, string apikey, string secret)
		{
			_apiKey = apikey;
			_apiSecret = secret;

			http.BaseAddress = new Uri(BaseUrl);
			Uri path = new Uri("tapi", UriKind.Relative);

            //var param = new Dictionary<string, string>
            //{
            //    { "currency_pair", "btc_jpy" },
            //    { "action", "bid" },
            //    { "price", "100000" },
            //    { "amount", "0.01" },
            //};

            return await SendAsync(http, path, "trade_history");

		}


        private static async Task<JObject> SendAsync(HttpClient http, Uri path, string method, Dictionary<string, string> parameters = null)
		{

            // nonceにunixtimeを用いる。整数だと1秒に一回しかAPIを呼べない。
			double nonce = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

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
            var json = JObject.Parse(await res.Content.ReadAsStringAsync());

			//通信上の失敗
			if (!res.IsSuccessStatusCode)
                return null;

            return json;
		}

    }


}
