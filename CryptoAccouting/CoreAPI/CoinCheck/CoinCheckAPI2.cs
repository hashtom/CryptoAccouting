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
            var leveragetransactions = new List<CoinCheckLeveragePosition.Order>();
            var tradelist = new TradeList() { SettlementCCY = EnuCCY.JPY, TradedExchange = _coincheck };

            var transactions = await GetTransactionsAsync(calendarYear);
            var leveragePositions = await GetLeveragePositionsAsync(calendarYear);
            leveragePositions.ForEach(x => leveragetransactions.Add(x.new_order));
            leveragePositions.ForEach(x => leveragetransactions.AddRange(x.close_orders));

            try
            {
                foreach (var tx in transactions)
                {
                    var symbol = tx.pair;
                    symbol = symbol.Replace("_jpy", "").ToUpper();
                    decimal val = (decimal)tx.funds.GetType().GetProperty(symbol.ToLower()).GetValue(tx.funds);

                    tradelist.AggregateTransaction(symbol,
                                                   leveragetransactions.Any(x => x.id == tx.order_id) ? AssetType.Margin : AssetType.Cash,
                                                   Util.ParseEnum<EnuSide>(tx.side),
                                                   Math.Abs((decimal)val),
                                                   (decimal)tx.rate,
                                                   EnuCCY.JPY,
                                                   Util.IsoDateTimeToLocal(tx.created_at),
                                                   (decimal)tx.fee,
                                                   _coincheck
                                                  );
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchTransactionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

            return tradelist;
        }

        private static async Task<List<CoinCheckTransactions.transaction>> GetTransactionsAsync(int calendarYear = 0)
        {
            var from = calendarYear == 0 ? new DateTime(2012, 1, 1) : new DateTime(calendarYear, 1, 1);
            var to = calendarYear == 0 ? new DateTime(DateTime.Now.Year, 12, 31) : new DateTime(calendarYear, 12, 31);
            var transactions = new List<CoinCheckTransactions.transaction>();

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
                                          Where(x => from < Util.IsoDateTimeToLocal(x.created_at)).
                                          Where(x => to >= Util.IsoDateTimeToLocal(x.created_at)));

                    if (to < Util.IsoDateTimeToLocal(results.data.Last().created_at))
                    {
                        break;
                    }

                    if (results.data.Count == 0 || results.pagination.limit > results.data.Count)
                    {
                        break;
                    }

                    var lastId = results.data.Last().id;
                    results = await FetchTransactionsPageAsync(null, lastId);
                    if (results.success != true)
                    {
                        throw new AppCoreParseException("Coincheck returned error: " + results);
                    }
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": GetTransactionsAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

            return transactions;

        }

        private static async Task<List<CoinCheckLeveragePosition.position>> GetLeveragePositionsAsync(int calendarYear = 0)
        {
            
            var from = calendarYear == 0 ? new DateTime(2012, 1, 1) : new DateTime(calendarYear, 1, 1);
            var to = calendarYear == 0 ? new DateTime(DateTime.Now.Year, 12, 31) : new DateTime(calendarYear, 12, 31);
            var positions = new List<CoinCheckLeveragePosition.position>();

            try
            {
                var results = await GetLeveragePositionsPageAsync();
                if (results.success != true)
                {
                    throw new AppCoreParseException("Coincheck returned error: " + results);
                }

                while (true)
                {
                    positions.AddRange(results.data.
                                       Where(x => from < Util.IsoDateTimeToLocal(x.created_at)).
                                       Where(x => to >= Util.IsoDateTimeToLocal(x.created_at)));

                    if (to < Util.IsoDateTimeToLocal(results.data.Last().created_at))
                    {
                        break;
                    }

                    if (results.data.Count == 0 || results.pagination.limit > results.data.Count)
                    {
                        break;
                    }

                    var lastId = results.data.Last().id;
                    results = await GetLeveragePositionsPageAsync(null, null, lastId);
                    if (results.success != true)
                    {
                        throw new AppCoreParseException("Coincheck returned error: " + results);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": GetLeveragePositionsAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

            return positions;

        }

        private static async Task<CoinCheckLeveragePosition> GetLeveragePositionsPageAsync(string status = null, string after = null, string before = null, string order = "asc")
        {
            var path = $"/api/exchange/leverage/positions?limit=50&order={order}";

            if (status != null)
            {
                path += $"&atatus={status}";
            }

            if (after != null)
            {
                path += $"&starting_after={after}";
            }

            if (before != null)
            {
                path += $"&ending_before={before}";
            }
            var req = BuildRequest(path);
            return await RestUtil.ExecuteRequestAsync<CoinCheckLeveragePosition>(_restClient, req);
        }

        private static async Task<CoinCheckTransactions> FetchTransactionsPageAsync(string after = null, string before = null, string order = "asc")
        {
            var path = $"/api/exchange/orders/transactions_pagination?limit=50&order={order}";
            //var path = $"/api/exchange/orders/transactions_pagination?order={order}";
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



