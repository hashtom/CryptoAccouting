﻿using System;
using System.Collections.Generic;
using System.Linq;
//using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using CoinBalance.CoreModel;

namespace CoinBalance.CoreAPI
{
    public static class ParseAPIStrings
    {

        public static InstrumentList ParseInstrumentListJson(string rawjson)
        {
            InstrumentList instrumentlist = new InstrumentList();
            try
            {
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
            catch (Exception e)
            {
                throw new AppCoreParseException("Exception during parsing InstrumentList Json: " + e.Message);
            }
        }


        public static void ParsePriceSourceXML(string rawxml, InstrumentList instrumentlist)
        {
            try
            {
                var instrumentsXE = XElement.Parse(rawxml).Descendants("instrument");
                //instrumentlist.Clear();

                foreach (var elem in instrumentsXE)
                {
                    var coin = instrumentlist.GetByInstrumentId(elem.Attribute("id").Value);
                    coin.PriceSourceCode = elem.Element("source").Value;
                }
            }
            catch (Exception e)
            {
                throw new AppCoreParseException("Exception during parsing price source XML: " + e.Message);
            }
        }

       
        public static async Task<List<CrossRate>> ParseCrossRateJsonAsync(string rawjson_today, string rawjson_yesterday)
        {
            List<CrossRate> crossrates = new List<CrossRate>();
            JObject json;

            try
            {
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
            catch (Exception e)
            {
                throw new AppCoreParseException("Exception during parsing crossrate json: " + e.Message);
            }

        }


        //public static async Task ParseCoinMarketCapJsonAsync(string rawjson, string rawcoinjson, string rawjson_yesterday, Instrument coin, CrossRate crossrate)
        //{
            
        //    try
        //    {
        //        var btctoken = await Task.Run(() => JArray.Parse(rawjson).SelectToken("[?(@.id == 'bitcoin')]"));
        //        var jtoken = await Task.Run(() => JArray.Parse(rawcoinjson).First());

        //        if (jtoken != null)
        //        {
        //            var jarray_yesterday = await Task.Run(() => JArray.Parse(rawjson_yesterday));
        //            var jtoken_yesterday = jarray_yesterday.SelectToken("[?(@.id == '" + coin.Id + "')]");

        //            //Parse Market Data 
        //            if (coin.MarketPrice == null)
        //            {
        //                var p = new Price(coin);
        //                coin.MarketPrice = p;
        //            }

        //            coin.MarketPrice.LatestPriceBTC = (double)jtoken["price_btc"];
        //            coin.MarketPrice.LatestPriceUSD = (double)jtoken["price_usd"];
        //            coin.MarketPrice.PriceSource = "coinmarketcap";
        //            coin.MarketPrice.DayVolume = btctoken != null ? (double)jtoken["24h_volume_usd"] / (double)btctoken["price_usd"] : 0;
        //            coin.MarketPrice.MarketCap = (string)jtoken["market_cap_usd"] != null ? (double)jtoken["market_cap_usd"] : 0;

        //            if (jtoken_yesterday != null)
        //            {
        //                coin.MarketPrice.PriceBTCBefore24h = (double)jtoken_yesterday["price_btc"];
        //                coin.MarketPrice.PriceUSDBefore24h = (double)jtoken_yesterday["price_usd"];
        //            }
        //        }
        //        else
        //        {
        //            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " :ParseCoinMarketCapJsonAsync: Failed to update " + coin.Name + " price.");
        //            //throw new AppCoreParseException("Failed to update " + coin.Name + " price.");
        //        }

        //        coin.MarketPrice.PriceDate = DateTime.Now;
        //        coin.MarketPrice.USDCrossRate = crossrate;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new AppCoreParseException("Exception during parsing coinmarketcap Json: " + e.Message);
        //    }
        //}


        //public static async Task ParseCoinMarketCapJsonAsync(string rawjson, string rawjson_yesterday, InstrumentList instrumentlist, CrossRate crossrate)
        //{

        //    try
        //    {
        //        var jarray = await Task.Run(() => JArray.Parse(rawjson));
        //        var jarray_yesterday = await Task.Run(() => JArray.Parse(rawjson_yesterday));
        //        var jtoken_bitcoin = jarray.SelectToken("[?(@.id == 'bitcoin')]");

        //        foreach (var coin in instrumentlist) //.Where(x => x.PriceSourceCode == "coinmarketcap" || x.PriceSourceCode is null))
        //        {
                    
        //            var jtoken = jarray.SelectToken("[?(@.id == '" + coin.Id + "')]");
        //            var jtoken_yesterday = jarray_yesterday.SelectToken("[?(@.id == '" + coin.Id + "')]");

        //            if (jtoken != null)
        //            {
        //                if (coin.MarketPrice == null)
        //                {
        //                    var p = new Price(coin);
        //                    coin.MarketPrice = p;
        //                }

        //                coin.MarketPrice.LatestPriceBTC = (double)jtoken["price_btc"];
        //                coin.MarketPrice.LatestPriceUSD = (double)jtoken["price_usd"];
        //                coin.MarketPrice.PriceSource = "coinmarketcap";
        //                coin.MarketPrice.DayVolume = jtoken_bitcoin != null ? (double)jtoken["24h_volume_usd"] / (double)jtoken_bitcoin["price_usd"] : 0;
        //                coin.MarketPrice.MarketCap = (string)jtoken["market_cap_usd"] != null ? (double)jtoken["market_cap_usd"] : 0;

        //                if (jtoken_yesterday != null)
        //                {
        //                    coin.MarketPrice.PriceBTCBefore24h = (double)jtoken_yesterday["price_btc"];
        //                    coin.MarketPrice.PriceUSDBefore24h = (double)jtoken_yesterday["price_usd"];
        //                }

        //                coin.MarketPrice.PriceDate = DateTime.Now;
        //                coin.MarketPrice.USDCrossRate = crossrate;
        //                coin.rank = (int)jtoken["rank"];
        //            }
        //            else
        //            {
        //                coin.rank = int.MaxValue;
        //                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " :ParseCoinMarketCapJsonAsync: Failed to update " + coin.Name + " price.");
        //                //throw new AppCoreParseException("Failed to update " + coin.Name + " price.");
        //            }
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        throw new AppCoreParseException("Exception during parsing coinmarketcap Json: " + e.Message);
        //    }
        //}

        public static Balance ParseBalanceXML(string balanceXML, InstrumentList instrumentlist)
        {

            if (balanceXML is null)
            {
                return null;
            }
            else
            {
                Balance mybal = new Balance();

                try
                {
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

                                var tradedexchange = elem.Element("exchange").Value != "" ? AppCore.GetExchange(elem.Element("exchange").Value) : null;
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
                                    Volume_Previous = elem.Element("volume") == null ? 0 : double.Parse(elem.Element("volume").Value),
                                    WatchOnly = watchonly
                                };

                                if (!watchonly)
                                {
                                    var storagecode = elem.Element("storage").Value;

                                    if (storagecode != "" && Enum.TryParse(elem.Element("storagetype").Value, out EnuCoinStorageType storagetype))
                                    {
                                        AppCore.AttachCoinStorage(storagecode, storagetype, pos);
                                        //var storage = ApplicationCore.GetCoinStorage(storagecode, storagetype);
                                        //pos.AttachCoinStorage(storage);
                                        //storage.AttachPosition(pos);
                                    }
                                    else
                                    {
                                        //var tba = CoinStorageList.GetStorageListSelection().First(x => x.StorageType == EnuCoinStorageType.TBA);
                                        AppCore.AttachCoinStorage(EnuCoinStorageType.TBA.ToString(), EnuCoinStorageType.TBA, pos);
                                    }
                                }

                                mybal.Attach(pos);
                            }
                        }
                    }

                    return mybal;
                }
                catch (Exception e)
                {
                    throw new AppCoreParseException("Exception during parsing balance XML: " + e.Message);
                }
            }
        }

        public static ExchangeList ParseExchangeListJson(string rawjson) //, ExchangeList exlist)
        {
            ExchangeList exchangelist = new ExchangeList();
            JObject json;

            if (AppCore.InstrumentList is null) throw new AppCoreParseException("InstrumentList is null.");

            try
            {
                json = JObject.Parse(rawjson);

                foreach (var market in (JArray)json["exchanges"])
                {
                    var exchange = exchangelist.GetExchange((string)market["code"]);
                    if(exchange is null)
                    {
                        exchange = new Exchange((string)market["code"], EnuCoinStorageType.Exchange);
                        exchangelist.Attach(exchange);
                    }
                    exchange.Name = (string)market["name"];

                    var listing = (JArray)market["listing"];

                    if (listing.ToList().Count() == 0)
                    {
                        AppCore.InstrumentList.ToList().ForEach(x => exchange.AttachListedCoin(x));
                    }
                    else
                    {
                        foreach (var symbol in listing)
                        {
                            Instrument coin = null;
                            if (symbol["symbol"] != null)
                            {
                                coin = AppCore.InstrumentList.GetBySymbol1((string)symbol["symbol"]);
                                if (coin != null)
                                    exchange.AttachSymbolMap(coin.Id, (string)symbol["symbol"], EnuSymbolMapType.Symbol1);
                            }
                            else if (symbol["symbol2"] != null)
                            {
                                coin = AppCore.InstrumentList.GetBySymbol2((string)symbol["symbol2"]);
                                if (coin != null)
                                    if (coin != null) exchange.AttachSymbolMap(coin.Id, (string)symbol["symbol2"], EnuSymbolMapType.Symbol2);
                            }

                            if (coin != null) exchange.AttachListedCoin(coin);
                        }
                    }
                    exchange.HasPriceAPI = (bool)market["apiprice"];
                    exchange.HasTradeAPI = (bool)market["apitrade"];
                    exchange.HasBalanceAPI = (bool)market["apibalance"];
                    exchange.CanCalcPL = (bool)market["calcPL"];
                }

            }
            catch (Exception e)
            {
                throw new AppCoreParseException("Exception during parsing Exchangelist Json: " + e.Message);
            }

            exchangelist.Sort();
            return exchangelist;
        }
    }
}
