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

        public static void FetchExchangeList(ExchangeList exlist)
        {
            string rawjson;
            string BaseUri = "http://coinbalance.jpn.org/ExchangeList.json";

            //if (!Reachability.IsHostReachable(BaseUri))
            //{
            //    rawjson = StorageAPI.LoadFromFile(ExchangeListfile);
            //    if (rawjson == null) rawjson = StorageAPI.LoadBundleFile(ExchangeListfile);
            //}
            //else
            //{
            try
            {
                using (var http = new HttpClient())
                {
                    var res = http.GetAsync(BaseUri).Result;
                    if (!res.IsSuccessStatusCode)
                    {
                        throw new AppCoreNetworkException("http response error. status code: " + res.StatusCode);
                    }
                    else
                    {
                        rawjson = res.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": FetchExchangeList(process continued with file): " + e.GetType() + ": " + e.Message);
                rawjson = StorageAPI.LoadFromFile(ExchangeListfile);
                if (rawjson == null) rawjson = StorageAPI.LoadBundleFile(ExchangeListfile);
            }

            try
            {
                ParseAPIStrings.ParseExchangeListJson(rawjson, exlist);
                StorageAPI.SaveFile(rawjson, ExchangeListfile);
            }
            catch(Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": FetchExchangeList(process continued with file): " + e.GetType() + ": " + e.Message);
                ParseAPIStrings.ParseExchangeListJson(StorageAPI.LoadBundleFile(ExchangeListfile), exlist);
            }

        }

        internal static async Task<TradeList> FetchTradeListAsync(Exchange exchange)
        {

            try
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
                        throw new AppCoreNetworkException("ExchangeCode error");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": FetchTradeListAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        internal static async Task<List<Position>> FetchPositionAsync(Exchange exchange)
        {
            try
            {
                switch (exchange.Code)
                {
                    case "Zaif":
                        return await ZaifAPI.FetchPositionAsync(exchange);

                    case "CoinCheck":
                        return await CoinCheckAPI.FetchPositionAsync(exchange);

                    case "Bittrex":
                        return  await BittrexAPI.FetchPositionAsync(exchange);

                    default:
                        throw new AppCoreNetworkException("ExchangeCode error");
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": FetchPositionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

    }

}
