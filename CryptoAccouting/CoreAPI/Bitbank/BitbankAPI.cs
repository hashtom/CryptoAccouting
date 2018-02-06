using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using CoinBalance.CoreModel;
using RestSharp;

namespace CoinBalance.CoreAPI
{
    public class BitbankAPI
    {
        private const string PublicApiRoot = "https://public.bitbank.cc";
        private const string PrivateApiRoot = "https://api.bitbank.cc";
        private static Exchange _bitbank;
        private static CrossRate _USDJPYrate;
        private static IRestClient _restClient = new RestClient(new Uri(PublicApiRoot));

        public static async Task FetchPriceAsync(Exchange bitbank, InstrumentList coins, CrossRate USDJPYrate)
        {
            _bitbank = bitbank;
            _USDJPYrate = USDJPYrate;
            _restClient.BaseUrl = new Uri(PublicApiRoot);

            try
            {

                foreach (var coin in coins.Where(x => x.PriceSourceCode == bitbank.Code))
                {
                    var path = $"/{_bitbank.GetSymbolForExchange(coin.Id).ToLower()}_jpy/ticker";
                    var req = RestUtil.CreateJsonRestRequest(path);

                    var response = await _restClient.ExecuteTaskAsync<BitbankTicker>(req);
                    if (response.ErrorException != null)
                    {
                        throw response.ErrorException;
                    }

                    if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);
                    if (coin.Id is "bitcoin")
                    {
                        coin.MarketPrice.LatestPriceBTC = 1;
                        coin.MarketPrice.LatestPriceUSD = (double)response.Data._data.last / _USDJPYrate.Rate;
                        //coin.MarketPrice.PriceBTCBefore24h = 1;
                        //coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD; //tmp
                    }
                    else
                    {
                        var btcprice = AppCore.Bitcoin.MarketPrice;
                        if (btcprice != null)
                        {
                            coin.MarketPrice.LatestPriceUSD = (double)response.Data._data.last / _USDJPYrate.Rate;
                            coin.MarketPrice.LatestPriceBTC = coin.MarketPrice.LatestPriceUSD / btcprice.LatestPriceUSD;
                            //coin.MarketPrice.PriceBTCBefore24h = price_yesterday.LatestPriceUSD; //tmp
                            //coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD; //tmp
                        }
                    }

                    coin.MarketPrice.DayVolume = (double)response.Data._data.vol;
                    coin.MarketPrice.PriceDate = response.Data._data.timestamp;
                    //coin.MarketPrice.USDCrossRate = _crossrate;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPriceAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<List<Position>> FetchPositionAsync(Exchange bitbank)
        {
            _bitbank = bitbank;
            var path = "/v1/user/assets";
            var positions = new List<Position>();
             _restClient.BaseUrl = new Uri(PrivateApiRoot);

            try
            {
                var req = BuildRequest(path);
                var results = await RestUtil.ExecuteRequestAsync<BitbankAsset>(_restClient, req);

                foreach (var result in results._data.assets)
                {
                    var instrumentId = _bitbank.GetIdForExchange(result.asset.ToUpper());
                    var coin = AppCore.InstrumentList.GetByInstrumentId(instrumentId);
                    if (coin != null)
                    {
                        var qty = (double)result.free_amount;
                        if (qty > 0)
                        {
                            var pos = new Position(coin)
                            {
                                Amount = qty,
                                BookedExchange = _bitbank
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

        //public static async Task<TradeList> FetchTransactionAsync(Exchange bitbank, int calendarYear = 0)
        //{
        //    _bitbank = bitbank;
        //    var leveragetransactions = new List<CoinCheckLeveragePosition.Order>();
        //    var tradelist = new TradeList() { SettlementCCY = EnuCCY.JPY, TradedExchange = _bitbank };

        //    var transactions = await GetTransactionsAsync(calendarYear);

        //    try
        //    {
        //        foreach (var tx in transactions)
        //        {
        //            var symbol = tx.pair;
        //            symbol = symbol.Replace("_jpy", "").ToUpper();
        //            decimal val = (decimal)tx.funds.GetType().GetProperty(symbol.ToLower()).GetValue(tx.funds);

        //            tradelist.AggregateTransaction(symbol,
        //                                           leveragetransactions.Any(x => x.id == tx.order_id) ? AssetType.Margin : AssetType.Cash,
        //                                           Util.ParseEnum<EnuSide>(tx.side),
        //                                           Math.Abs((decimal)val),
        //                                           (decimal)tx.rate,
        //                                           EnuCCY.JPY,
        //                                           tx.created_at,
        //                                           (decimal)tx.fee,
        //                                           _bitbank
        //                                          );
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchTransactionAsync: " + e.GetType() + ": " + e.Message);
        //        throw;
        //    }

        //    return tradelist;
        //}


        //private static async Task<List<CoinCheckTransactions.transaction>> GetTransactionsAsync(int calendarYear = 0)
        //{
        //    var from = calendarYear == 0 ? new DateTime(2012, 1, 1) : new DateTime(calendarYear, 1, 1);
        //    var to = calendarYear == 0 ? new DateTime(DateTime.Now.Year, 12, 31) : new DateTime(calendarYear, 12, 31);
        //    var transactions = new List<CoinCheckTransactions.transaction>();

        //    try
        //    {
        //        var results = await FetchTransactionsPageAsync();
        //        if (results.success != true)
        //        {
        //            throw new AppCoreParseException("Coincheck returned error: " + results);
        //        }

        //        while (true)
        //        {
        //            transactions.AddRange(results.data.Where(x => from < x.created_at).Where(x => to >= x.created_at));

        //            if (to < results.data.Last().created_at)
        //            {
        //                break;
        //            }

        //            if (results.data.Count == 0 || results.pagination.limit > results.data.Count)
        //            {
        //                break;
        //            }

        //            var lastId = results.data.Last().id;
        //            results = await FetchTransactionsPageAsync(null, lastId);
        //            if (results.success != true)
        //            {
        //                throw new AppCoreParseException("Coincheck returned error: " + results);
        //            }
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": GetTransactionsAsync: " + e.GetType() + ": " + e.Message);
        //        throw;
        //    }

        //    return transactions;

        //}

        //private static async Task<CoinCheckTransactions> FetchTransactionsPageAsync(string pair, string since = null, string before = null, string order = "asc")
        //{
     
        //    var path = $"/v1/user/spot/trade_history?pair={pair}order_id={order_id}&count=10";
        //    if (after != null)
        //    {
        //        path += $"&since={Util.ToEpochSeconds(since)}";
        //    }

        //    if (before != null)
        //    {
        //        path += $"&end={Util.ToEpochSeconds(DateTime.Now)}";
        //    }

        //    var req = BuildRequest(path);
        //    return await RestUtil.ExecuteRequestAsync<CoinCheckTransactions>(_restClient, req);
        //}


        private static RestRequest BuildRequest(string path, string method = "GET", string body = "")
        {
            var nonce = Util.Nonce;
            var message = method == "GET" ? nonce + path : nonce + body;
            var sign = Util.GenerateNewHmac(_bitbank.Secret, message);
            var req = RestUtil.CreateJsonRestRequest(path, false);
            req.Method = Util.ParseEnum<Method>(method);
            if (body != "")
            {
                req.AddParameter("application/x-www-form-urlencoded", body, ParameterType.RequestBody);
            }
            req.AddHeader("ACCESS-KEY", _bitbank.Key);
            req.AddHeader("ACCESS-NONCE", nonce);
            req.AddHeader("ACCESS-SIGNATURE", sign);
            return req;
        }
    }
}
