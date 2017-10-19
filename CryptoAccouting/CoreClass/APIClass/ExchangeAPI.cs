using System;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class ExchangeAPI
    {
        
        public static EnuAPIStatus FetchExchangeList(ExchangeList exlist)
        {
            const string jsonfilename = "ExchangeList.json";
            JObject json;
            string rawjson;
            string BaseUri = "http://coinbalance.jpn.org/ExchangeList.json";

            if (!Reachability.IsHostReachable(BaseUri))
            {
                rawjson = StorageAPI.LoadFromFile(jsonfilename);
                if (rawjson == null) rawjson = StorageAPI.LoadBundleFile(jsonfilename);
            }
            else
            {
                try
                {
                    using (var http = new HttpClient())
                    {
                        HttpResponseMessage response = http.GetAsync(BaseUri).Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            return EnuAPIStatus.FailureNetwork;
                        }
                        rawjson = response.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception)
                {
                    rawjson = StorageAPI.LoadFromFile(jsonfilename);
                    if (rawjson == null) rawjson = StorageAPI.LoadBundleFile(jsonfilename);
                    //return EnuAPIStatus.FailureNetwork;
                }
            }
            try
            {
                json = JObject.Parse(rawjson);
            }
            catch (JsonException)
            {
                return EnuAPIStatus.FatalError;
            }

            foreach (var market in (JArray)json["exchanges"])
            {
                
                var exchange = exlist.GetExchange((string)market["code"]);
                exchange.Name = (string)market["name"];

                var listing = (JArray)market["listing"];

                if (listing.ToList().Count() == 0)
                {
                    ApplicationCore.InstrumentList.ToList().ForEach(x => exchange.AttachListedCoin(x));
                }
                else
                {
                    foreach (var symbol in listing)
                    {
                        Instrument coin = null;
                        if (symbol["symbol"] != null)
                        {
                            coin = ApplicationCore.InstrumentList.GetBySymbol1((string)symbol["symbol"]);
                            if (coin != null)
                                exchange.AttachSymbolMap(coin.Id, (string)symbol["symbol"], EnuSymbolMapType.Symbol1);
                        }
                        else if (symbol["symbol2"] != null)
                        {
                            coin = ApplicationCore.InstrumentList.GetBySymbol2((string)symbol["symbol2"]);
                            if (coin != null)
                                if (coin != null) exchange.AttachSymbolMap(coin.Id, (string)symbol["symbol2"], EnuSymbolMapType.Symbol2);
                        }

                        if (coin != null) exchange.AttachListedCoin(coin);
                    }
                }
                exchange.APIReady = (bool)market["api"];
                //}
            }

            StorageAPI.SaveFile(rawjson, jsonfilename);
            return EnuAPIStatus.Success;
        }

        internal static async Task<TradeList> FetchTradeListAsync(Exchange exchange)
		{
            
            switch (exchange.Code)
			{
                case "Zaif":
                    return await ZaifAPI.FetchTransactionAsync(exchange);

                case "CoinCheck":
                    return await CoinCheckAPI.FetchTransactionAsync(exchange);

                case "Bittrex":
                    return await BittrexAPI.FetchTransactionAsync(exchange);
				
                default:
					return null;
			}

		}

        internal static async Task<List<Position>> FetchPositionAsync(Exchange exchange)
        {
            
            switch (exchange.Code)
            {
                case "Zaif":
                    return await ZaifAPI.FetchPositionAsync(exchange);

                case "CoinCheck":
                    return await CoinCheckAPI.FetchPositionAsync(exchange);
                
                case "Bittrex":
                    return await BittrexAPI.FetchPositionAsync(exchange);

                default:
                    return null;
            }

        }

    }

}
