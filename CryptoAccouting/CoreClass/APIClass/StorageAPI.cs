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
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documents, fileName);
            var balanceXML = File.ReadAllText(path);

            var mybalXE = XElement.Parse(balanceXML).Descendants("position");
            var mybal = new Balance();

            foreach (var elem in mybalXE)
            {
                Instrument coin;
                if (instruments.Where(i => i.Symbol == elem.Descendants("symbol").Select(x => x.Value).First()).Any())
                {
                    coin = instruments.Where(i => i.Symbol == elem.Descendants("symbol").Select(x => x.Value).First()).First();
                    var pos = new Position(coin)
                    {
                        Id = int.Parse(elem.Attribute("id").Value),
                        Amount = double.Parse(elem.Descendants("amount").Select(x => x.Value).First())
                        //TradedExchange = 
                    };
                    mybal.AttachPosition(pos);
                }

            }

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

			foreach (var pos in myBalance)
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
                                                          new XElement("logofile", coin.LogoFileName),
                                                          new XElement("isactive", coin.IsActive.ToString())
                                                  );
				instruments.Add(instrument);
			}

			File.WriteAllText(path, application.ToString());
        }

        public static List<Instrument> LoadInstrumentXML(string fileName)
		{
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(documents, fileName);
			var balanceXML = File.ReadAllText(path);

			var mybalXE = XElement.Parse(balanceXML).Descendants("instrument");
			var instruments = new List<Instrument>();

            foreach (var elem in mybalXE)
            {
                var coin = new Instrument((string)elem.Attribute("id").Value,
                                          (string)elem.Descendants("symbol").Select(x => x.Value).First(),
                                          (string)elem.Descendants("name").Select(x => x.Value).First());
                if (elem.Descendants("logofile").Select(x => x.Value).Any())
                {
                    coin.LogoFileName = (string)elem.Descendants("logofile").Select(x => x.Value).First();
                }
                coin.IsActive = bool.Parse((string)elem.Descendants("isactive").Select(x => x.Value).First());
                instruments.Add(coin);
            }

			return instruments;

		}
    }
}
