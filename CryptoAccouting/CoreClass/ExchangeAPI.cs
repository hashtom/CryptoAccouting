using System;
using Newtonsoft.Json;

namespace CryptoAccouting
{
    public static class ExchangeAPI
    {

        internal static void FetchPrice(ExchangeType exc){
            
        }
        internal static void FetchPosition(ExchangeType exc){
            
        }
        internal static void FetchTransaction(ExchangeType exc){
            
        }

    }

    public enum ExchangeType
    {
        Zaif = 1,
		BitFlyer = 2,
		Kraken = 3,
        CoinCheck = 4,
        BitBank = 5
	}
}
