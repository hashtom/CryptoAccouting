using System;
using System.Text;
using ServiceStack.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CryptoAccouting
{
    public static class ZaifAPI
    {

        // RestSharpでAPIにアクセスしservicestack.textでパース


		internal static void FetchTransaction()
		{
			var result = JsonObject.Parse(json).Object("rxtermsProperties")
		.ConvertTo(x => new RxTerm
		{
			BrandName = x.Get("brandName"),
			DisplayName = x.Get("displayName"),
			Synonym = x.Get("synonym"),
			FullName = x.Get("fullName"),
			FullGenericName = x.Get("fullGenericName"),
			Strength = x.Get("strength"),
			RxTermDoseForm = x.Get("rxtermsDoseForm"),
			Route = x.Get("route"),
			RxCUI = x.Get("rxcui"),
			RxNormDoseForm = x.Get("rxnormDoseForm"),
		});
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

    }


}
