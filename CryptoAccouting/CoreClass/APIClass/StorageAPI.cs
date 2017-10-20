﻿using System;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class StorageAPI
    {
        const string PriceSourceFile = "pricesource.xml";
        const string BalanceFile = "mybalance.xml";
        const string BalanceBundleFile = "BalanceData.xml";
        const string AppSettingFile = "AppSetting.xml";

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

        public static Balance LoadBalanceXML(InstrumentList instrumentlist)
        {
            //const string BalanceFile = ApplicationCore.BalanceFile;
            //const string BalanceBundleFile = ApplicationCore.BalanceBundleFile;

            var balanceXML = LoadFromFile(BalanceFile);
            if (balanceXML == null) balanceXML = LoadBundleFile(BalanceBundleFile);

            return ParseAPIStrings.ParseBalanceXML(balanceXML, instrumentlist);

        }

        public static EnuAPIStatus SaveBalanceXML(Balance myBalance)
        {

            XElement application = new XElement("application",
                                                new XAttribute("name", ApplicationCore.AppName));
            XElement balance = new XElement("balance", 
                                            new XAttribute("pricedate", myBalance.PriceDateTime));
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
                                                 new XElement("usdret1d", pos.USDRet1d().ToString()),
                                                 new XElement("btcret1d", pos.BTCRet1d().ToString()),
                                                 new XElement("baseret1d", pos.BaseRet1d().ToString()),
                                                 new XElement("watchonly", pos.WatchOnly.ToString())
                                                );
                balance.Add(position);
            }

            return SaveFile(application.ToString(), BalanceFile);

        }

        public static EnuAPIStatus SavePriceSourceXML(InstrumentList instrumentlist)
        {

            XElement application = new XElement("application",
                                                new XAttribute("name", ApplicationCore.AppName));
            XElement instruments = new XElement("instruments");
            application.Add(instruments);

            foreach (var coin in instrumentlist.Where(x => x.PriceSourceCode != null && x.PriceSourceCode != "coinmarketcap"))
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

            return SaveFile(application.ToString(), PriceSourceFile);
        }

        public static InstrumentList LoadInstrument()
        {
            const string InstrumentListFile = MarketDataAPI.InstrumentListFile;
            InstrumentList instrumentlist;

            var rawjson = LoadFromFile(InstrumentListFile);
            if (rawjson is null) rawjson = LoadBundleFile(InstrumentListFile);

            try
            {
                instrumentlist = ParseAPIStrings.ParseInstrumentListJson(rawjson);
            }
            catch (Exception)
            {
                return null;
            }

            var PriceSourceXML = LoadFromFile(PriceSourceFile);
            if (PriceSourceXML != null)
            {
                try
                {
                    ParseAPIStrings.ParsePriceSourceXML(PriceSourceXML, instrumentlist);
                }
                catch (Exception)
                {
                    Console.WriteLine("Parse error: ParsePriceSourceXML");
                }
            }

            return instrumentlist;
        }

        public static async Task<List<CrossRate>> LoadCrossRateAsync()
        {
            const string jsonfilename_today = MarketDataAPI.CrossRatefile_today;
            const string jsonfilename_yesterday = MarketDataAPI.CrossRatefile_yesterday;
            string rawjson_today, rawjson_yesterday;

            rawjson_today = LoadFromFile(jsonfilename_today);
            rawjson_yesterday = LoadFromFile(jsonfilename_yesterday);

            if (rawjson_today != null & rawjson_yesterday != null)
            {
                return await ParseAPIStrings.ParseCrossRateJsonAsync(rawjson_today, rawjson_yesterday);
            }
            else
            {
                return null;
            }
        }

        public static EnuAPIStatus LoadAppSettingXML()
        {
            EnuBaseFiatCCY baseccy;
            var xmldoc = LoadFromFile(AppSettingFile);

            if (xmldoc != null)
            {
                if (!Enum.TryParse(XElement.Parse(xmldoc).Element("basecurrency").Value, out baseccy))
                    baseccy = EnuBaseFiatCCY.USD;

                var apikeysXE = XElement.Parse(xmldoc).Descendants("exchange");

                foreach (var elem in apikeysXE)
                {
                    var exchange = ApplicationCore.GetExchange((string)elem.Attribute("name").Value);
                    exchange.Key = elem.Element("key").Value;
                    exchange.Secret = elem.Element("secret").Value;
                }

                ApplicationCore.BaseCurrency = baseccy;

                return EnuAPIStatus.Success;

            }
            else
            {
                return EnuAPIStatus.FailureStorage;
            }


        }

        public static EnuAPIStatus SaveAppSettingXML(string AppName, EnuBaseFiatCCY BaseCurrency, ExchangeList exList)
        {
            XElement application = new XElement("application",
                                                new XAttribute("name", AppName));


            XElement basecurrency = new XElement("basecurrency", BaseCurrency);
            application.Add(basecurrency);

            XElement apikeys = new XElement("apikeys");
            application.Add(apikeys);

            foreach (var exchange in exList.Where(x => x.APIProvided == true))
            {
                XElement key = new XElement("exchange",
                                            new XAttribute("name", exchange.Code),
                                            new XElement("key", exchange.Key),
                                            new XElement("secret", exchange.Secret)
                                           );
                apikeys.Add(key);
            }

            return SaveFile(application.ToString(), AppSettingFile);
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
                File.Delete(Path.Combine(documents, AppSettingFile));
                File.Delete(Path.Combine(documents, PriceSourceFile));

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

    }
}
