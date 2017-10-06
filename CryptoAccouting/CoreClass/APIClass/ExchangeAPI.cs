using System;
using System.IO;
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

        internal static async Task<TradeList> FetchTradeListAsync(Exchange exchange, string calendarYear = null, bool isAggregateDaily = true)
		{
			string rawjson;
            string filename = exchange.Name + "Transaction_" + calendarYear + ".json";
            TradeList tradelist;

            switch (exchange.Code)
			{
                case "Zaif":

                    rawjson = StorageAPI.LoadFromFile(filename);
                    if (rawjson is null || calendarYear == DateTime.Now.Year.ToString() || calendarYear is null)
                    {
                        rawjson = await ZaifAPI.FetchTransactionAsync(exchange.Key, exchange.Secret, calendarYear);
                    }

                    tradelist = ZaifAPI.ParseTrade(rawjson);
                    if (tradelist != null) StorageAPI.SaveFile(rawjson, filename);

                    return tradelist;

                case "CoinCheck":

                    rawjson = StorageAPI.LoadFromFile(filename);
                    if (rawjson is null || calendarYear == DateTime.Now.Year.ToString() || calendarYear is null)
                    {
                        rawjson = await CoinCheckAPI.FetchTransactionAsync(exchange.Key, exchange.Secret, calendarYear);
                    }

                    tradelist = CoinCheckAPI.ParseTrade(rawjson);
                    if (tradelist != null) StorageAPI.SaveFile(rawjson, filename);

                    return tradelist;

				default:
					return null;
			}

		}

        internal static async Task<List<Position>> FetchPositionAsync(Exchange exchange)
        {
            string rawjson;
            string filename = exchange.Name + "Position" + ".json";
            List<Position> positions = null;

            switch (exchange.Code)
            {
                case "Zaif":

                    rawjson = await ZaifAPI.FetchPositionAsync(exchange.Key, exchange.Secret);

                    if (rawjson != null)
                    {
                        positions = ZaifAPI.ParsePosition(rawjson, exchange);
                        if (positions != null) StorageAPI.SaveFile(rawjson, filename);
                    }
                    break;

                case "CoinCheck":

                    rawjson = await CoinCheckAPI.FetchPositionAsync(exchange.Key, exchange.Secret);

                    if (rawjson != null)
                    {
                        positions = CoinCheckAPI.ParsePosition(rawjson, exchange);
                        if (positions != null) StorageAPI.SaveFile(rawjson, filename);
                    }
                    break;

                default:
                    break;
            }

            return positions;
        }

    }

}
