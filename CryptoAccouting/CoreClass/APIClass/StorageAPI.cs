﻿using System;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class StorageAPI
    {

        public static void SaveJsonFile(string json, string fileName)
        {

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documents, fileName);
            File.WriteAllText(path, json);

        }


        public static string LoadFromJsonFile(string fileName)
        {

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documents, fileName);
            return File.ReadAllText(path);
        }

        public static Balance LoadBalanceXML(string fileName, List<Instrument> instruments)
        {
            Balance mybal;

			//try
			//{
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(documents, fileName);

            if (File.Exists(path))
            {
                var balanceXML = File.ReadAllText(path);

                var mybalXE = XElement.Parse(balanceXML).Descendants("position");
                mybal = new Balance();

                foreach (var elem in mybalXE)
                {
                    Instrument coin;
                    if (instruments.Where(i => i.Symbol == elem.Element("symbol").Value).Any())
                    {
                        coin = instruments.Where(i => i.Symbol == elem.Element("symbol").Value).First();

                        EnuExchangeType tradedexchange;
                        if(!Enum.TryParse(elem.Element("exchange").Value, out tradedexchange))
                            tradedexchange = EnuExchangeType.NotSelected;

                        var pos = new Position(coin, EnuPositionType.Detail)
                        {
                            Id = int.Parse(elem.Attribute("id").Value),
                            Amount = double.Parse(elem.Element("amount").Value),
                            BookPrice = double.Parse(elem.Element("book").Value),
                            BalanceDate = DateTime.Parse(elem.Element("date").Value),
                            TradedExchange = tradedexchange //(EnuExchangeType)Enum.Parse(typeof(EnuExchangeType), elem.Descendants("exchange").Select(x => x.Value).First())
                        };
                        mybal.AttachPosition(pos, false);
                    }

                }

            }
            else
            {
                mybal = new Balance();
            }
            //}catch(IOException e){
            //    Console.WriteLine(e.ToString());
            //}catch(Exception e){
            //    Console.WriteLine(e.ToString());
            //}finally{
            //    mybal = new Balance();
            //}

            mybal.RecalculatePositionSummary();
			return mybal;

        }

		public static void SaveBalanceXML(Balance myBalance, string fileName)
		{
			var mydocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(mydocuments, fileName);

			XElement application = new XElement("application",
												new XAttribute("name", ApplicationCore.AppName));
            XElement balance = new XElement("balance");
											//,new XAttribute("exchange", myBalance.ExchangeTraded.ToString()));
			application.Add(balance);

            foreach (var pos in myBalance.positions)
			{
                XElement position = new XElement("position",
                                                 new XAttribute("id", pos.Id.ToString()),
                                                 new XElement("symbol", pos.Coin.Symbol),
                                                 new XElement("date", pos.BalanceDate),
                                                 new XElement("amount", pos.Amount.ToString()),
                                                 new XElement("book", pos.BookPrice.ToString()),
                                                 new XElement("exchange", pos.TradedExchange.ToString())
                                                );
				balance.Add(position);
			}

			File.WriteAllText(path, application.ToString());

		}

        public static void SaveInstrumentXML(List<Instrument> myinstruments, string fileName)
        {
			var mydocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(mydocuments, fileName);

            if (File.Exists(path)) File.Delete(path);

			XElement application = new XElement("application",
												new XAttribute("name", ApplicationCore.AppName));
            XElement instruments = new XElement("instruments");
			application.Add(instruments);

            foreach (var coin in myinstruments)
			{
                XElement instrument = new XElement("instrument",
                                                          new XAttribute("id", coin.Id),
                                                          new XElement("symbol", coin.Symbol),
                                                          new XElement("name", coin.Name),
                                                          new XElement("type", coin.Type.ToString()),
                                                          //new XElement("logofile", coin.LogoFileName),
                                                          new XElement("isactive", coin.IsActive.ToString())
                                                  );
				instruments.Add(instrument);
			}

			File.WriteAllText(path, application.ToString());
        }

        public static List<Instrument> LoadInstrumentXML(string fileName)
		{
            List<Instrument> instruments;
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(documents, fileName);

            if (File.Exists(path))
            {
                var instrumentsXML = File.ReadAllText(path);

                var instrumentsXE = XElement.Parse(instrumentsXML).Descendants("instrument");
                instruments = new List<Instrument>();

                foreach (var elem in instrumentsXE)
                {
                    var coin = new Instrument(elem.Attribute("id").Value,
                                              elem.Element("symbol").Value,
                                              elem.Element("name").Value);
                    //if (elem.Descendants("logofile").Select(x => x.Value).Any())
                    //{
                    //    coin.LogoFileName = (string)elem.Descendants("logofile").Select(x => x.Value).First();
                    //}
                    coin.IsActive = bool.Parse((string)elem.Descendants("isactive").Select(x => x.Value).First());
                    instruments.Add(coin);
                }

                return instruments;
            }
            else
            {
                return null;
            }

		}

        public static List<Instrument> LoadMarketDataXML(string fileName)
        {
            List<Instrument> instruments = new List<Instrument>();

            return instruments;
        }

        public static void SaveMarketDataXML(string fileName)
        {

        }

        public static EnuAppStatus LoadAppSettingXML(string fileName)
        {
            EnuCCY baseccy;
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(documents, fileName);

            if (File.Exists(path))
            {
                var xmldoc = File.ReadAllText(path);

                if (!Enum.TryParse(XElement.Parse(xmldoc).Element("basecurrency").Value, out baseccy))
                    baseccy = EnuCCY.USD;
                
                var apikeysXE = XElement.Parse(xmldoc).Descendants("exchange");

                foreach (var elem in apikeysXE)
                {
                    EnuExchangeType extype;
                    if (!Enum.TryParse((string)elem.Attribute("name").Value, out extype))
                    {
                        extype = EnuExchangeType.NotSelected;
                    }
                    else
                    {
                        var exchange = ApplicationCore.GetExchange(extype);
                        exchange.Key = elem.Element("key").Value;
                        exchange.Secret = elem.Element("secret").Value;
                    }
                }

                ApplicationCore.BaseCurrency = baseccy;

                return EnuAppStatus.Success;

            }else{
                
                return EnuAppStatus.FailureStorage;
            }


        }

        public static EnuAppStatus SaveAppSettingXML(string fileName, string AppName, EnuCCY BaseCurrency, ExchangeList exList)
		{
			var mydocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(mydocuments, fileName);

			XElement application = new XElement("application",
												new XAttribute("name", AppName));
			

            XElement basecurrency = new XElement("basecurrency", BaseCurrency);
			application.Add(basecurrency);

            XElement apikeys = new XElement("apikeys");
            application.Add(apikeys); 

            foreach (var exchange in exList)
			{
                XElement key = new XElement("exchange",
                                            new XAttribute("name", exchange.ExchangeType),
                                            new XElement("key", exchange.Key),
                                            new XElement("secret", exchange.Secret)
                                           );
				apikeys.Add(key);
			}

			File.WriteAllText(path, application.ToString());

            return EnuAppStatus.Success; //todo

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
