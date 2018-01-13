using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CoinBalance.CoreModel
{
    public class TradeList : IEnumerable<Trade>
    {
        //public List<string> TradedCoin { get; private set; }
        //public string TradedCoinString { get; set; }
        public EnuCCY SettlementCCY { get; set; } // Fiat Only
        public Exchange TradedExchange { get; set; }
        private List<Trade> transactions;

        public List<Trade> TransactionCollection
        {
            get { return transactions; }
            set { this.transactions = value; }
        }

        public double NumOrdersBuy
        {
            get
            {
                return transactions.Where(t => t.Side == EnuSide.Buy).Sum(t => t.NumTransaction);
            }
        }

        public double NumOrdersSell
        {
            get
            {
                return transactions.Where(t => t.Side == EnuSide.Sell).Sum(t => t.NumTransaction);
            }
        }

        public decimal TotalBTCTradeValueBuy
        {
            get
            {
                return transactions.Where(t => t.Side == EnuSide.Buy).
                                   Where(t => t.SettlementCCY == EnuCCY.BTC).
                                   Sum(t => t.TradeNetValue);
            }
        }

        public decimal TotalBTCTradeValueSell
        {
            get
            {
                return transactions.Where(t => t.Side == EnuSide.Sell).
                                 Where(t => t.SettlementCCY == EnuCCY.BTC).
                                 Sum(t => t.TradeNetValue);
            }
        }

        public decimal TotalExchangeSettleTradeValueBuy
        {
            get
            {
                return transactions.Where(t => t.Side == EnuSide.Buy).
                               Where(t => t.SettlementCCY == SettlementCCY).
                               Sum(t => t.TradeNetValue);
            }
        }

        public decimal TotalExchangeSettleTradeValueSell
        {
            get
            {
                return transactions.Where(t => t.Side == EnuSide.Sell).
                               Where(t => t.SettlementCCY == SettlementCCY).
                               Sum(t => t.TradeNetValue);
            }
        }

        public TradeList()
        {
            transactions = new List<Trade>();
        }

        public decimal TotalQuantity(string instrumentId, EnuSide side)
        {
            return transactions.Where(x => x.TradedCoin.Id == instrumentId).Where(x => x.Side == side).Sum(x => x.Quantity);
        }

        public decimal TotalNetValue(string instrumentId, EnuSide side)
        {
            return transactions.Where(x => x.TradedCoin.Id == instrumentId).Where(x => x.Side == side).Sum(x => x.TradeNetValue);
        }

        public string ExchangeName()
        {
            return TradedExchange != null ? TradedExchange.Name : null;
        }

        public void AggregateTransaction(string symbol,
                                         AssetType type,
                                         EnuSide side,
                                         decimal qty,
                                         decimal tradePrice,
                                         EnuCCY settleCcy,
                                         DateTime tradeDate,
                                         decimal fee,
                                         Exchange exchange)
        {

            Trade tx;
            var instrumentId = exchange.GetIdForExchange(symbol);
            if (instrumentId != null)
            {

                if (transactions.Any(t => (t.CoinId == instrumentId && t.Type == type && t.Side == side && t.TradeDate.Date == tradeDate.Date && t.SettlementCCY == settleCcy)))
                {
                    tx = transactions.Single(t => (t.CoinId == instrumentId && t.Type == type && t.Side == side && t.TradeDate.Date == tradeDate.Date && t.SettlementCCY == settleCcy));

                    var newqty = tx.Quantity + qty;

                    tx.TradePriceSettle = (tx.TradePriceSettle * tx.Quantity + tradePrice * qty) / newqty;
                    tx.Quantity = newqty;
                    tx.Fee += fee;
                    tx.NumTransaction++;
                }
                else
                {
                    tx = new Trade(AppCore.InstrumentList.GetByInstrumentId(instrumentId), exchange);
                    tx.TxId = (transactions.Count + 1);
                    tx.Type = type;
                    tx.Side = side;
                    tx.SettlementCCY = settleCcy;
                    tx.Quantity = qty;
                    tx.TradePriceSettle = tradePrice;
                    tx.TradeDate = tradeDate;
                    tx.Fee = fee;
                    tx.NumTransaction = 1;
                    this.AttachTrade(tx);
                }
            }
        }

        // calculate Realized PL when settlement currency is Base Fiat Currency
        // ignore trades both sides are Crypto for Realized PL calculation now
        public List<RealizedPL> CalculateTradesPL(AssetType assetType = AssetType.Cash)
        {
            string status;
            var pls = new List<RealizedPL>();
            var pltype = new EnuPLType();

            switch (assetType)
            {
                case AssetType.Cash:
                    pltype = EnuPLType.CashTrade;
                    break;
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

            foreach (var s in transactions.Select(x => x.CoinId).Distinct())
            {
                EnuSide side_closed;
                decimal balance = 0;
                decimal accumulated_value = 0;
                decimal accumulated_qty = 0;
                decimal current_bookprice = 0;

                foreach (var tx in transactions.
                         Where(t => t.CoinId == s).OrderBy(t => t.TradeDate).
                         Where(t=> t.SettlementCCY == CoreAPI.Util.ParseEnum<EnuCCY>(AppCore.BaseCurrency.ToString())).
                         Where(t=>t.Type == assetType))
                {
                    
                    if (assetType == AssetType.Cash)
                    {
                        status = tx.Side == EnuSide.Buy ? "open" : "closed";
                    }
                    else
                    {
                        if (balance > 0)
                        {
                            status = tx.Side == EnuSide.Buy ? "open" : "closed";
                        }
                        else if (balance < 0)
                        {
                            status = tx.Side == EnuSide.Buy ? "closed" : "open";
                        }
                        else
                        {
                            status = "open";
                        }
                    }

                    if (status == "open")
                    {
                        //Buy : Update Bookcost
                        current_bookprice = (accumulated_value + tx.TradeNetValue) / (accumulated_qty + tx.Quantity);
                        accumulated_value += tx.TradeNetValue;
                        accumulated_qty += tx.Quantity;
                        balance += tx.Side == EnuSide.Buy ? tx.Quantity : -tx.Quantity;
                    }
                    else if (status == "closed" )
                    {
                        
                        if (assetType == AssetType.Cash)
                        {
                            if (accumulated_qty < tx.Quantity)
                            {
                                continue;
                            }

                            side_closed = tx.Side;
                        }
                        else
                        {
                            side_closed = tx.Side == EnuSide.Buy ? EnuSide.Sell : EnuSide.Buy;
                        }

                        var pl = new RealizedPL(tx.TradedCoin, pltype, tx.TradeDate, side_closed, AppCore.BaseCurrency,
                                                tx.Quantity, current_bookprice, tx.TradePriceSettle, this.TradedExchange)
                        {
                            TradeFee = tx.Fee,
                            MarginFee = 0,
                            Swap = 0,
                            DepWithFee = 0
                        };

                        pls.Add(pl);

                        //Sell : Reduce Accumulated value
                        accumulated_value -= tx.Quantity * current_bookprice;
                        accumulated_qty -= tx.Quantity;
                        balance += tx.Side == EnuSide.Buy ? tx.Quantity : -tx.Quantity;
                    }
                }
            }

            return pls;
        }

        public void AttachTrade(Trade tx)
        {
            if (transactions.Any(x => x.TxId == tx.TxId)) DetachTrade(tx);
            transactions.Add(tx);
        }

        public void DetachTrade(Trade tx)
        {
            transactions.RemoveAll(x => x.TxId == tx.TxId);
        }

        public Trade GetTradeByIndex(int indexNumber)
        {
            return transactions[indexNumber];
        }

        public int TradeCount()
        {
            return transactions.Count;
        }

        public void AddRange(TradeList tradelist)
        {
            transactions.AddRange(tradelist);
        }

        public IEnumerator<Trade> GetEnumerator()
        {
            for (int i = 0; i <= transactions.Count - 1; i++) yield return transactions[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
                                         
    }

}
