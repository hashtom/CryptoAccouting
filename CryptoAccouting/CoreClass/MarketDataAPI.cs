using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CryptoAccouting.CoreClass
{
    public static class MarketDataAPI
    {


		private static async Task<string> SendAsync(string method)
		{

            const string BaseUrl = "https://api.coinmarketcap.com/v1/ticker/";
            HttpClient http = new HttpClient();

			// POSTするメッセージを作成
			var content = new FormUrlEncodedContent(null);
			string message = await content.ReadAsStringAsync();


			// HTTPヘッダをセット
			//http.DefaultRequestHeaders.Clear();


            HttpResponseMessage res = await http.PostAsync(BaseUrl, content);
			var json = await res.Content.ReadAsStringAsync();

			//通信上の失敗
			if (!res.IsSuccessStatusCode)
				return null;

			return json;
		}
    }
}
