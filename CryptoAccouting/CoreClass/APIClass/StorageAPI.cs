﻿using System;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class StorageAPI
    {

        public static EnuAppStatus SaveJsonFile(string json, string fileName)
        {

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documents, fileName);
            File.WriteAllText(path, json);

            return EnuAppStatus.Success;
        }


        public static string LoadFromJsonFile(string fileName)
        {

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documents, fileName);
            return File.ReadAllText(path);
        }

        public static Balance LoadBalanceXML(string fileName, InstrumentList coins)
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
                    if (coins.Where(i => i.Symbol == elem.Element("symbol").Value).Any())
                    {
                        coin = coins.Where(i => i.Symbol == elem.Element("symbol").Value).First();
                        var tradedexchange = ApplicationCore.GetExchange(elem.Element("exchange").Value);

                        var pos = new Position(coin)
                        {
                            Id = int.Parse(elem.Attribute("id").Value),
                            Amount = double.Parse(elem.Element("amount").Value),
                            BookPriceUSD = double.Parse(elem.Element("book").Value),
                            BalanceDate = DateTime.Parse(elem.Element("date").Value),
                            BookedExchange = tradedexchange //(EnuExchangeType)Enum.Parse(typeof(EnuExchangeType), elem.Descendants("exchange").Select(x => x.Value).First())
                        };

                        EnuCoinStorageType storagetype;
						var storagecode = elem.Element("storage").Value;
                        if (storagecode != "" && Enum.TryParse(elem.Element("storagetype").Value, out storagetype))
                        {
                            var storage = mybal.GetCoinStorage(storagecode, storagetype);
                            if (storage != null)
                            {
                                pos.AttachCoinStorage(storage);
                            }
                            else
                            {
                                pos.AttachNewStorage(storagecode, storagetype);
                            }
                        }

                        mybal.Attach(pos);
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

			return mybal;

        }

        public static EnuAppStatus SaveBalanceXML(Balance myBalance, string fileName)
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
                                                 new XElement("book", pos.BookPriceUSD.ToString()),
                                                 new XElement("exchange", pos.BookedExchange == null ? "" : pos.BookedExchange.Code),
                                                 new XElement("storage", pos.CoinStorage == null ? "" : pos.CoinStorage.Code),
                                                 new XElement("storagetype", pos.CoinStorage == null ? "" : pos.CoinStorage.StorageType.ToString())
                                                );
				balance.Add(position);
			}

			File.WriteAllText(path, application.ToString());

            return EnuAppStatus.Success;

		}

        public static EnuAppStatus SaveInstrumentXML(InstrumentList instrumentlist, string fileName)
        {
			var mydocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(mydocuments, fileName);

			XElement application = new XElement("application",
												new XAttribute("name", ApplicationCore.AppName));
            XElement instruments = new XElement("instruments");
			application.Add(instruments);

            foreach (var coin in instrumentlist)
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

            return EnuAppStatus.Success;
        }

        public static InstrumentList LoadInstrumentXML(string fileName)
		{
            InstrumentList instruments;
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(documents, fileName);

            if (File.Exists(path))
            {
                var instrumentsXML = File.ReadAllText(path);

                var instrumentsXE = XElement.Parse(instrumentsXML).Descendants("instrument");
                instruments = new InstrumentList();

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
                    instruments.Attach(coin);
                }

                return instruments;
            }
            else
            {
                return null;
            }

		}

        public static InstrumentList LoadMarketDataXML(string fileName)
        {
            var instruments = new InstrumentList();

            //todo

            return instruments;
        }

        public static EnuAppStatus SaveMarketDataXML(string fileName)
        {

            return EnuAppStatus.Success;
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

            foreach (var exchange in exList.Where(x => x.APIReady == true))
            {
                XElement key = new XElement("exchange",
                                            new XAttribute("name", exchange.Code),
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
