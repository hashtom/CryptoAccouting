using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class ExchangeAPI
    {

        //public static async Task<Price> FetchBTCPriceAsyncTest(Exchange exchange, string apikey, string apisecret){

        //    var coin = ApplicationCore.GetInstrument("BTC");
        //    var p = new Price(coin);

        //    p.SourceCurrency = EnuCCY.JPY;
        //    coin.MarketPrice = p;

        //    switch (exchange.ExchangeType) {

        //        case EnuExchangeType.Zaif :

        //            if (p.Coin.Symbol != "BTC") return null; // will add exception statement here

        //            //GetAPIKey(EnuExchangeType.Zaif);
        //            var rawjson = await ZaifAPI.FetchPriceAsync(exchange.Key, exchange.Secret);
        //            var json = JObject.Parse(rawjson);

        //            p.LatestPriceBTC = (double)json.SelectToken("$.last");
        //            p.DayVolume = (int)json.SelectToken("$.volume");
        //            p.PriceSource = "zaif";
        //            p.PriceDate = DateTime.Now;
        //            //p.UpdateTime = DateTime.Now;

        //            return p;

        //        default:
        //            return null;
        //    }

        //}


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

        internal static async Task<TradeList> FetchTradeListAsync(Exchange exchange, string calendarYear = null, bool isAggregateDaily = true)
		{
			string rawjson;
            string filename = exchange.Name + "Transaction_" + calendarYear + ".json";

            switch (exchange.Code)
			{
                case "Zaif":

                    rawjson = StorageAPI.LoadFromFile(filename);
                    if (rawjson is null || calendarYear == DateTime.Now.Year.ToString() || calendarYear is null)
                    {
                        rawjson = await ZaifAPI.FetchTransactionAsync(exchange.Key, exchange.Secret, calendarYear);
                    }

                    var tradelist = ZaifAPI.ParseTrade(rawjson);
                    if (tradelist != null) StorageAPI.SaveFile(rawjson, filename);

                    return tradelist;

				default:
					return null;
			}

		}

        internal static async Task<List<Position>> FetchPositionAsync(Exchange exchange)
        {
            string rawjson;
            string filename = exchange.Name + "Position" + ".json";

            switch (exchange.Code)
            {
                case "Zaif":

                    rawjson = await ZaifAPI.FetchPositionAsync(exchange.Key, exchange.Secret);


                    var positions = ZaifAPI.ParsePosition(rawjson, exchange);
                    if (positions != null) StorageAPI.SaveFile(rawjson, filename);

                    return positions;

                default:
                    return null;
            }

        }

    }

}
