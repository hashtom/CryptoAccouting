﻿using System;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinBalance.CoreClass.APIClass
{
    public static class StorageAPI
    {
        const string PriceSourceFile = "pricesource.xml";
        const string BalanceFile = "mybalance.xml";
        const string BalanceBundleFile = "BalanceData.xml";
        const string AppSettingFile = "AppSetting.xml";

        public static string LoadBundleFile(string fileName)
        {
            try
            {
                return File.ReadAllText("Bundlefile/" + fileName);
            }
            catch (IOException e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": LoadBundleFile: " + e.GetType() + ": " + e.Message);
                throw new AppCoreStorageException(e.GetType() + ": " + e.Message);
            }

        }

        public static void SaveFile(string json, string fileName)
        {
            try
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var path = Path.Combine(documents, fileName);
                File.WriteAllText(path, json);
            }
            catch (IOException e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": SaveFile: " + e.GetType() + ": " + e.Message);
                throw new AppCoreStorageException(e.GetType() + ": " + e.Message);
            }

        }

        public static string LoadFromFile(string fileName)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documents, fileName);

            try
            {
                if (!File.Exists(path))
                {
                    throw new AppCoreStorageException("File not found: " + fileName);
                }
                else
                {
                    return File.ReadAllText(path);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": LoadFromFile: " + e.GetType() + ": " + e.Message);
                throw new AppCoreStorageException(e.GetType() + ": " + e.Message);
            }
        }

        public static Balance LoadBalanceXML(InstrumentList instrumentlist)
        {
            //const string BalanceFile = ApplicationCore.BalanceFile;
            //const string BalanceBundleFile = ApplicationCore.BalanceBundleFile;
            string balanceXML;

            if (instrumentlist is null) throw new AppCoreBalanceException("InstrumentList is null");

            try
            {
                balanceXML = LoadFromFile(BalanceFile);
            }
            catch (Exception e)
            {
                balanceXML = LoadBundleFile(BalanceBundleFile);
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": LoadBalanceXML(continued with bundlefile): " + e.GetType() + ": " + e.Message);
            }

            try
            {
                return ParseAPIStrings.ParseBalanceXML(balanceXML, instrumentlist);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": LoadBalanceXML(process terminated): " + e.GetType() + ": " + e.Message);
                throw new AppCoreBalanceException(e.GetType() + ": " + e.Message);
            }

        }

        public static void SaveBalanceXML(Balance myBalance)
        {

            XElement application = new XElement("application", new XAttribute("name", AppCore.AppName));
            XElement balance = new XElement("balance", new XAttribute("pricedate", myBalance.PriceDateTime));
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
                                                 new XElement("pricebtc", pos.LatestPriceBTC.ToString()),
                                                 new XElement("pricebase", pos.LatestPriceBase.ToString()),
                                                 new XElement("usdret1d", pos.USDRet1d.ToString()),
                                                 new XElement("btcret1d", pos.BTCRet1d.ToString()),
                                                 new XElement("baseret1d", pos.BaseRet1d.ToString()),
                                                 new XElement("volume", pos.MarketDayVolume.ToString()),
                                                 new XElement("watchonly", pos.WatchOnly.ToString())
                                                );
                balance.Add(position);
            }

            SaveFile(application.ToString(), BalanceFile);

        }

        public static void SavePriceSourceXML(InstrumentList instrumentlist)
        {

            XElement application = new XElement("application", new XAttribute("name", AppCore.AppName));
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

            SaveFile(application.ToString(), PriceSourceFile);
        }

        public static InstrumentList LoadInstrument()
        {
            InstrumentList instrumentlist;
            string rawjson;

            try
            {
                rawjson = LoadFromFile(MarketDataAPI.InstrumentListFile);
            }
            catch (Exception e)
            {
                rawjson = LoadBundleFile(MarketDataAPI.InstrumentListFile);
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": LoadInstrument(continued with bundlefile): " + e.GetType() + ": " + e.Message);
            }

            try
            {
                instrumentlist = ParseAPIStrings.ParseInstrumentListJson(rawjson);

                try
                {
                    LoadPriceSource(instrumentlist);
                    return instrumentlist;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": LoadInstrument: Error on loading price Source: " + ex.GetType() + ": " + ex.Message);
                    return instrumentlist;
                }
            }
            catch (AppCoreParseException e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": LoadInstrument: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        public static ExchangeList LoadExchangeList()
        {
            string rawjson;
            try
            {
                rawjson = StorageAPI.LoadFromFile(MarketDataAPI.ExchangeListFile);
            }
            catch (Exception e)
            {
                rawjson = LoadBundleFile(MarketDataAPI.ExchangeListFile);
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": LoadExchangeList(continued with bundlefile): " + e.GetType() + ": " + e.Message);
            }

            try
            {
                return ParseAPIStrings.ParseExchangeListJson(rawjson);
            }
            catch (AppCoreInstrumentException e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchExchangeList(Process terminated): " + e.GetType() + ": " + e.Message);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchExchangeList(Fatal Error): " + e.GetType() + ": " + e.Message);
                throw new AppCoreExchangeException(e.GetType() + ": " + e.Message);
            }

        }

        public static void LoadPriceSource(InstrumentList instrumentlist)
        {
            try
            {
                ParseAPIStrings.ParsePriceSourceXML(LoadFromFile(PriceSourceFile), instrumentlist);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": LoadPriceSource: " + e.GetType() + ": " + e.Message);
            }
        }

        public static async Task<List<CrossRate>> LoadCrossRateAsync()
        {
            const string jsonfilename_today = MarketDataAPI.CrossRatefile_today;
            const string jsonfilename_yesterday = MarketDataAPI.CrossRatefile_yesterday;
            string rawjson_today, rawjson_yesterday;

            try
            {
                rawjson_today = LoadFromFile(jsonfilename_today);
                rawjson_yesterday = LoadFromFile(jsonfilename_yesterday);
                return await ParseAPIStrings.ParseCrossRateJsonAsync(rawjson_today, rawjson_yesterday);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": LoadCrossRateAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        public static void LoadAppSettingXML()
        {
            try
            {
                var xmldoc = LoadFromFile(AppSettingFile);
                EnuBaseFiatCCY baseccy;
                if (!Enum.TryParse(XElement.Parse(xmldoc).Element("basecurrency").Value, out baseccy))
                    baseccy = EnuBaseFiatCCY.USD;

                var apikeysXE = XElement.Parse(xmldoc).Descendants("exchange");

                foreach (var elem in apikeysXE)
                {
                    var exchange = AppCore.GetExchange((string)elem.Attribute("name").Value);
                    exchange.Key = elem.Element("key") != null ? elem.Element("key").Value : "";
                    exchange.Secret = elem.Element("secret") != null ? elem.Element("secret").Value : "";
                    exchange.CustomerID = elem.Element("customer") != null ? elem.Element("customer").Value : "";
                }

                AppCore.BaseCurrency = baseccy;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": LoadAppSettingXML: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static void SaveAppSettingXML(string AppName, EnuBaseFiatCCY BaseCurrency, ExchangeList exList)
        {
            XElement application = new XElement("application",
                                                new XAttribute("name", AppName));


            XElement basecurrency = new XElement("basecurrency", BaseCurrency);
            application.Add(basecurrency);

            XElement apikeys = new XElement("apikeys");
            application.Add(apikeys);

            foreach (var exchange in exList.Where(x => x.APIKeySaved() == true))
            {
                XElement key = new XElement("exchange",
                                            new XAttribute("name", exchange.Code),
                                            new XElement("key", exchange.Key),
                                            new XElement("secret", exchange.Secret),
                                            new XElement("customer", exchange.CustomerID)
                                           );
                apikeys.Add(key);
            }

            SaveFile(application.ToString(), AppSettingFile);
        }

        public static void RemoveFile(string filename)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            try
            {
                File.Delete(Path.Combine(documents, filename));
            }
            catch (IOException e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": RemoveFile: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static void RemoveAllCache()
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
            catch(IOException e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": RemoveAllCache: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

    }
}
