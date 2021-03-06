﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using CoinBalance.CoreModel;
using RestSharp;

namespace CoinBalance.CoreAPI
{
    public static class QuoineAPI
    {

        private const string ApiRoot = "https://api.quoine.com";
        private static Exchange _quoine;
        private static IRestClient _restClient = new RestClient(new Uri(ApiRoot));
        private static List<QuoineProduct> Products = new List<QuoineProduct>();

        public static async Task FetchPriceAsync(Exchange quoine, InstrumentList coins)
        {
            _quoine = quoine;

            try
            {

                foreach (var coin in coins.Where(x => x.PriceSourceCode == _quoine.Code))
                {
                    if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);

                    var product = GetProduct(_quoine.GetSymbolForExchange(coin.Id));
                    var path = $"/products/{product.Id}";

                    var req = RestUtil.CreateJsonRestRequest(path);
                    var response = await _restClient.ExecuteGetTaskAsync<QuoineProduct>(req);
                    if (response.ErrorException != null)
                    {
                        throw response.ErrorException;
                    }

                    //var result = response.Data.First(x => x.code == _quoine.GetSymbolForExchange(coin.Id));
                    var result = response.Data;

                    if (coin.Id is "bitcoin")
                    {
                        coin.MarketPrice.LatestPriceBTC = 1;
                        coin.MarketPrice.LatestPriceUSD = (double)result.last_traded_price / AppCore.GetLatestCrossRate();
                    }
                    else
                    {
                        var btcprice = AppCore.Bitcoin.MarketPrice;
                        if (btcprice != null)
                        {
                            coin.MarketPrice.LatestPriceUSD = (double)result.last_traded_price / AppCore.GetLatestCrossRate();
                            coin.MarketPrice.LatestPriceBTC = coin.MarketPrice.LatestPriceUSD / btcprice.LatestPriceUSD;
                        }
                    }

                    coin.MarketPrice.DayVolume = (double)result.volume_24h;
                    coin.MarketPrice.PriceDate = DateTime.Now;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPriceAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<List<Position>> FetchPositionAsync(Exchange quoine)
        {
            _quoine = quoine;
            //var path = "/trading_accounts";
            var path = "/accounts/balance";
            var positions = new List<Position>();

            try
            {
                var req = BuildRequest(path);
                var results = await RestUtil.ExecuteRequestAsync<List<QuoineBalance>>(_restClient, req);

                foreach (var result in results)
                {
                    var instrumentId = _quoine.GetIdForExchange(result.currency);
                    var coin = AppCore.InstrumentList.GetByInstrumentId(instrumentId);
                    if (coin != null)
                    {
                        var qty = (double)result.balance;
                        if (qty > 0)
                        {
                            var pos = new Position(coin)
                            {
                                Amount = qty,
                                BookedExchange = _quoine
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

        public static async Task<TradeList> FetchTransactionAsync(Exchange quoine, int calendarYear = 0)
        {
            _quoine = quoine;
            int limit = 500;
            var from = calendarYear == 0 ? new DateTime(2012, 1, 1) : new DateTime(calendarYear, 1, 1);
            var to = calendarYear == 0 ? new DateTime(DateTime.Now.Year, 12, 31) : new DateTime(calendarYear, 12, 31);
            var tradelist = new TradeList() { SettlementCCY = (EnuCCY)AppCore.BaseCurrency, TradedExchange = _quoine };
            var orders = new List<QuoineOrders.Order>();
            var products = GetProducts();

            try
            {
                var trades = await GetOrdersPageAsync(limit);

                if (trades.models.Any())
                {

                    while (true)
                    {
                        orders.AddRange(trades.models.Where(x => x.filled_quantity > 0).Where(x => from < x.created_at).Where(x => to >= x.created_at));

                        if (trades.current_page == trades.total_pages)
                        {
                            break;
                        }

                        trades = await GetOrdersPageAsync(limit, trades.current_page + 1);
                    }

                    foreach (var trade in orders)
                    {
                        if (!products.Any(x => x.currency_pair_code == trade.currency_pair_code))
                        {
                            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + $": FetchTransactionAsync: Warning no pair {trade.currency_pair_code} found.");
                        }
                        else
                        {
                            var symbol = products.First(x => x.currency_pair_code == trade.currency_pair_code).base_currency;

                            foreach (var e in trade.executions)
                            {
                                tradelist.AggregateTransaction(symbol,
                                                               trade.leverage_level == 1 ? AssetType.Cash : AssetType.Margin,
                                                               Util.ParseEnum<EnuSide>(e.my_side),
                                                               e.quantity,
                                                               e.price,
                                                               EnuCCY.JPY,
                                                               e.created_at,
                                                               0,
                                                               _quoine
                                                              );
                            }
                        }
                    }
                }

                return tradelist;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchTransactionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }


        public static async Task<List<RealizedPL>> FetchLeveragePLAsync(Exchange quoine, int calendarYear = 0)
        {
            _quoine = quoine;
            var leveragePL = new List<RealizedPL>();
            var leveragePositions = await GetLeverageTradesAsync(calendarYear, "closed");
            var products = GetProducts();

            try
            {
                foreach (var p in leveragePositions)
                {
                    if (p.close_quantity > 0 && p.pnl != 0)
                    {
                        var symbol = products.First(x => x.currency_pair_code == p.currency_pair_code).base_currency;
                        var id = _quoine.GetIdForExchange(symbol);
                        var pl = new RealizedPL(
                            AppCore.InstrumentList.GetByInstrumentId(id),
                            p.leverage_level == 1 ? EnuPLType.CashTrade : EnuPLType.MarginTrade,
                            p.updated_at,
                            p.side.Contains("long") ? EnuSide.Buy : EnuSide.Sell,
                            Util.ParseEnum<EnuBaseFiatCCY>(p.funding_currency),
                            p.close_quantity,
                            p.open_price,
                            p.close_price,
                            _quoine);

                        var grossprofit = p.side.Contains("long") ? (p.close_price - p.open_price) * p.close_quantity : (p.open_price - p.close_price) * p.close_quantity;

                        // No fee information in this output //todo

                        //if (p.leverage_level == 1)
                        //{
                        //    pl.TradeFee = grossprofit - p.close_pnl;
                        //}
                        //else
                        //{
                        //    pl.MarginFee = grossprofit - p.close_pnl;
                        //}

                        leveragePL.Add(pl);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchLeveragePLAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

            return leveragePL;

        }

        //public static async Task<TradeList> FetchExecutionAsync(Exchange quoine, int calendarYear = 0)
        //{
        //    _quoine = quoine;
        //    int limit = 500;
        //    var from = calendarYear == 0 ? new DateTime(2012, 1, 1) : new DateTime(calendarYear, 1, 1);
        //    var to = calendarYear == 0 ? new DateTime(DateTime.Now.Year, 12, 31) : new DateTime(calendarYear, 12, 31);
        //    var tradelist = new TradeList() { SettlementCCY = (EnuCCY)AppCore.BaseCurrency, TradedExchange = _quoine };
        //    var products = GetProducts();

        //    try
        //    {
        //        foreach (var product in products)
        //        {
        //            var executions = new List<QuoineExecutions.Execution>();

        //            var results = await FetchExecutionsPageAsync(product.Id, limit);

        //            while (true)
        //            {
        //                executions.AddRange(results.models.
        //                                      Where(x => from < x.created_at).
        //                                      Where(x => to >= x.created_at));

        //                if (results.current_page == results.total_pages)
        //                {
        //                    break;
        //                }

        //                results = await FetchExecutionsPageAsync(product.Id, limit, results.current_page + 1);
        //            }


        //            foreach (var execution in executions)
        //            {

        //                tradelist.AggregateTransaction(product.base_currency,
        //                                               AssetType.Cash,
        //                                               execution.my_side.Contains("buy") ? EnuSide.Buy : EnuSide.Sell,
        //                                               (decimal)execution.quantity,
        //                                               (decimal)execution.price,
        //                                               EnuCCY.JPY,
        //                                               execution.created_at,
        //                                               0,
        //                                               _quoine
        //                                              );

        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchTransactionAsync: " + e.GetType() + ": " + e.Message);
        //        throw;
        //    }

        //    return tradelist;
        //}

        private static async Task<List<QuoineTrades.Trade>> GetLeverageTradesAsync(int calendarYear = 0, string status = null)
        {
            int limit = 500;
            var from = calendarYear == 0 ? new DateTime(2012, 1, 1) : new DateTime(calendarYear, 1, 1);
            var to = calendarYear == 0 ? new DateTime(DateTime.Now.Year, 12, 31) : new DateTime(calendarYear, 12, 31);
            var positions = new List<QuoineTrades.Trade>();

            try
            {
                var results = await GetLeverageTradesPageAsync(limit, status);

                while (true)
                {
                    positions.AddRange(results.models.Where(x => from < x.updated_at).Where(x => to >= x.updated_at));

                    if (results.current_page == results.total_pages)
                    {
                        break;
                    }

                    results = await GetLeverageTradesPageAsync(limit, status, results.current_page + 1);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": GetLeveragePositionsAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

            return positions;
        }

        private static async Task<List<QuoineTradingAccount>> GetTradingAccountAsync(int calendarYear = 0, string status = null)
        {
            int limit = 500;
            var from = calendarYear == 0 ? new DateTime(2012, 1, 1) : new DateTime(calendarYear, 1, 1);
            var to = calendarYear == 0 ? new DateTime(DateTime.Now.Year, 12, 31) : new DateTime(calendarYear, 12, 31);
            var accounts = new List<QuoineTradingAccount>();

            try
            {
                accounts = await GetTradeAccountPageAsync(limit);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": GetLeveragePositionsAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

            return accounts;
        }

        private static async Task<QuoineOrders> GetOrdersPageAsync(int limit, int page = 0)
        {
            var path = $"/orders?funding_currency={AppCore.BaseCurrency}&limit={limit}&with_details=1";
            if (page != 0)
            {
                path += $"&page={page}";
            }

            var req = BuildRequest(path);

            return await RestUtil.ExecuteRequestAsync<QuoineOrders>(_restClient, req);
        }

        private static async Task<QuoineTrades> GetLeverageTradesPageAsync(int limit, string status, int page = 0)
        {
            var path = $"/trades?funding_currency={AppCore.BaseCurrency}&limit={limit}";

            if(status != null)
            {
                path += $"&status={status}";
            }

            if (page != 0)
            {
                path += $"&page={page}";
            }

                var req = BuildRequest(path);
            return await RestUtil.ExecuteRequestAsync<QuoineTrades>(_restClient, req);
        }

        private static async Task<List<QuoineTradingAccount>> GetTradeAccountPageAsync(int limit, int page = 0)
        {
            var path = $"/trading_accounts?funding_currency={AppCore.BaseCurrency}&limit={limit}";

            if (page != 0)
            {
                path += $"&page={page}";
            }

            var req = BuildRequest(path);
            return await RestUtil.ExecuteRequestAsync<List<QuoineTradingAccount>>(_restClient, req);
        }

        //private static async Task<QuoineExecutions> FetchExecutionsPageAsync(string productid, int limit, int page = 0)
        //{
        //    var path = $"/executions/me?product_id={productid}&limit={limit}";
        //    if (page != 0)
        //    {
        //        path += $"&page={page}";
        //    }

        //    var req = BuildRequest(path);
        //    return await RestUtil.ExecuteRequestAsync<QuoineExecutions>(_restClient, req);
        //}

        private static List<QuoineProduct> GetProducts()
        {
            if (!Products.Any(x => x.currency == AppCore.BaseCurrency.ToString()))
            {
                var path = $"/products/";
                var req = RestUtil.CreateJsonRestRequest(path);
                var response = _restClient.Execute<List<QuoineProduct>>(req);
                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }

                foreach (var product in response.Data.
                        Where(x => x.disabled == false).
                        Where(x => x.currency == AppCore.BaseCurrency.ToString()))
                {
                    var instrumentid = _quoine.GetIdForExchange(product.base_currency);
                    if (_quoine.IsListed(instrumentid))
                    {
                        Products.Add(product);
                    }
                }
            }
            return Products;
        }

        private static QuoineProduct GetProduct(string Quoinecurrency)
        {
            return GetProducts().First(x => x.base_currency == Quoinecurrency);
        }

        private static RestRequest BuildRequest(string path, string method = "GET", string body = "")
        {
            var nonce = Util.Nonce;
            var payload = new Dictionary<string, object>
            {
                {"path", path},
                {"nonce", nonce},
                {"token_id", _quoine.Key}
            };
            var sign = Util.JwtHs256Encode(payload, _quoine.Secret);
            var req = RestUtil.CreateJsonRestRequest(path);
            req.Method = Util.ParseEnum<Method>(method);
            if (body != "")
            {
                req.AddParameter("application/json", body, ParameterType.RequestBody);
            }
            req.AddHeader("X-Quoine-API-Version", "2");
            req.AddHeader("X-Quoine-Auth", sign);
            return req;
        }
    }
}


