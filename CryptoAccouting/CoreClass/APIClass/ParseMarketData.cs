using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class ParseMarketData
    {

        public static InstrumentList ParseInstrumentListJson(string rawjson)
        {
            InstrumentList instrumentlist = new InstrumentList();
            var jarray = JArray.Parse(rawjson);

            //Parse Market Data 
            foreach (var elem in jarray)
            {
                if ((bool)elem["active"])
                {
                    //Instrument coin = instrumentlist.GetByInstrumentId((string)elem["id"]);
                    //if (coin != null)
                    //{
                    //    //coin = instrumentlist.First(x => x.Id == (string)elem["id"]);
                    //    coin.Symbol1 = (string)elem["symbol"];
                    //    coin.Name = (string)elem["name"];
                    //}
                    //else
                    //{
                       var coin = new Instrument((string)elem["id"])
                        {
                            Symbol1 = (string)elem["symbol"],
                            Name = (string)elem["name"]
                        };//IOTA symbol注意
                    //}

                    if (elem["symbol2"] != null)
                    {
                        coin.Symbol2 = (string)elem["symbol2"];
                    }
                    coin.rank = int.Parse((string)elem["rank"]);
                    instrumentlist.Attach(coin);
                }
            }

            return instrumentlist;
        }

        public static void ParsePriceSourceXML(string rawxml, InstrumentList instrumentlist)
        {
            var instrumentsXE = XElement.Parse(rawxml).Descendants("instrument");
            //instrumentlist.Clear();

            foreach (var elem in instrumentsXE)
            {
                var coin = instrumentlist.GetByInstrumentId(elem.Attribute("id").Value);
                coin.PriceSourceCode = elem.Element("source").Value;
                //var coin = new Instrument(elem.Attribute("id").Value)
                //{
                //    Symbol1 = elem.Element("symbol").Value,
                //    Name = elem.Element("name").Value,
                //    PriceSourceCode = elem.Element("source").Value,
                //    rank = int.Parse(elem.Element("rank").Value)
                //};

                //if (elem.Element("symbol2") != null) coin.Symbol2 = elem.Element("symbol2").Value;
                //if (elem.Descendants("logofile").Select(x => x.Value).Any())
                //{
                //    coin.LogoFileName = (string)elem.Descendants("logofile").Select(x => x.Value).First();
                //}
                //coin.IsActive = bool.Parse((string)elem.Descendants("isactive").Select(x => x.Value).First());
                //instrumentlist.Attach(coin);
            }
        }

        public static List<CrossRate> ParseCrossRateJson(string rawjson_today, string rawjson_yesterday)
        {
            List<CrossRate> crossrates = new List<CrossRate>();
            JObject json;

            json = JObject.Parse(rawjson_today);
            foreach (var ccy in (JArray)json["list"]["resources"])
            {
                EnuBaseFiatCCY baseccy;
                var cursymbol = (string)ccy["resource"]["fields"]["symbol"];

                if (!Enum.TryParse(cursymbol.Replace("=X", ""), out baseccy))
                    continue;

                var crossrate = new CrossRate(baseccy, (double)ccy["resource"]["fields"]["price"], DateTime.Now.Date);
                crossrates.Add(crossrate);
            }

            json = JObject.Parse(rawjson_yesterday);
            foreach (var ccy in (JArray)json["list"]["resources"])
            {
                EnuBaseFiatCCY baseccy;
                var cursymbol = (string)ccy["resource"]["fields"]["symbol"];

                if (!Enum.TryParse(cursymbol.Replace("=X", ""), out baseccy))
                    continue;

                if (crossrates.Any(x => x.Currency == baseccy))
                    crossrates.First(x => x.Currency == baseccy).RateBefore24h = (double)ccy["resource"]["fields"]["price"];

            }

            return crossrates;
        }

        public static EnuAPIStatus ParseCoinMarketCapJson(string rawjson, string rawjson_yesterday, InstrumentList instrumentlist, CrossRate crossrate)
        {
            foreach (var coin in instrumentlist.Where(x => x.PriceSourceCode == "coinmarketcap" || x.PriceSourceCode is null))
            {
                //Parse Market Data 
                if (coin.MarketPrice == null)
                {
                    var p = new Price(coin);
                    coin.MarketPrice = p;
                }

                try
                {
                    var jarray = JArray.Parse(rawjson);
                    var jarray_yesterday = JArray.Parse(rawjson_yesterday);

                    coin.MarketPrice.LatestPriceBTC = (double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["price_btc"];
                    coin.MarketPrice.LatestPriceUSD = (double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["price_usd"];
                    coin.MarketPrice.PriceSource = "coinmarketcap";
                    coin.MarketPrice.DayVolume = (double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["24h_volume_usd"] / coin.MarketPrice.LatestPriceBTC;
                    coin.MarketPrice.MarketCap = (double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["market_cap_usd"];
                    coin.MarketPrice.PriceDate = DateTime.Now;//ApplicationCore.FromEpochSeconds((long)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["last_updated"]);
                    coin.MarketPrice.PriceBTCBefore24h = (double)jarray_yesterday.SelectToken("[?(@.id == '" + coin.Id + "')]")["price_btc"];
                    coin.MarketPrice.PriceUSDBefore24h = (double)jarray_yesterday.SelectToken("[?(@.id == '" + coin.Id + "')]")["price_usd"];
                    coin.MarketPrice.USDCrossRate = crossrate;
                }
                catch (Exception)
                {
                    return EnuAPIStatus.ParseError;
                }

            }

            return EnuAPIStatus.Success;
        }
    }
}
