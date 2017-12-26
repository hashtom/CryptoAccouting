using System;
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
        //private static CrossRate _USDJPYrate;
        private static IRestClient _restClient = new RestClient(new Uri(ApiRoot));
        private static Dictionary<string, string> ProductIdMap = new Dictionary<string, string>();

        public static async Task FetchPriceAsync(Exchange quoine, InstrumentList coins)
        {
            _quoine = quoine;

            try
            {

                foreach (var coin in coins.Where(x => x.PriceSourceCode == _quoine.Code))
                {
                    if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);

                    var product_id = GetProductID(_quoine.GetSymbolForExchange(coin.Id));
                    var path = $"/products/{product_id}";

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

        public static async Task<TradeList> FetchMarginTransactionAsync(Exchange quoine)
        {
            _quoine = quoine;
            var tradelist = new TradeList() { SettlementCCY = EnuCCY.JPY, TradedExchange = _quoine };
            var path = "/trades";

            try
            {
                var req = BuildRequest(path);
                var results = await RestUtil.ExecuteRequestAsync<List<QuoineTrade>>(_restClient, req);

                foreach (var result in results)
                {

                    tradelist.AggregateTransaction(_quoine.GetSymbolForExchange("bitcoin"),
                                                   result.side.Contains("long")? EnuBuySell.Buy : EnuBuySell.Sell,
                                                   (double)result.open_quantity,
                                                   (double)result.open_price,
                                                   EnuCCY.JPY,
                                                   result.created_at,
                                                   0,
                                                   _quoine
                                                  );

                    if (result.close_quantity > 0)
                    {
                        tradelist.AggregateTransaction(_quoine.GetSymbolForExchange("bitcoin"),
                                                       result.side.Contains("long") ? EnuBuySell.Sell : EnuBuySell.Buy,
                                                       (double)result.close_quantity,
                                                       (double)result.close_price,
                                                       EnuCCY.JPY,
                                                       result.created_at,
                                                       0,
                                                       _quoine
                                                      );
                    }
                }

                return tradelist.Any() ? tradelist : throw new AppCoreWarning("No data returned from the Exchange.");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchTransactionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        public static async Task<TradeList> FetchExecutionAsync(Exchange quoine)
        {
            _quoine = quoine;
            var tradelist = new TradeList() { SettlementCCY = EnuCCY.JPY, TradedExchange = _quoine };
            var executions = new List<QuoineExecutions.execution>();

            var product_ids = _quoine.ListedCoins.Select(x => GetProductID(_quoine.GetSymbolForExchange(x.Id))).ToList();

            try
            {
                
                foreach (var product_id in product_ids)
                {
                    var path = $"/executions/me?product_id={product_id}&limit=1000";
                    var req = BuildRequest(path);
                    var results = await RestUtil.ExecuteRequestAsync<QuoineExecutions>(_restClient, req);
                    executions.AddRange(results.models);
                }

                foreach (var execution in executions)
                {

                    tradelist.AggregateTransaction(_quoine.GetSymbolForExchange("bitcoin"),
                                                   execution.my_side.Contains("buy") ? EnuBuySell.Buy : EnuBuySell.Sell,
                                                   (double)execution.quantity,
                                                   (double)execution.price,
                                                   EnuCCY.JPY,
                                                   execution.created_at,
                                                   0,
                                                   _quoine
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

        private static string GetProductID(string Quoinecurrency)
        {

            if(ProductIdMap == null) 
            {
                ProductIdMap = new Dictionary<string, string>();

                var path = $"/products";

                var req = RestUtil.CreateJsonRestRequest(path);
                var response = _restClient.Execute<List<QuoineProduct>>(req);
                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }

                foreach(var product in response.Data.
                        Where(x=>x.currency == AppCore.BaseCurrency.ToString()))
                {
                    ProductIdMap.Add(product.code, product.currency);
                }
            }

            return ProductIdMap.First(x => x.Value == Quoinecurrency).Key;
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


