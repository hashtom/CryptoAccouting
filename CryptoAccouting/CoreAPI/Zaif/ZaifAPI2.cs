﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using RestSharp;
using CoinBalance.CoreModel;

namespace CoinBalance.CoreAPI
{
    public static class ZaifAPI2
    {
        private const string ApiRoot = "https://api.zaif.jp";
        private static Exchange _zaif;
        private static CrossRate _USDJPYrate;
        private static IRestClient _restClient = new RestClient(new Uri(ApiRoot));

        public static async Task FetchPriceAsync(Exchange zaif, InstrumentList coins, CrossRate USDJPYrate)
        {
            _zaif = zaif;
            _USDJPYrate = USDJPYrate;

            try
            {

                foreach (var coin in coins.Where(x => x.PriceSourceCode == zaif.Code))
                {
                    var path = $"/api/1/ticker/{_zaif.GetSymbolForExchange(coin.Id).ToLower()}_jpy";
                    var req = RestUtil.CreateJsonRestRequest(path);

                    var response = await _restClient.ExecuteTaskAsync<ZaifTicker>(req);
                    if (response.ErrorException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to receive price from Zaif.");
                    }


                    if (coin.MarketPrice == null) coin.MarketPrice = new Price(coin);

                    if (coin.Id is "bitcoin")
                    {
                        coin.MarketPrice.LatestPriceBTC = 1;
                        coin.MarketPrice.LatestPriceUSD = (double)response.Data.last / _USDJPYrate.Rate;
                        //coin.MarketPrice.PriceBTCBefore24h = 1;
                        //coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD; //tmp
                    }
                    else
                    {
                        var btcprice = AppCore.Bitcoin.MarketPrice;
                        if (btcprice != null)
                        {
                            coin.MarketPrice.LatestPriceUSD = (double)response.Data.last / _USDJPYrate.Rate;
                            coin.MarketPrice.LatestPriceBTC = coin.MarketPrice.LatestPriceUSD / btcprice.LatestPriceUSD;
                            //coin.MarketPrice.PriceBTCBefore24h = price_yesterday.LatestPriceBTC; //tmp
                            //coin.MarketPrice.PriceUSDBefore24h = price_yesterday.LatestPriceUSD; //tmp
                        }
                    }

                    coin.MarketPrice.DayVolume = (double)response.Data.volume * coin.MarketPrice.LatestPriceBTC;
                    coin.MarketPrice.PriceDate = DateTime.Now;
                    //coin.MarketPrice.USDCrossRate = _crossrate;
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchPriceAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }

        }

        public static async Task<List<Position>> FetchPositionAsync(Exchange zaif)
        {
            _zaif = zaif;

            try
            {
                var req = BuildRequest(AssetType.Cash, "POST", "get_info");
                var balance = await RestUtil.ExecuteRequestAsync<ZaifBalance>(_restClient, req);

                var positions = new List<Position>();
                foreach (var fund in balance.return_.funds)
                {
                    var instrumentId = _zaif.GetIdForExchange(fund.Key.ToUpper());
                    var coin = AppCore.InstrumentList.GetByInstrumentId(instrumentId);
                    if (coin != null)
                    {
                        var qty = (double)fund.Value;
                        if (qty > 0)
                        {
                            var pos = new Position(coin)
                            {
                                Amount = qty,
                                BookedExchange = _zaif
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

        public static async Task<TradeList> FetchTransactionAsync(Exchange zaif, int calendarYear = 0)
        {
            _zaif = zaif;
            _restClient.BaseUrl = new Uri(ApiRoot);
            TradeList tradelist = null;

            try
            {

                tradelist = await GetTradeHistoryAsync(calendarYear);
                tradelist.AddRange(await GetTradeHistoryAsync(calendarYear, "eth_jpy"));
                tradelist.AddRange(await GetTradeHistoryAsync(calendarYear, "bch_jpy"));
                tradelist.AddRange(await GetLeverageTradeHistoryAsync(AssetType.FX, calendarYear));
                tradelist.AddRange(await GetLeverageTradeHistoryAsync(AssetType.Margin, calendarYear));
                //tradelist.AddRange(await GetLeverageTradeHistoryAsync(AssetType.Futures, calendarYear, null, "4"));
                return tradelist;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchTransactionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        public static async Task<List<RealizedPL>> FetchLeveragePLAsync(Exchange zaif, int calendarYear = 0)
        {
            _zaif = zaif;

            try
            {
                var leveragePL = await GetLeveragePositionsAsync(AssetType.Margin, calendarYear);
                leveragePL.AddRange(await GetLeveragePositionsAsync(AssetType.FX, calendarYear));
                //leveragePL.AddRange(await GetLeveragePositionsAsync(AssetType.Futures, calendarYear, null, "4"));
                //leveragePL.AddRange(await GetLeveragePositionsAsync(AssetType.Futures, calendarYear, null, "5"));

                return leveragePL;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": FetchTransactionAsync: " + e.GetType() + ": " + e.Message);
                throw;
            }
        }

        private static async Task<TradeList> GetTradeHistoryAsync(int calendarYear = 0, string currency_pair = null)
        {
            EnuSide ebuysell;

            var since = calendarYear == 0 ? new DateTime(2012, 1, 1) : new DateTime(calendarYear, 1, 1);
            var end = calendarYear == 0 ? new DateTime(DateTime.Now.Year, 12, 31) : new DateTime(calendarYear, 12, 31);

            var tradelist = new TradeList() { SettlementCCY = EnuCCY.JPY, TradedExchange = _zaif };
            var trades_history = new Dictionary<string, ZaifTrades.trade>();
            int from_id = 0;
            int limit = 500;
            string order = "ASC";

            var results = await GetTradeHistoryPageAsync(
                                                         limit,
                                                         Util.ToEpochSeconds(since),
                                                         Util.ToEpochSeconds(end),
                                                         from_id,
                                                         order,
                                                         currency_pair);
            if (results.return_.Any())
            {
                while (true)
                {
                    foreach (var trade in results.return_)
                    {
                        var timestamp = Util.UnixTimeStampToDateTime(trade.Value.timestamp);
                        if (since < timestamp)
                            trades_history.Add(trade.Key, trade.Value);

                        if (since > timestamp)
                        {
                            continue;
                        }
                    }

                    if (results.return_.Count == 0 || limit > results.return_.Count)
                    {
                        break;
                    }

                    from_id += limit + 1;

                    results = await GetTradeHistoryPageAsync(
                                                             limit,
                                                             Util.ToEpochSeconds(since),
                                                             Util.ToEpochSeconds(end),
                                                             from_id,
                                                             order,
                                                             currency_pair);
                }

                foreach (var trade in trades_history)
                {

                    switch (trade.Value.your_action)
                    {
                        case "bid":
                            ebuysell = EnuSide.Buy;
                            break;
                        case "ask":
                            ebuysell = EnuSide.Sell;
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    var symbol = trade.Value.currency_pair;
                    EnuCCY settleccy;
                    if (symbol.Contains("_jpy"))
                    {
                        settleccy = EnuCCY.JPY;
                    }
                    else if (symbol.Contains("_btc"))
                    {
                        settleccy = EnuCCY.BTC;
                    }
                    else
                    {
                        continue;
                    }

                    symbol = symbol.Replace("_jpy", "").Replace("_btc", "").ToUpper();

                    var fee = trade.Value.fee_amount != null ? trade.Value.fee_amount : trade.Value.fee;

                    tradelist.AggregateTransaction(symbol,
                                                   AssetType.Cash,
                                                   ebuysell,
                                                   (decimal)trade.Value.amount,
                                                   (decimal)trade.Value.price,
                                                   settleccy,
                                                   Util.FromEpochSeconds((long)trade.Value.timestamp).Date,
                                                   (decimal)fee,
                                                   _zaif);
                }
            }

            return tradelist;
        }

        private static async Task<TradeList> GetLeverageTradeHistoryAsync(AssetType assetType, int calendarYear = 0, string currency_pair = null, string group_id = null)
        {
            EnuSide ebuysell;

            var since = calendarYear == 0 ? new DateTime(2012, 1, 1) : new DateTime(calendarYear, 1, 1);
            var end = calendarYear == 0 ? new DateTime(DateTime.Now.Year, 12, 31) : new DateTime(calendarYear, 12, 31);

            var tradelist = new TradeList() { SettlementCCY = EnuCCY.JPY, TradedExchange = _zaif };

            var leveragePositions = new Dictionary<string, ZaifPositions.position>();
            int from_id = 0;
            int limit = 500;
            string order = "ASC";

            var results = await GetLeveragePositionsPageAsync(assetType,
                                                              limit,
                                                              Util.ToEpochSeconds(since),
                                                              Util.ToEpochSeconds(end),
                                                              from_id,
                                                              order,
                                                              currency_pair,
                                                              group_id);
            while (true)
            {
                foreach (var p in results.return_)
                {
                    var timestamp = Util.UnixTimeStampToDateTime(p.Value.timestamp);
                    if (since < timestamp)
                        leveragePositions.Add(p.Key, p.Value);

                    if (since > timestamp)
                    {
                        continue;
                    }
                }

                if (results.return_.Count == 0 || limit > results.return_.Count)
                {
                    break;
                }

                from_id += limit + 1;

                results = await GetLeveragePositionsPageAsync(assetType,
                                                            limit,
                                                            Util.ToEpochSeconds(since),
                                                            Util.ToEpochSeconds(end),
                                                            from_id,
                                                            order,
                                                            currency_pair,
                                                             group_id);
            }

            foreach (var p in leveragePositions)
            {

                switch (p.Value.action)
                {
                    case "bid":
                        ebuysell = EnuSide.Buy;
                        break;
                    case "ask":
                        ebuysell = EnuSide.Sell;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                var symbol = p.Value.currency_pair;
                EnuCCY settleccy;
                if (symbol.Contains("_jpy"))
                {
                    settleccy = EnuCCY.JPY;
                }
                else if (symbol.Contains("_btc"))
                {
                    settleccy = EnuCCY.BTC;
                }
                else
                {
                    continue;
                }

                symbol = symbol.Replace("_jpy", "").Replace("_btc", "").ToUpper();

                if (p.Value.amount_done > 0)
                {
                    tradelist.AggregateTransaction(symbol,
                                                   assetType,
                                                   ebuysell,
                                                   (decimal)p.Value.amount_done,
                                                   (decimal)p.Value.price_avg,
                                                   settleccy,
                                                   Util.FromEpochSeconds((long)p.Value.timestamp).Date,
                                                   p.Value.fee_spent,
                                                   _zaif);
                }

                if (p.Value.close_done > 0)
                {
                    tradelist.AggregateTransaction(symbol,
                                                   assetType,
                                                   ebuysell == EnuSide.Buy ? EnuSide.Sell : EnuSide.Buy,
                                                   (decimal)p.Value.close_done,
                                                   (decimal)p.Value.close_avg,
                                                   settleccy,
                                                   Util.FromEpochSeconds((long)p.Value.timestamp_closed).Date,
                                                   p.Value.guard_fee - p.Value.swap,
                                                   _zaif);
                }
            }

            return tradelist;
        }

        private static async Task<List<RealizedPL>> GetLeveragePositionsAsync(AssetType assetType, int calendarYear = 0, string currency_pair = null, string group_id = null)
        {
            EnuSide ebuysell;
            EnuPLType pltype;
            var since = calendarYear == 0 ? new DateTime(2012, 1, 1) : new DateTime(calendarYear, 1, 1);
            var end = calendarYear == 0 ? new DateTime(DateTime.Now.Year, 12, 31) : new DateTime(calendarYear, 12, 31);

            var leveragePL = new List<RealizedPL>();
            var leveragePositions = new Dictionary<string, ZaifPositions.position>();
            int from_id = 0;
            int limit = 500;
            string order = "ASC";

            var results = await GetLeveragePositionsPageAsync(assetType,
                                                              limit,
                                                              Util.ToEpochSeconds(since),
                                                              Util.ToEpochSeconds(end),
                                                              from_id,
                                                              order,
                                                              currency_pair,
                                                              group_id);
            while (true)
            {
                foreach (var p in results.return_)
                {
                    var timestamp = Util.UnixTimeStampToDateTime(p.Value.timestamp);
                    if (since < timestamp)
                        leveragePositions.Add(p.Key, p.Value);

                    if (since > timestamp)
                    {
                        continue;
                    }
                }

                if (results.return_.Count == 0 || limit > results.return_.Count)
                {
                    break;
                }

                from_id += limit + 1;

                results = await GetLeveragePositionsPageAsync(assetType,
                                                            limit,
                                                            Util.ToEpochSeconds(since),
                                                            Util.ToEpochSeconds(end),
                                                            from_id,
                                                            order,
                                                            currency_pair,
                                                             group_id);
            }

            switch (assetType)
            {
                case AssetType.Cash:
                    throw new InvalidOperationException();

                case AssetType.Margin:
                    pltype = EnuPLType.MarginTrade;
                    break;

                case AssetType.FX:
                    pltype = EnuPLType.FXTrade;
                    break;

                case AssetType.Futures:
                    pltype = EnuPLType.FuturesTrade;
                    break;

                default:
                    throw new NotImplementedException();
            }

            foreach (var p in leveragePositions.Where(x => x.Value.close_done > 0))
            {
                var symbol = p.Value.currency_pair.Replace("_jpy", "").ToUpper();
                var id = _zaif.GetIdForExchange(symbol);

                switch (p.Value.action)
                {
                    case "bid":
                        ebuysell = EnuSide.Buy;
                        break;
                    case "ask":
                        ebuysell = EnuSide.Sell;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                var pl = new RealizedPL(
                    AppCore.InstrumentList.GetByInstrumentId(id),
                    pltype,
                    Util.FromEpochSeconds((long)p.Value.timestamp_closed),
                    ebuysell,
                    EnuBaseFiatCCY.JPY,
                    p.Value.close_done,
                    p.Value.price_avg,
                    p.Value.close_avg,
                    _zaif);

                pl.MarginFee = p.Value.fee_spent + p.Value.guard_fee;
                pl.Swap = p.Value.swap;
                leveragePL.Add(pl);
            }

            return leveragePL;
        }

        private static async Task<ZaifTrades> GetTradeHistoryPageAsync(int limit, long since, long end, int from = 0, string order = "DESC", string currency_pair = null)
        {
            var method = "POST";
            var zaifmethod = "trade_history";

            var body = $"count={limit}&order={order}";

            if (from != 0)
            {
                body += $"&from={from}";
            }
            else
            {
                body += $"&since={since}&end={end}";
            }

            if (currency_pair != null)
            {
                body += $"&currency_pair={currency_pair}";
            }

            var req = BuildRequest(AssetType.Cash, method, zaifmethod, body);
            var result = await RestUtil.ExecuteRequestAsync<ZaifTrades>(_restClient, req);
            if (result.success != 1)
            {
                throw new InvalidOperationException($"{zaifmethod} failed. message:{result.error}");
            }
            return result;
        }

        private static async Task<ZaifPositions> GetLeveragePositionsPageAsync(AssetType assetType, int limit, long since, long end, int from = 0, string order = "DESC", string currency_pair = null, string group_id = null)
        {
            var method = "POST";
            var zaifmethod = "get_positions";

            var body = $"count={limit}&order={order}";

            if (from != 0)
            {
                body += $"&from={from}";
            }
            else
            {
                body += $"&since={since}&end={end}";
            }

            if (currency_pair != null)
            {
                body += $"&currency_pair={currency_pair}";
            }

            var req = BuildRequest(assetType, method, zaifmethod, body, group_id);
            var result = await RestUtil.ExecuteRequestAsync<ZaifPositions>(_restClient, req);
            if (result.success != 1)
            {
                throw new InvalidOperationException($"{zaifmethod} failed. message:{result.error}");
            }
            return result;
        }

        private static RestRequest BuildRequest(AssetType assettype, string method, string zaifmethod, string body = "", string group_id = null)
        {
            var nonce = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            var message = $"nonce={nonce}&method={zaifmethod}";
            if (body != "") message += $"&{body}";
            string path;

            switch (assettype)
            {
                case AssetType.Cash:
                    path = "/tapi";
                    break;
                case AssetType.Margin:
                    path = "/tlapi";
                    message += $"&type=margin";
                    break;
                case AssetType.FX:
                    path = "/tlapi";
                    message += $"&type=futures&group_id=1";
                    break;
                case AssetType.Futures:
                    path = "/tlapi";
                    //var group_id = "4"; // "5"
                    message += $"&type=futures&group_id={group_id}";
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var sign = Util.SHA512NewHmac(_zaif.Secret, message);
            var req = RestUtil.CreateJsonRestRequest(path, false);
            req.Method = Util.ParseEnum<Method>(method);

            req.AddParameter("application/x-www-form-urlencoded", message, ParameterType.RequestBody);
            req.AddHeader("key", _zaif.Key);
            req.AddHeader("sign", sign);
            return req;
        }
    }
}