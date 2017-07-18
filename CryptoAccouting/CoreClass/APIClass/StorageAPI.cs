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

            //LoadInstruments(false);

            var mybalXE = XElement.Parse(balanceXML).Descendants("position");
            var mybal = new Balance(EnuExchangeType.Zaif);

            foreach (var elem in mybalXE)
            {
                Instrument coin;
                if (instruments.Where(i => i.Symbol == elem.Descendants("symbol").Select(x => x.Value).First()).Any())
                {
                    coin = instruments.Where(i => i.Symbol == elem.Descendants("symbol").Select(x => x.Value).First()).First();
                    var pos = new Position(coin, (string)elem.Attribute("id").Value) { Amount = double.Parse(elem.Descendants("amount").Select(x => x.Value).First()) };
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
			XElement balance = new XElement("balance",
											new XAttribute("exchange", myBalance.ExchangeTraded.ToString()));
			application.Add(balance);

			foreach (var pos in myBalance)
			{
				XElement position = new XElement("position",
												 new XAttribute("id", pos.Id.ToString()),
												 new XElement("symbol", pos.Coin.Symbol),
												 new XElement("date", pos.BalanceDate),
												 new XElement("amount", pos.Amount.ToString()),
												 new XElement("book", pos.BookPrice.ToString())
												);
				balance.Add(position);
			}

			File.WriteAllText(path, application.ToString());

		}
    }
}
