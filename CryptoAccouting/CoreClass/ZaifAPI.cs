using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Linq;

namespace CryptoAccouting
{
    public static class ZaifAPI
    {
		const string BaseUrl = "https://api.zaif.jp/";
        static string _apiKey;
        static string _apiSecret;

        public static async Task<string> FetchPriceAsync(HttpClient http, string apikey, string secret){
            
            _apiKey = apikey;
            _apiSecret = secret;

            http.BaseAddress = new Uri("https://api.zaif.jp");
            Uri path = new Uri("/api/1/depth/btc_jpy", UriKind.Relative);

            return await SendAsync(http, path, "depth");

        }

        public static void FetchTransaction(HttpClient http)
		{

		}


		static async Task<string> SendAsync(HttpClient http, Uri path, string method, Dictionary<string, string> parameters = null)
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

			// 送信
			HttpResponseMessage res = await http.PostAsync(path, content);

			//返答内容を取得
			string text = await res.Content.ReadAsStringAsync();

			//通信上の失敗
			if (!res.IsSuccessStatusCode)
				return "";

			return text;
		}

    }


}
