using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using CoinBalance.CoreModel;
using RestSharp;

namespace CoinBalance.CoreAPI
{
    public static class CoinCheckAPI2
    {
        private const string ApiRoot = "https://coincheck.com";
        private static Exchange _coincheck;
        private static CrossRate _USDJPYrate;
        private static IRestClient _restClient = new RestClient(new Uri(ApiRoot));
        //private static AssetType _assetType = AssetType.Cash;

        public static async Task FetchPriceAsync(Exchange coincheck, InstrumentList coins, CrossRate USDJPYrate)
        {
            _coincheck = coincheck;
            _USDJPYrate = USDJPYrate;

            try
            {
                foreach (var coin in coins.Where(x => x.PriceSourceCode == coincheck.Code))
                {
                    var path = $"/api/rate/{_coincheck.GetSymbolForExchange(coin.Id).ToLower()}_jpy";

                    var req = RestUtil.CreateJsonRestRequest(path);

                    var response = await _restClient.ExecuteTaskAsync<CoinCheckRate>(req);
                    if (response.ErrorException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to receive price from CoinCheck.");
                    }

                    if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);

                    if (coin.Id is "bitcoin")
                    {
                        coin.MarketPrice.LatestPriceBTC = 1;
                        coin.MarketPrice.LatestPriceUSD = (double)response.Data.rate / USDJPYrate.Rate;
                        //coin.MarketPrice.PriceBTCBefore24h = 1;
                        //coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD; //tmp
                    }
                    else
                    {
                        var btcprice = AppCore.Bitcoin.MarketPrice;
                        if (btcprice != null)
                        {
                            coin.MarketPrice.LatestPriceUSD = (double)response.Data.rate / USDJPYrate.Rate;
                            coin.MarketPrice.LatestPriceBTC = coin.MarketPrice.LatestPriceUSD / btcprice.LatestPriceUSD;
                            //coin.MarketPrice.PriceBTCBefore24h = price_yesterday.LatestPriceBTC; //tmp
                            //coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD; //tmp
                        }
                    }
                    coin.MarketPrice.DayVolume = 0;
                    coin.MarketPrice.PriceDate = DateTime.Now;
                    //coin.MarketPrice.USDCrossRate = crossrate;

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPriceAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<List<Position>> FetchPositionAsync(Exchange coincheck)
        {
            _coincheck = coincheck;

            try
            {
                var path = "/api/accounts/balance";
                var req = BuildRequest(path);
                var result = await RestUtil.ExecuteRequestAsync<CoinCheckBalance>(_restClient, req);

                if (result.Success != true)
                {
                    throw new AppCoreParseException("Coincheck returned error: " + result);
                }
                else
                {
                    var positions = new List<Position>();

                    foreach (var property in result.GetType().GetProperties())
                    {
                        var instrumentId = _coincheck.GetIdForExchange(property.Name.ToUpper());
                        var coin = instrumentId != null ? AppCore.InstrumentList.GetByInstrumentId(instrumentId) : null;

                        if (coin != null)
                        {
                            var val = (decimal)result.GetType().GetProperty(property.Name).GetValue(result);
                            var qty = (double)val;
                            if (qty > 0)
                            {
                                var pos = new Position(coin)
                                {
                                    Amount = qty,
                                    BookedExchange = _coincheck
                                };
                                positions.Add(pos);
                            }
                        }
                    }

                    return positions;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPositionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        public static async Task<TradeList> FetchTransactionAsync(Exchange coincheck, int calendarYear = 0)
        {
            _coincheck = coincheck;
            var searchAfter = calendarYear == 0 ? new DateTime(2012, 1, 1) : new DateTime(calendarYear, 1, 1);
            var searchBefore = calendarYear == 0 ? DateTime.Now.Date : new DateTime(calendarYear, 12, 31);
            var transactions = new List<CoinCheckTransactions.Datum>();

            try
            {
                var results = await FetchTransactionsPageAsync();
                if (results.success != true)
                {
                    throw new AppCoreParseException("Coincheck returned error: " + results);
                }

                while (true)
                {
                    transactions.AddRange(results.data.
                                          Where(x => searchAfter < Util.IsoDateTimeToLocal(x.created_at)).
                                          Where(x => searchBefore >= Util.IsoDateTimeToLocal(x.created_at)));
                    
                    if (searchAfter > Util.IsoDateTimeToLocal(results.data.Last().created_at))
                    {
                        break;
                    }

                    if (results.data.Count == 0 ||
                        results.pagination.limit > results.data.Count)
                    {
                        break;
                    }

                    var lastId = results.data.Last().id;
                    results = await FetchTransactionsPageAsync(lastId);
                    if (results.success != true)
                    {
                        throw new AppCoreParseException("Coincheck returned error: " + results);
                    }
                }

                var tradelist = new TradeList() { SettlementCCY = EnuCCY.JPY, TradedExchange = _coincheck };

                foreach (var tx in transactions)
                {
                    EnuSide ebuysell;

                    switch (tx.side)
                    {
                        case "buy":
                            ebuysell = EnuSide.Buy;
                            break;
                        case "sell":
                            ebuysell = EnuSide.Sell;
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    var symbol = tx.pair;
                    symbol = symbol.Replace("_jpy", "").ToUpper();
                    decimal val = (decimal)tx.funds.GetType().GetProperty(symbol.ToLower()).GetValue(tx.funds);

                    tradelist.AggregateTransaction(symbol,
                                                   AssetType.Cash,
                                                   ebuysell,
                                                   Math.Abs((double)val),
                                                   (double)tx.rate,
                                                   EnuCCY.JPY,
                                                   Util.IsoDateTimeToLocal(tx.created_at),
                                                   (double)tx.fee,
                                                   _coincheck
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

        private static async Task<CoinCheckTransactions> FetchTransactionsPageAsync(string after = null, string before = null, int limit = 100,
            string order = "desc")
        {
            var path = $"/api/exchange/orders/transactions_pagination?limit={limit}&order={order}";
            if (after != null)
            {
                path += $"&starting_after={after}";
            }

            if (before != null)
            {
                path += $"&ending_before={before}";
            }

            var req = BuildRequest(path);
            return await RestUtil.ExecuteRequestAsync<CoinCheckTransactions>(_restClient, req);
        }

        private static RestRequest BuildRequest(string path, string method = "GET", string body = "")
        {
            var nonce = Util.Nonce;
            var url = ApiRoot + path;
            var message = nonce + url + body;
            var sign = Util.GenerateNewHmac(_coincheck.Secret, message);
            var req = RestUtil.CreateJsonRestRequest(path, false);
            req.Method = Util.ParseEnum<Method>(method);
            if (body != "")
            {
                req.AddParameter("application/x-www-form-urlencoded", body, ParameterType.RequestBody);
            }
            req.AddHeader("ACCESS-KEY", _coincheck.Key);
            req.AddHeader("ACCESS-NONCE", nonce);
            req.AddHeader("ACCESS-SIGNATURE", sign);
            return req;
        }

    }
}



