using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
//using ServiceStack.Text;
using RestSharp;
using RestSharp.Authenticators;
using System.Linq;
using System.Xml.Linq;
using System.IO;


namespace CryptoAccouting
{
    public class ZaifAPI
    {
		// RestSharpでAPIにアクセスしservicestack.textでパース


		const string BaseUrl = "https://api.zaif.jp/";


		private readonly string _apiKey;
		private readonly string _apiSecret;
        private string _apiMethod;

		public ZaifAPI(string apiKey, string apiSecret)
		{
			_apiKey = apiKey;
			_apiSecret = apiSecret;
		}

		public T Execute<T>(RestRequest request) where T : new()
		{
            double nonce = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
			var client = new RestClient();
            client.BaseUrl = new System.Uri(BaseUrl);
			//client.Authenticator = new HttpBasicAuthenticator(_apiKey, _apiSecret);

            request.AddParameter("nonce", nonce.ToString());
			//request.AddParameter("AccountSid", _apiKey, ParameterType.UrlSegment); // used on every request
			request.AddParameter("method", _apiMethod);

            request.RequestFormat = DataFormat.Json;

			// メッセージをHMACSHA512で署名
			var content = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				{ "nonce", nonce.ToString() },
                { "method", _apiMethod }
			});
            var msg = content.ToString();

            byte[] hash = new HMACSHA512(Encoding.UTF8.GetBytes(_apiSecret)).ComputeHash(Encoding.UTF8.GetBytes(msg));
            string aaa = request.Parameters.ToString();
            string sign = BitConverter.ToString(hash).ToLower().Replace("-", "");
			request.AddHeader("key", _apiKey);
			request.AddHeader("Sign", sign);

            var response = client.Execute<T>(request);

			if (response.ErrorException != null)
			{
				const string message = "Error retrieving response.  Check inner details for more info.";
				var Exception = new ApplicationException(message, response.ErrorException);
				throw Exception;
			}
			return response.Data;
		}

        public string FetchPrice(){
            _apiMethod = "depth";
            var request = new RestRequest(Method.GET);
            request.Resource = "/api/1/depth/btc_jpy";

			//request.AddParameter(""); 

            var ret = Execute<ZaifPrice>(request);
            return ret.TestString();

        }

		public void FetchTransaction()
		{

		}


		/// <summary>Zaif取引所のAPIを実行します。
		/// </summary>
		/// <param name="http">取引所と通信する HttpClient。</param>
		/// <param name="path">APIの通信URL（取引所サイトからの相対）。</param>
		/// <param name="apiKey">APIキー。</param>
		/// <param name="secret">秘密キー。</param>
		/// <param name="method">APIのメソッド名。</param>
		/// <param name="parameters">APIのパラメータのリスト（Key:パラメータ名, Value:パラメータの値）。</param>
		/// <returns>レスポンスとして返されるJSON形式の文字列。</returns>
		static async Task<string> Send(HttpClient http, Uri path, string apiKey, string secret, string method, Dictionary<string, string> parameters = null)
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
			byte[] hash = new HMACSHA512(Encoding.UTF8.GetBytes(secret)).ComputeHash(Encoding.UTF8.GetBytes(message));
			string sign = BitConverter.ToString(hash).ToLower().Replace("-", "");

			// HTTPヘッダをセット
			http.DefaultRequestHeaders.Clear();
			http.DefaultRequestHeaders.Add("key", apiKey);
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

        public class ZaifPrice
        {
            public List<ZaifValue> Asks { get; set; }
            public List<ZaifValue> Bids { get; set; }

            public string TestString(){
                return Asks.Select(x => x.Price).First().ToString();
            }

        }

		public class ZaifValue
		{
            public double Price { get; set; }
			public double Qty { get; set; }
		}
    }


}
