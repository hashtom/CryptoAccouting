﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CryptoAccouting
{
    public static class ExchangeAPI
    {
        private static HttpClient http;
        private static string apikey, apisecret;

        public static Task<Price> FetchPriceAsync(EnuExchangeType exType, Price p){
            
            switch (exType) {
                case EnuExchangeType.Zaif :
                    getAPIKey(EnuExchangeType.Zaif);
                    return ZaifAPI.FetchPriceAsync(http, apikey, apisecret, p);

                default:
                    return null;
			} 
        }
        internal static void FetchPosition(EnuExchangeType exType){


        }

        internal static void FetchTransaction(EnuExchangeType exType){
            
			switch (exType)
			{
				case EnuExchangeType.Zaif:
					getAPIKey(EnuExchangeType.Zaif);
					ZaifAPI.FetchTransaction(http, apikey, apisecret);
					break;
				default:

					Console.WriteLine("error");
					break;
			}
        }


		private static void getAPIKey(EnuExchangeType exType)
		{

			http = new HttpClient();

			switch (exType)
			{
				case EnuExchangeType.Zaif:

					var xmldoc = File.ReadAllText("TestData/apikey.xml");
					//Console.WriteLine(xmldoc);

					//var doc = XElement.Parse("TestData/apikey.xml").Descendants("broker").Where(n => n.Attribute("name").Value == "Zaif");
					var doc = XElement.Parse(xmldoc).Descendants("broker").Where(n => n.Attribute("name").Value == "Zaif");
					apikey = doc.Descendants("key").Select(x => x.Value).First();
					apisecret = doc.Descendants("secret").Select(x => x.Value).First();
					break;
				default:
					break;
			}
		}

    }

}
