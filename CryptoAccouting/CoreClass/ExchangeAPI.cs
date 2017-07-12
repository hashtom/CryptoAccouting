using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CryptoAccouting.CoreClass
{
    public class ExchangeAPI
    {
        private HttpClient http;
        private string apikey, apisecret;

        public async Task<Price> FetchBTCPriceAsyncTest(EnuExchangeType exType){

            var coin = ApplicationCore.GetInstrument("BTC");
            var p = new Price(coin);

            p.BaseCurrency = EnuBaseCCY.JPY;
            coin.PriceValue = p;

            switch (exType) {

                case EnuExchangeType.Zaif :

                    if (p.Coin.Symbol != "BTC") return null; // will add exception statement here

                    GetAPIKey(EnuExchangeType.Zaif);
                    var rawjson = await ZaifAPI.FetchPriceAsync(http, apikey, apisecret);
                    var json = JObject.Parse(rawjson);

                    p.LatestPrice = (double)json.SelectToken("$.last");
                    p.DayVolume = (int)json.SelectToken("$.volume");
                    p.PriceSource = "zaif";
                    p.PriceDate = DateTime.Now;
                    //p.UpdateTime = DateTime.Now;

                    return p;

                default:
                    return null;
            }

        }
        internal void FetchPosition(EnuExchangeType exType){


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

        internal async Task<Transactions> FetchTransactionAsync(EnuExchangeType exType, bool isAggregateDaily = true, bool ReadFromFile = false)
        {

            Transactions txs = new Transactions();
            string rawjson;

            switch (exType)
            {
                case EnuExchangeType.Zaif:
                    

                    if (ReadFromFile)
                    {
                        rawjson = LoadFromJsonFile("zaifTransaction.json");
                    }
                    else
                    {
                        GetAPIKey(EnuExchangeType.Zaif);
                        rawjson = await ZaifAPI.FetchTransactionAsync(http, apikey, apisecret);
    
                    }

                    var json = JObject.Parse(rawjson);
                    int status = (int)json.SelectToken("$.success");

                    foreach (JProperty x in (JToken)json["return"])
                    {
                        DateTime tm = DateTime.Now;

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

                        txs.AggregateTransaction(ApplicationCore.GetInstrument("BTC"),
                                                 exType,
                                                 ebuysell,
                                                 (double)json["return"][x.Name]["amount"],
                                                 (double)json["return"][x.Name]["price"],
                                                 ZaifAPI.FromEpochSeconds(tm, (long)json["return"][x.Name]["timestamp"]).Date,
                                                 (int)json["return"][x.Name]["fee"]);
                    }

                    // Save Json file
                    SaveJsonFile(rawjson, "zaifTransaction.json");

                    break;

                default:

                    break;
            }

            return txs;
        }

        private void SaveJsonFile(string json, string fileName){

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documents, fileName);
            File.WriteAllText(path, json);

        }


        private string LoadFromJsonFile(string fileName)
        {

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documents, fileName);
            return File.ReadAllText(path);
        }

        private void GetAPIKey(EnuExchangeType exType)
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
