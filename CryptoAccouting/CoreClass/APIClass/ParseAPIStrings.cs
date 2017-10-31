using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class ParseAPIStrings
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
            }
        }

        public static async Task<List<CrossRate>> ParseCrossRateJsonAsync(string rawjson_today, string rawjson_yesterday)
        {
            List<CrossRate> crossrates = new List<CrossRate>();
            JObject json;

            json = await Task.Run(() => JObject.Parse(rawjson_today));
            foreach (var ccy in (JArray)json["list"]["resources"])
            {
                EnuBaseFiatCCY baseccy;
                var cursymbol = (string)ccy["resource"]["fields"]["symbol"];

                if (!Enum.TryParse(cursymbol.Replace("=X", ""), out baseccy))
                    continue;

                var crossrate = new CrossRate(baseccy, (double)ccy["resource"]["fields"]["price"], DateTime.Now.Date);
                crossrates.Add(crossrate);
            }

            json = await Task.Run(() => JObject.Parse(rawjson_yesterday));
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

        public static async Task<EnuAPIStatus> ParseCoinMarketCapJsonAsync(string rawjson, string rawjson_yesterday, InstrumentList instrumentlist, CrossRate crossrate)
        {

            var jarray = await Task.Run(() => JArray.Parse(rawjson));
            var jarray_yesterday = await Task.Run(() => JArray.Parse(rawjson_yesterday));

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
                    coin.MarketPrice.LatestPriceBTC = (double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["price_btc"];
                    coin.MarketPrice.LatestPriceUSD = (double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["price_usd"];
                    coin.MarketPrice.PriceSource = "coinmarketcap";
                    coin.MarketPrice.DayVolume = 0; //(double)jarray.SelectToken("[?(@.id == '" + coin.Id + "')]")["24h_volume_usd"] / coin.MarketPrice.LatestPriceBTC;
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

        public static Balance ParseBalanceXML(string balanceXML, InstrumentList instrumentlist)
        {

            if (balanceXML is null)
            {
                return null;
            }
            else
            {
                Balance mybal = new Balance();
                var mybalXE = XElement.Parse(balanceXML);
                var myPositionsXE = mybalXE.Descendants("position");

                var pricedate = mybalXE.Element("balance").Attribute("pricedate");
                mybal.PriceDateTime = pricedate != null ? DateTime.Parse(pricedate.Value) : DateTime.MinValue;

                foreach (var elem in myPositionsXE)
                {
                    if (elem.Element("instrument") != null)
                    {
                        Instrument coin;
                        if (instrumentlist.Any(i => i.Id == elem.Element("instrument").Value))
                        {
                            coin = instrumentlist.First(i => i.Id == elem.Element("instrument").Value);
                            var tradedexchange = ApplicationCore.GetExchange(elem.Element("exchange").Value);
                            var watchonly = elem.Element("watchonly") == null ? false : bool.Parse(elem.Element("watchonly").Value);

                            var pos = new Position(coin)
                            {
                                Id = int.Parse(elem.Attribute("id").Value),
                                Amount = double.Parse(elem.Element("amount").Value),
                                AmountBTC_Previous = elem.Element("amountbtc") == null ? 0 : double.Parse(elem.Element("amountbtc").Value),
                                //BookPriceUSD = elem.Element("book") == null ? 0 : double.Parse(elem.Element("book").Value),
                                BalanceDate = DateTime.Parse(elem.Element("date").Value),
                                BookedExchange = tradedexchange, //(EnuExchangeType)Enum.Parse(typeof(EnuExchangeType), elem.Descendants("exchange").Select(x => x.Value).First())
                                PriceUSD_Previous = elem.Element("priceusd") == null ? 0 : double.Parse(elem.Element("priceusd").Value),
                                PriceBTC_Previous = elem.Element("pricebtc") == null ? 0 : double.Parse(elem.Element("pricebtc").Value),
                                PriceBase_Previous = elem.Element("pricebase") == null ? 0 : double.Parse(elem.Element("pricebase").Value),
                                USDRet1d_Previous = elem.Element("usdret1d") == null ? 0 : double.Parse(elem.Element("usdret1d").Value),
                                BTCRet1d_Previous = elem.Element("btcret1d") == null ? 0 : double.Parse(elem.Element("btcret1d").Value),
                                BaseRet1d_Previous = elem.Element("baseret1d") == null ? 0 : double.Parse(elem.Element("baseret1d").Value),
                                WatchOnly = watchonly
                            };

                            if (!watchonly)
                            {
                                var storagecode = elem.Element("storage").Value;

                                if (storagecode != "" && Enum.TryParse(elem.Element("storagetype").Value, out EnuCoinStorageType storagetype))
                                {
                                    ApplicationCore.AttachCoinStorage(storagecode, storagetype, pos);
                                    //var storage = ApplicationCore.GetCoinStorage(storagecode, storagetype);
                                    //pos.AttachCoinStorage(storage);
                                    //storage.AttachPosition(pos);
                                }
                            }

                            mybal.Attach(pos);
                        }
                    }
                }

                return mybal;
            }
        }

        public static EnuAPIStatus ParseExchangeListJson(string rawjson, ExchangeList exlist)
        {
            JObject json;

            try
            {
                json = JObject.Parse(rawjson);

                foreach (var market in (JArray)json["exchanges"])
                {

                    var exchange = exlist.GetExchange((string)market["code"]);
                    exchange.Name = (string)market["name"];

                    var listing = (JArray)market["listing"];

                    if (listing.ToList().Count() == 0)
                    {
                        ApplicationCore.InstrumentList.ToList().ForEach(x => exchange.AttachListedCoin(x));
                    }
                    else
                    {
                        foreach (var symbol in listing)
                        {
                            Instrument coin = null;
                            if (symbol["symbol"] != null)
                            {
                                coin = ApplicationCore.InstrumentList.GetBySymbol1((string)symbol["symbol"]);
                                if (coin != null)
                                    exchange.AttachSymbolMap(coin.Id, (string)symbol["symbol"], EnuSymbolMapType.Symbol1);
                            }
                            else if (symbol["symbol2"] != null)
                            {
                                coin = ApplicationCore.InstrumentList.GetBySymbol2((string)symbol["symbol2"]);
                                if (coin != null)
                                    if (coin != null) exchange.AttachSymbolMap(coin.Id, (string)symbol["symbol2"], EnuSymbolMapType.Symbol2);
                            }

                            if (coin != null) exchange.AttachListedCoin(coin);
                        }
                    }
                    exchange.APIProvided = (bool)market["api"];
                }

                return EnuAPIStatus.Success;
            }
            catch (JsonException)
            {
                return EnuAPIStatus.FatalError;
            }
        }
    }
}
