using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAccouting.CoreClass.APIClass
{
    public static class ExchangeAPI
    {
        const string ExchangeListfile = "ExchangeList.json";

        public static EnuAPIStatus FetchExchangeList(ExchangeList exlist)
        {
            string rawjson;
            string BaseUri = "http://coinbalance.jpn.org/ExchangeList.json";

            if (!Reachability.IsHostReachable(BaseUri))
            {
                rawjson = StorageAPI.LoadFromFile(ExchangeListfile);
                if (rawjson == null) rawjson = StorageAPI.LoadBundleFile(ExchangeListfile);
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
                    rawjson = StorageAPI.LoadFromFile(ExchangeListfile);
                    if (rawjson == null) rawjson = StorageAPI.LoadBundleFile(ExchangeListfile);
                }
            }

            if (ParseAPIStrings.ParseExchangeListJson(rawjson, exlist) != EnuAPIStatus.Success)
            {
                return ParseAPIStrings.ParseExchangeListJson(StorageAPI.LoadBundleFile(ExchangeListfile), exlist);
            }
            else
            {
                return StorageAPI.SaveFile(rawjson, ExchangeListfile);
            }

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
