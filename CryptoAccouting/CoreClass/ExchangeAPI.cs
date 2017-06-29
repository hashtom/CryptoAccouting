using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CryptoAccouting
{
    public class ExchangeAPI
    {
        private ZaifAPI zaif;

        public void Login(EnuExchangeType exType){
            
			switch (exType)
			{
				case EnuExchangeType.Zaif:

                    var xmldoc = File.ReadAllText("TestData/apikey.xml");
					//Console.WriteLine(xmldoc);

                    //var doc = XElement.Parse("TestData/apikey.xml").Descendants("broker").Where(n => n.Attribute("name").Value == "Zaif");
                    var doc = XElement.Parse(xmldoc).Descendants("broker").Where(n => n.Attribute("name").Value == "Zaif");
					string _key = doc.Descendants("key").Select(x => x.Value).First();
					string _secret = doc.Descendants("secret").Select(x => x.Value).First();
                    zaif = new ZaifAPI(_key, _secret);
                    break;
				default:
					break;
			}
		}

        public string FetchPrice(EnuExchangeType exType){



            switch (exType) {
                case EnuExchangeType.Zaif :
                    return zaif.FetchPrice(); // Test
                default:
                    return "null";
            } 
        }
        internal static void FetchPosition(EnuExchangeType exc){
            
        }
        internal static void FetchTransaction(EnuExchangeType exc){
            
        }

    }

}
