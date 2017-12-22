using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using CoinBalance.CoreModel;
using RestSharp;

namespace CoinBalance.CoreAPI
{
    public static class BitFlyerAPI2
    {

        private const string ApiRoot = "https://api.bitflyer.jp";
        private static Exchange _bitflyer;
        private static CrossRate _USDJPYrate;
        private static IRestClient _restClient = new RestClient(new Uri(ApiRoot));

        public static async Task FetchPriceAsync(Exchange bitflyer, InstrumentList coins, CrossRate USDJPYrate)
        {
            _bitflyer = bitflyer;
            _USDJPYrate = USDJPYrate;

            try
            {
                foreach (var coin in coins.Where(x => x.PriceSourceCode == bitflyer.Code))
                {
                    var path = $"/v1/ticker?product_code={_bitflyer.GetSymbolForExchange(coin.Id)}_JPY";
                    var req = RestUtil.CreateJsonRestRequest(path);

                    var response = await _restClient.ExecuteTaskAsync<BitflyerTicker>(req);
                    if (response.ErrorException != null)
                    {
                        throw response.ErrorException;
                    }


                    if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);
                    if (coin.Id is "bitcoin")
                    {
                        coin.MarketPrice.LatestPriceBTC = 1;
                        coin.MarketPrice.LatestPriceUSD = (double)response.Data.ltp / _USDJPYrate.Rate;
                        //coin.MarketPrice.PriceBTCBefore24h = 1;
                        //coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD; //tmp
                    }
                    else
                    {
                        var btcprice = AppCore.Bitcoin.MarketPrice;
                        if (btcprice != null)
                        {
                            coin.MarketPrice.LatestPriceUSD = (double)response.Data.ltp/ _USDJPYrate.Rate;
                            coin.MarketPrice.LatestPriceBTC = coin.MarketPrice.LatestPriceUSD / btcprice.LatestPriceUSD;
                            //coin.MarketPrice.PriceBTCBefore24h = price_yesterday.LatestPriceUSD; //tmp
                            //coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD; //tmp
                        }
                    }

                    coin.MarketPrice.DayVolume = (double)response.Data.volume_by_product;
                    coin.MarketPrice.PriceDate = response.Data.timestamp.ToLocalTime();
                    //coin.MarketPrice.USDCrossRate = _crossrate;

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPriceAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<List<Position>> FetchPositionAsync(Exchange bitflyer)
        {
            _bitflyer = bitflyer;
            var path = "/v1/me/getbalance";
            var positions = new List<Position>();

            try
            {
                var req = BuildRequest(path);
                var results = await RestUtil.ExecuteRequestAsync<List<BitflyerBalance>>(_restClient, req);

                foreach (var result in results)
                {
                    var instrumentId = _bitflyer.GetIdForExchange(result.CurrencyCode);
                    var coin = AppCore.InstrumentList.GetByInstrumentId(instrumentId);
                    if (coin != null)
                    {
                        var qty = (double)result.Amount;
                        if (qty > 0)
                        {
                            var pos = new Position(coin)
                            {
                                Amount = qty,
                                BookedExchange = _bitflyer
                            };
                            positions.Add(pos);
                        }
                    }
                }

                return positions;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPositionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<TradeList> FetchTransactionAsync(Exchange bitflyer)
        {
            _bitflyer = bitflyer;
            var tradelist = new TradeList() { SettlementCCY = EnuCCY.JPY, TradedExchange = _bitflyer };
            var path = $"/v1/me/getexecutions?product_code=BTC_JPY";

            try
            {
                var req = BuildRequest(path);
                var results = await RestUtil.ExecuteRequestAsync<List<BitflyerExecution>>(_restClient, req);

                foreach (var result in results)
                {

                    EnuBuySell ebuysell;

                    if (result.side.Contains("BUY"))
                    {
                        ebuysell = EnuBuySell.Buy;
                    }
                    else if (result.side.Contains("SELL"))
                    {
                        ebuysell = EnuBuySell.Sell;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    tradelist.AggregateTransaction(_bitflyer.GetSymbolForExchange("bitcoin"),
                                                   ebuysell,
                                                   (double)result.size,
                                                   (double)result.price,
                                                   EnuCCY.JPY,
                                                   result.exec_date.ToLocalTime(),
                                                   (double)result.commission * (double)result.price,
                                                   _bitflyer
                                                  );
                }

                return tradelist.Any() ? tradelist : throw new AppCoreWarning("No data returned from the Exchange.");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchTransactionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        private static RestRequest BuildRequest(string path, string method = "GET", string body = "")
        {
            var nonce = Util.Nonce;
            var message = nonce + method + path + body;
            var sign = Util.GenerateNewHmac(_bitflyer.Secret, message);
            var req = RestUtil.CreateJsonRestRequest(path);
            req.Method = Util.ParseEnum<Method>(method);
            req.AddParameter("application/json", body, ParameterType.RequestBody);
            req.AddHeader("ACCESS-KEY", _bitflyer.Key);
            req.AddHeader("ACCESS-TIMESTAMP", nonce);
            req.AddHeader("ACCESS-SIGN", sign);
            return req;
        }

    }
}

