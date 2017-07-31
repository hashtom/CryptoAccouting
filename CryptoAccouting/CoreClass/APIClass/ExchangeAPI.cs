using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class ExchangeAPI
    {
        private static string apikey, apisecret;

        public static async Task<Price> FetchBTCPriceAsyncTest(EnuExchangeType exType){

            var coin = ApplicationCore.GetInstrument("BTC");
            var p = new Price(coin);

            p.SourceCurrency = EnuCCY.JPY;
            coin.MarketPrice = p;

            switch (exType) {

                case EnuExchangeType.Zaif :

                    if (p.Coin.Symbol != "BTC") return null; // will add exception statement here

                    GetAPIKey(EnuExchangeType.Zaif);
                    var rawjson = await ZaifAPI.FetchPriceAsync(apikey, apisecret);
                    var json = JObject.Parse(rawjson);

                    p.LatestPriceBTC = (double)json.SelectToken("$.last");
                    p.DayVolume = (int)json.SelectToken("$.volume");
                    p.PriceSource = "zaif";
                    p.PriceDate = DateTime.Now;
                    //p.UpdateTime = DateTime.Now;

                    return p;

                default:
                    return null;
            }

        }

        internal static void FetchPosition(EnuExchangeType exType){


        }

		//     internal async Task<Transactions> FetchTransaction(EnuExchangeType exType, bool isAggregateDaily = true){

		//         Transactions txs = new Transactions();

		//switch (exType)
		//{
		//    case EnuExchangeType.Zaif:
		//        getAPIKey(EnuExchangeType.Zaif);
		//        var json = await ZaifAPI.FetchTransaction(http, apikey, apisecret);

		//                 int status = (int)json.SelectToken("$.success");

		//                 foreach (JProperty x in (JToken)json["return"])
		//                 {
		//                     var tx = new Transaction(new Instrument() { Symbol = "BTC" }, EnuExchangeType.Zaif);
		//                     tx.TxId = x.Name;
		//                     tx.BuySell = (string)json["return"][x.Name]["action"] == "bid" ? EnuBuySell.Buy : EnuBuySell.Sell;
		//                     tx.Amount = (double)json["return"][tx.TxId]["amount"];
		//                     tx.TradePrice = (double)json["return"][tx.TxId]["price"];
		//                     tx.TradeDate = ZaifAPI.FromEpochSeconds(tx.TradeDate,(long)json["return"][tx.TxId]["timestamp"]);
		//                     txs.AttachTransaction(tx);
		//                 }

		//                 break;;

		//    default:

		//                 break;
		//}
		//    return txs;

		//}

		internal static async Task<TradeList> FetchTradeListAsync(EnuExchangeType exType, bool isAggregateDaily = true, bool ReadFromFile = false)
		{

            TradeList tradelist = new TradeList(ApplicationCore.BaseCurrency);
			string rawjson;

			switch (exType)
			{
				case EnuExchangeType.Zaif:

					if (ReadFromFile)
					{
						rawjson = StorageAPI.LoadFromJsonFile("zaifTransaction.json");
					}
					else
					{
						GetAPIKey(EnuExchangeType.Zaif);
						rawjson = await ZaifAPI.FetchTransactionAsync(apikey, apisecret);

					}

					var json = JObject.Parse(rawjson);
					int status = (int)json.SelectToken("$.success");

					foreach (JProperty x in (JToken)json["return"])
					{
						//Transaction Date Order must be ascending by design...
						EnuBuySell ebuysell;

						switch ((string)json["return"][x.Name]["your_action"])
						{
							case "bid":
								ebuysell = EnuBuySell.Buy;
								break;
							case "ask":
								ebuysell = EnuBuySell.Sell;
								break;
							default:
								ebuysell = EnuBuySell.Check;
								break;
						}


						var symbol = (string)json["return"][x.Name]["currency_pair"];
						symbol = symbol.Replace("_jpy", "").Replace("_btc", "").ToUpper();

						tradelist.AggregateTransaction(ApplicationCore.GetInstrument(symbol),
												 exType,
												 ebuysell,
												 (double)json["return"][x.Name]["amount"],
												 (double)json["return"][x.Name]["price"],
												 ApplicationCore.FromEpochSeconds((long)json["return"][x.Name]["timestamp"]).Date,
												 (int)json["return"][x.Name]["fee"]);
                        

					}

					// Save Json file
					StorageAPI.SaveJsonFile(rawjson, "zaifTransaction.json");
					break;

				default:
					break;
			}

			return tradelist;
		}

        //internal static async Task<List<TradeList>> FetchTradeListsAsync(EnuExchangeType exType, bool isAggregateDaily = true, bool ReadFromFile = false)
        //{

        //    var tradelists = new List<TradeList>();

        //    string rawjson;

        //    switch (exType)
        //    {
        //        case EnuExchangeType.Zaif:
                    
        //            //tl = new TradeList(ApplicationCore.BaseCurrency, "BTC");

        //            if (ReadFromFile)
        //            {
        //                rawjson = StorageAPI.LoadFromJsonFile("zaifTransaction.json");
        //            }
        //            else
        //            {
        //                GetAPIKey(EnuExchangeType.Zaif);
        //                rawjson = await ZaifAPI.FetchTransactionAsync(apikey, apisecret);
    
        //            }

        //            var json = JObject.Parse(rawjson);
        //            int status = (int)json.SelectToken("$.success");

        //            foreach (JProperty x in (JToken)json["return"])
        //            {
        //               // DateTime tm = DateTime.Now;

        //                //Transaction Date Order must be ascending by design...
        //                EnuBuySell ebuysell;

        //                switch ((string)json["return"][x.Name]["your_action"])
        //                {   
        //                    case "bid":
        //                        ebuysell = EnuBuySell.Buy;
        //                        break;
        //                    case "ask":
        //                        ebuysell = EnuBuySell.Sell;
        //                        break;
        //                    default:
        //                        ebuysell = EnuBuySell.Check;
        //                        break;
        //                }


        //                var symbol = (string)json["return"][x.Name]["currency_pair"];
        //                symbol = symbol.Replace("_jpy","").Replace("_btc","").ToUpper();

        //                var tl = tradelists.Any(t => t.TradedCoin.Symbol == symbol) ?
        //                                   tradelists.First(t => t.TradedCoin.Symbol == symbol) :
        //                                   new TradeList(ApplicationCore.BaseCurrency, symbol);


        //                tl.AggregateTransaction(ApplicationCore.GetInstrument(symbol),
        //                                         exType,
        //                                         ebuysell,
        //                                         (double)json["return"][x.Name]["amount"],
        //                                         (double)json["return"][x.Name]["price"],
        //                                         ApplicationCore.FromEpochSeconds((long)json["return"][x.Name]["timestamp"]).Date,
        //                                         (int)json["return"][x.Name]["fee"]);

        //                if (!tradelists.Any(t => t.TradedCoin.Symbol == symbol))
        //                    tradelists.Add(tl);

        //            }

        //            // Save Json file
        //            StorageAPI.SaveJsonFile(rawjson, "zaifTransaction.json");
        //            break;

        //        default:

        //            break;
        //    }

        //    return tradelists;
        //}


        internal static void GetAPIKey(EnuExchangeType exType)
        {

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
