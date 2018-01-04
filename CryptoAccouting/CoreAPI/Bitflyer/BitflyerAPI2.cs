using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public static async Task<TradeList> FetchTransactionAsync(Exchange bitflyer, int calendarYear = 0)
        {
            _bitflyer = bitflyer;
            //var path = $"/v1/me/getexecutions?product_code=BTC_JPY";
            int limit = 500;
            var from = calendarYear == 0 ? new DateTime(2012, 1, 1) : new DateTime(calendarYear, 1, 1);
            var to = calendarYear == 0 ? new DateTime(DateTime.Now.Year, 12, 31) : new DateTime(calendarYear, 12, 31);
            var transactions = new List<BitflyerExecution>();
            try
            {
                //var req = BuildRequest(path);
                //var results = await RestUtil.ExecuteRequestAsync<List<BitflyerExecution>>(_restClient, req);
                var results = await FetchTransactionsPageAsync(limit);

                while (true)
                {
                    transactions.AddRange(results.
                                          Where(x => from < x.exec_date).
                                          Where(x => to >= x.exec_date));

                    if (from > results.Last().exec_date)
                    {
                        break;
                    }

                    if (results.Count == 0 ||
                        limit > results.Count)
                    {
                        break;
                    }

                    var lastId = results.Last().id;
                    results = await FetchTransactionsPageAsync(limit, lastId);
                }

                var tradelist = new TradeList() { SettlementCCY = EnuCCY.JPY, TradedExchange = _bitflyer };

                foreach (var result in transactions)
                {

                    EnuSide ebuysell;

                    if (result.side.Contains("BUY"))
                    {
                        ebuysell = EnuSide.Buy;
                    }
                    else if (result.side.Contains("SELL"))
                    {
                        ebuysell = EnuSide.Sell;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    tradelist.AggregateTransaction(_bitflyer.GetSymbolForExchange("bitcoin"),
                                                   AssetType.Cash,
                                                   ebuysell,
                                                   (decimal)result.size,
                                                   (decimal)result.price,
                                                   EnuCCY.JPY,
                                                   result.exec_date.ToLocalTime(),
                                                   (decimal)result.commission * (decimal)result.price,
                                                   _bitflyer
                                                  );
                }

                //return tradelist.Any() ? tradelist : throw new AppCoreWarning("No data returned from the Exchange.");
                return tradelist;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchTransactionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<List<BitflyerExecution>> FetchTransactionsPageAsync(int limit = 500, string after = null, string before = null)
        {
            var path = $"/v1/me/getexecutions?product_code=BTC_JPY&count={limit}";

            if (after != null)
            {
                path += $"&after={after}";
            }

            if (before != null)
            {
                path += $"&before={before}";
            }

            try
            {
                var req = BuildRequest(path);
                return await RestUtil.ExecuteRequestAsync<List<BitflyerExecution>>(_restClient, req);
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

