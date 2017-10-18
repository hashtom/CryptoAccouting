﻿using System;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class StorageAPI
    {

        public static string LoadBundleFile(string fileName)
        {
            return File.ReadAllText("Bundlefile/" + fileName);
        }

        public static EnuAPIStatus SaveFile(string json, string fileName)
        {
            try
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var path = Path.Combine(documents, fileName);
                File.WriteAllText(path, json);
            }
            catch (IOException)
            {
                return EnuAPIStatus.FailureStorage;
            }

            return EnuAPIStatus.Success;
        }

        public static string LoadFromFile(string fileName)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documents, fileName);

            try
            {
                if (!File.Exists(path))
                {
                    return null;
                }
                else
                {
                    return File.ReadAllText(path);
                }
            }
            catch (IOException)
            {
                return null;
            }
        }

        public static Balance LoadBalanceXML(string fileName, InstrumentList instrumentlist)
        {
            Balance mybal = new Balance();

            var balanceXML = LoadFromFile(fileName);
            if (balanceXML == null) balanceXML = LoadBundleFile("BalanceData.xml");

            //try
            //{
            //var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //var path = Path.Combine(documents, fileName);

            if (balanceXML != null)
            {
                var mybalXE = XElement.Parse(balanceXML).Descendants("position");

                foreach (var elem in mybalXE)
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
            }

            //else
            //{
            //    mybal = new Balance();
            //}
            //}catch(IOException e){
            //    Console.WriteLine(e.ToString());
            //}catch(Exception e){
            //    Console.WriteLine(e.ToString());
            //}finally{
            //    mybal = new Balance();
            //}

            return mybal;

        }

        public static EnuAPIStatus SaveBalanceXML(Balance myBalance, string fileName)
        {
            //var mydocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //var path = Path.Combine(mydocuments, fileName);

            XElement application = new XElement("application",
                                                new XAttribute("name", ApplicationCore.AppName));
            XElement balance = new XElement("balance");
            //,new XAttribute("exchange", myBalance.ExchangeTraded.ToString()));
            application.Add(balance);

            foreach (var pos in myBalance)
            {
                XElement position = new XElement("position",
                                                 new XAttribute("id", pos.Id.ToString()),
                                                 new XElement("instrument", pos.Coin.Id),
                                                 new XElement("date", pos.BalanceDate),
                                                 new XElement("amount", pos.Amount.ToString()),
                                                 new XElement("amountbtc", pos.LatestAmountBTC.ToString()),
                                                 new XElement("book", "NA"), //pos.BookPriceUSD.ToString()),
                                                 new XElement("exchange", pos.BookedExchange == null ? "" : pos.BookedExchange.Code),
                                                 new XElement("storage", pos.CoinStorage == null ? "" : pos.CoinStorage.Code),
                                                 new XElement("storagetype", pos.CoinStorage == null ? "" : pos.CoinStorage.StorageType.ToString()),
                                                 new XElement("priceusd", pos.LatestPriceUSD.ToString()),
                                                 new XElement("pricebtc", pos.LatestPriceBTC().ToString()),
                                                 new XElement("pricebase", pos.LatestPriceBase().ToString()),
                                                 new XElement("watchonly", pos.WatchOnly.ToString())
                                                );
                balance.Add(position);
            }

            //File.WriteAllText(path, application.ToString());

            return SaveFile(application.ToString(), fileName);

        }

        public static EnuAPIStatus SavePriceSourceXML(InstrumentList instrumentlist, string fileName)
        {
            //var mydocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //var path = Path.Combine(mydocuments, fileName);

            XElement application = new XElement("application",
                                                new XAttribute("name", ApplicationCore.AppName));
            XElement instruments = new XElement("instruments");
            application.Add(instruments);

            foreach (var coin in instrumentlist.Where(x => x.PriceSourceCode != null))
            {
                XElement instrument = new XElement("instrument",
                                                   new XAttribute("id", coin.Id),
                                                   //new XElement("symbol", coin.Symbol1),
                                                   //new XElement("symbol2", coin.Symbol2),
                                                   //new XElement("name", coin.Name),
                                                   //new XElement("type", coin.Type.ToString()),
                                                   new XElement("source", coin.PriceSourceCode)
                                                  //new XElement("rank", coin.rank)
                                                  //new XElement("logofile", coin.LogoFileName),
                                                  //new XElement("isactive", coin.IsActive.ToString())
                                                  );
                instruments.Add(instrument);
            }

            //File.WriteAllText(path, application.ToString());
            return SaveFile(application.ToString(), fileName);
        }

        public static InstrumentList LoadInstrument()
        {
            const string instrumentlistFile = ApplicationCore.InstrumentListFile;
            const string pricesorceFile = ApplicationCore.PriceSourceFile;
            InstrumentList instrumentlist;

            var rawjson = LoadFromFile(instrumentlistFile);
            if (rawjson is null) rawjson = LoadBundleFile(instrumentlistFile);

            try
            {
                instrumentlist = ParseMarketData.ParseInstrumentListJson(rawjson);
            }
            catch (Exception)
            {
                return null;
            }

            var PriceSourceXML = LoadFromFile(pricesorceFile);
            if (PriceSourceXML != null)
            {
                try
                {
                    ParseMarketData.ParsePriceSourceXML(pricesorceFile, instrumentlist);
                }
                catch (Exception)
                {
                    Console.WriteLine("Parse error: ParsePriceSourceXML");
                }
            }

            return instrumentlist;
        }

        public static List<CrossRate> LoadCrossRate()
        {
            const string jsonfilename_today = ApplicationCore.CrossRatefile_today;
            const string jsonfilename_yesterday = ApplicationCore.CrossRatefile_yesterday;
            string rawjson_today, rawjson_yesterday;

            rawjson_today = StorageAPI.LoadFromFile(jsonfilename_today);
            rawjson_yesterday = StorageAPI.LoadFromFile(jsonfilename_yesterday);

            if (rawjson_today != null & rawjson_yesterday != null)
            {
                return ParseMarketData.ParseCrossRateJson(rawjson_today, rawjson_yesterday);
            }
            else
            {
                return null;
            }
        }

        public static EnuAPIStatus LoadAppSettingXML(string fileName)
        {
            EnuBaseFiatCCY baseccy;
            var xmldoc = LoadFromFile(fileName);

            if (xmldoc != null)
            {
                //var xmldoc = File.ReadAllText(path);

                if (!Enum.TryParse(XElement.Parse(xmldoc).Element("basecurrency").Value, out baseccy))
                    baseccy = EnuBaseFiatCCY.USD;

                var apikeysXE = XElement.Parse(xmldoc).Descendants("exchange");

                foreach (var elem in apikeysXE)
                {
                    //EnuExchangeType extype;
                    //if (!Enum.TryParse((string)elem.Attribute("name").Value, out extype))
                    //{
                    //    extype = EnuExchangeType.NotSelected;
                    //}
                    //else
                    //{
                    var exchange = ApplicationCore.GetExchange((string)elem.Attribute("name").Value);
                    exchange.Key = elem.Element("key").Value;
                    exchange.Secret = elem.Element("secret").Value;
                    //}
                }

                ApplicationCore.BaseCurrency = baseccy;

                return EnuAPIStatus.Success;

            }
            else
            {
                return EnuAPIStatus.FailureStorage;
            }


        }

        public static EnuAPIStatus SaveAppSettingXML(string fileName, string AppName, EnuBaseFiatCCY BaseCurrency, ExchangeList exList)
        {
            //var mydocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //var path = Path.Combine(mydocuments, fileName);

            XElement application = new XElement("application",
                                                new XAttribute("name", AppName));


            XElement basecurrency = new XElement("basecurrency", BaseCurrency);
            application.Add(basecurrency);

            XElement apikeys = new XElement("apikeys");
            application.Add(apikeys);

            foreach (var exchange in exList.Where(x => x.APIReady == true))
            {
                XElement key = new XElement("exchange",
                                            new XAttribute("name", exchange.Code),
                                            new XElement("key", exchange.Key),
                                            new XElement("secret", exchange.Secret)
                                           );
                apikeys.Add(key);
            }

            //File.WriteAllText(path, application.ToString());

            return SaveFile(application.ToString(), fileName);

        }

        public static EnuAPIStatus RemoveFile(string filename)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            try
            {
                File.Delete(Path.Combine(documents, filename));
            }
            catch (IOException)
            {
                return EnuAPIStatus.FailureStorage;
            }

            return EnuAPIStatus.Success;
        }
        public static EnuAPIStatus RemoveAllCache()
        {
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //Don't delete balance data
            try
            {
                //AppSetting
                File.Delete(Path.Combine(documents, "AppSetting.xml"));

                //json file cache
                foreach (var file in Directory.EnumerateFiles(documents, "*.json"))
                {
                    File.Delete(file);
                }

                //Excel files
                foreach (var file in Directory.EnumerateFiles(documents, "*.xlsx"))
                {
                    File.Delete(file);
                }

                //png data
                foreach (var image in Directory.EnumerateFiles(Path.Combine(documents, "Images")))
                {
                    File.Delete(image);
                }
            }
            catch(IOException)
            {
                return EnuAPIStatus.FailureStorage;
            }

            return EnuAPIStatus.Success;
        }

  //      public static EnuAppStatus SaveExchangeListXMLTemp(ExchangeList exList, string fileName)
		//{
		//	var mydocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		//	var path = Path.Combine(mydocuments, fileName);

		//	XElement application = new XElement("application",
  //                                              new XAttribute("name", ApplicationCore.AppName));

		//	XElement exchanges = new XElement("exchanges");
		//	application.Add(exchanges);

  //          foreach (var ex in exList)
		//	{
  //              var coins = new XElement("coins");
  //              foreach (var coin in ex.ListedCoin)
  //              {
  //                  var listing = new XElement("listing",
  //                                                  new XAttribute("symbol", coin.Symbol));
  //                  coins.Add(listing);
  //              }

		//		XElement exchange = new XElement("exchange",
  //                                          new XAttribute("type", ex.ExchangeType),
  //                                          new XElement("name", ex.ExchangeName),
  //                                          coins
		//								   );

  //              exchanges.Add(exchange);
		//	}

		//	File.WriteAllText(path, application.ToString());

		//	return EnuAppStatus.Success;

		//}

    }
}
