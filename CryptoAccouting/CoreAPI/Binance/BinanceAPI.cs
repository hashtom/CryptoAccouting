using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using CoinBalance.CoreModel;
using RestSharp;

namespace CoinBalance.CoreAPI
{
    public static class BinanceAPI
    {
        private const string ApiRoot = "https://api.binance.com";
        private static Exchange _binance;
        private static IRestClient _restClient = new RestClient(new Uri(ApiRoot));


        public static async Task FetchPriceAsync(Exchange binance, InstrumentList coins)
        {
            _binance = binance;
            string symbol;
            try
            {

                foreach (var coin in coins.Where(x => x.PriceSourceCode == _binance.Code))
                {
                    if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);

                    if (coin.Id is "bitcoin" || coin.Id is "tether")
                    {
                        symbol = "BTCUSDT";
                    }
                    else
                    {
                        symbol = _binance.GetSymbolForExchange(coin.Id) + "BTC";
                    }
                    var path = $"/api/v1/ticker/24hr?symbol={symbol}";

                    var req = RestUtil.CreateJsonRestRequest(path);
                    var response = await _restClient.ExecuteGetTaskAsync<BinanceTicker>(req);
                    if (response.ErrorException != null)
                    {
                        throw response.ErrorException;
                    }

                    var result = response.Data;

                    if (coin.Id is "bitcoin")
                    {
                        coin.MarketPrice.LatestPriceBTC = 1;
                        coin.MarketPrice.LatestPriceUSD = (double)result.lastPrice;
                        coin.MarketPrice.PriceBTCBefore24h = 1;
                        coin.MarketPrice.PriceUSDBefore24h = (double)result.prevClosePrice;
                        coin.MarketPrice.DayVolume = (double)result.volume;
                        //coin.MarketPrice.PriceDate = result.closeTime;
                    }
                    else if (coin.Id is "tether")
                    {
                        coin.MarketPrice.LatestPriceBTC = 1 / (double)result.lastPrice;
                        coin.MarketPrice.LatestPriceUSD = 1;
                        coin.MarketPrice.PriceBTCBefore24h = 1 / (double)result.prevClosePrice;
                        coin.MarketPrice.PriceUSDBefore24h = 1;
                        coin.MarketPrice.DayVolume = (double)result.volume;
                        //coin.MarketPrice.PriceDate = result.closeTime;
                    }
                    else
                    {
                        var btcprice = AppCore.Bitcoin.MarketPrice;
                        if (btcprice != null)
                        {
                            coin.MarketPrice.LatestPriceBTC = (double)result.lastPrice;
                            coin.MarketPrice.LatestPriceUSD = (double)result.lastPrice * btcprice.LatestPriceUSD;
                            coin.MarketPrice.PriceBTCBefore24h = (double)result.prevClosePrice;
                            coin.MarketPrice.PriceUSDBefore24h = (double)result.prevClosePrice * btcprice.PriceUSDBefore24h;
                            coin.MarketPrice.DayVolume = (double)result.quoteVolume;
                        }
                    }
                    coin.MarketPrice.PriceDate = result.closeTime;

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPriceAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

    }
}
