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

        public static async Task<List<Position>> FetchPositionAsync(Exchange binance)
        {
            _binance = binance;
            var path = "/api/v3/account";
            var positions = new List<Position>();

            try
            {
                var req = BuildRequest(path);
                var results = await RestUtil.ExecuteRequestAsync<BinanceAccount>(_restClient, req);

                foreach (var result in results.balances)
                {
                    var instrumentId = _binance.GetIdForExchange(result.asset);
                    var coin = AppCore.InstrumentList.GetByInstrumentId(instrumentId);
                    if (coin != null)
                    {
                        var qty = (double)result.free;
                        if (qty > 0)
                        {
                            var pos = new Position(coin)
                            {
                                Amount = qty,
                                BookedExchange = _binance
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

        public static async Task<TradeList> FetchTransactionAsync(Exchange binance, int calendarYear = 0)
        {
            _binance = binance;
            int limit = 500;
            var from = calendarYear == 0 ? new DateTime(2012, 1, 1) : new DateTime(calendarYear, 1, 1);
            var to = new DateTime(DateTime.Now.Year, 12, 31);
            var transactions = new Dictionary<BinanceTrade, string>();

            try
            {
                foreach (var coin in _binance.ListedCoins)
                {
                    string symbol;

                    if (coin.Id == "bitcoin" || coin.Id == "tether")
                    {
                        symbol = "BTCUSDT";
                    }
                    else
                    {
                        symbol = _binance.GetSymbolForExchange(coin.Id) + "BTC";
                    }

                    var results = await FetchTransactionsPageAsync(symbol, limit);

                    if (results.Any())
                    {
                        while (true)
                        {
                            var buffers = results.Where(x => from < x.time).Where(x => to >= x.time);

                            foreach(var tx in buffers)
                            {
                                transactions.Add(tx, symbol);
                            }

                            if (to < results.Last().time)
                            {
                                break;
                            }

                            if (results.Count == 0 || limit > results.Count)
                            {
                                break;
                            }

                            var lastId = results.Last().id;
                            results = await FetchTransactionsPageAsync(symbol, limit, lastId + 1);
                        }
                    }
                }

                var tradelist = new TradeList() { SettlementCCY = EnuCCY.JPY, TradedExchange = _binance };

                foreach (var tx in transactions)
                {
                    tradelist.AggregateTransaction(tx.Value.Replace("BTC",""),
                                                   AssetType.Cash,
                                                   tx.Key.isBuyer ? EnuSide.Buy : EnuSide.Sell,
                                                   Math.Abs((double)tx.Key.qty),
                                                   (double)tx.Key.price,
                                                   EnuCCY.BTC,
                                                   tx.Key.time,
                                                   (double)tx.Key.commission,
                                                   _binance
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

        private static async Task<List<BinanceTrade>> FetchTransactionsPageAsync(string symbol, int limit, int fromid = 0)
        {
            var path = $"/api/v3/myTrades";
            var param = $"symbol={symbol}&limit={limit}";

            //if (fromid != 0)
            //{
                param += $"&fromId={fromid}";
            //}

            var req = BuildRequest(path, param);
            return await RestUtil.ExecuteRequestAsync<List<BinanceTrade>>(_restClient, req);
        }

        private static RestRequest BuildRequest(string path, string param = "", string method = "GET", string body = "")
        {
            param += (!string.IsNullOrWhiteSpace(param) ? "&timestamp=" : "timestamp=") + Util.GenerateTimeStamp(DateTime.Now);
            var sign = Util.GenerateNewHmac(_binance.Secret, param);
            param += $"&signature={sign}";

            var req = RestUtil.CreateJsonRestRequest($"{path}?{param}");
            req.Method = Util.ParseEnum<Method>(method);
            if (body != "")
            {
                req.AddParameter("application/json", body, ParameterType.RequestBody);
            }
            req.AddHeader("X-MBX-APIKEY", _binance.Key);
            return req;
        }
    }
}
