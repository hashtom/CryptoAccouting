using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CryptoAccouting
{
    public class ExchangeAPI
    {
        private HttpClient http;
        private string apikey, apisecret;

        public async Task<Price> FetchPriceAsync(EnuExchangeType exType){

			var p = new Price(new Instrument() { Symbol = "BTC" });
			p.BaseCurrency = "JPY";

            switch (exType) {

                case EnuExchangeType.Zaif :

                    if (p.Coin.Symbol != "BTC") return null; // will add exception statement here

					getAPIKey(EnuExchangeType.Zaif);
                    var json = await ZaifAPI.FetchPriceAsync(http, apikey, apisecret);

					p.LatestPrice = (double)json.SelectToken("$.last");
					p.DayVolume = (int)json.SelectToken("$.volume");
					p.PriceSource = "zaif";
					p.PriceDate = DateTime.Now;
					p.UpdateTime = DateTime.Now;

                    //var price = (string)json.SelectToken("$.asks[0][0]");
                    //var amount = (string)json.SelectToken("$.asks[0][1]");
                    return p;

				default:
                    return null;
			} 
        }
        internal void FetchPosition(EnuExchangeType exType){


        }

        internal async Task<Transactions> FetchTransaction(EnuExchangeType exType){

            Transactions txs = new Transactions();

			switch (exType)
			{
				case EnuExchangeType.Zaif:
					getAPIKey(EnuExchangeType.Zaif);
					var json = await ZaifAPI.FetchTransaction(http, apikey, apisecret);

                    int status = (int)json.SelectToken("$.success");
					
                    //int cnt = 0;

                    foreach (JProperty x in (JToken)json["return"])
                    {
                        //cnt++;
                        var tx = new Transaction(new Instrument() { Symbol = "BTC" }, new Exchange(EnuExchangeType.Zaif));
                        tx.txid = x.Name;
                        tx.BuySell = (string)json["return"][tx.txid]["action"];
                        tx.Amount = (double)json["return"][tx.txid]["amount"];
                        tx.TradePrice = (double)json["return"][tx.txid]["price"];
                        tx.TradeDate = ZaifAPI.FromEpochSeconds(tx.TradeDate,(long)json["return"][tx.txid]["timestamp"]);
                        txs.AttachTransaction(tx);

                        //if (cnt == 50) break;
                    }

                    return txs;

				default:

                    return null;
			}
        }


		private void getAPIKey(EnuExchangeType exType)
		{

			http = new HttpClient();

			switch (exType)
			{
				case EnuExchangeType.Zaif:

					var xmldoc = File.ReadAllText("TestData/apikey.xml");
					//Console.WriteLine(xmldoc);

					//var doc = XElement.Parse("TestData/apikey.xml").Descendants("broker").Where(n => n.Attribute("name").Value == "Zaif");
					var doc = XElement.Parse(xmldoc).Descendants("broker").Where(n => n.Attribute("name").Value == "Zaif");
					apikey = doc.Descendants("key").Select(x => x.Value).First();
					apisecret = doc.Descendants("secret").Select(x => x.Value).First();
					break;
				default:
					break;
			}
		}

    }

}
