using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CoinBalance.CoreModel
{
    public class TradeList : IEnumerable<Transaction>
    {
        public List<string> TradedCoin { get; private set; }
        //public int TradeYear { get; private set; }
        public string TradedCoinString { get; set; }
        public EnuCCY SettlementCCY { get; set; } // Fiat Only
        public double UnrealizedBookValue { get; set; }
        public double AverageBookPrice { get; set; }
        public Exchange TradedExchange { get; set; }

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

        public double TotalBTCTradeValueBuy
        {
            get
            {
                return transactions.Where(t => t.Side == EnuSide.Buy).
                                   Where(t => t.SettlementCCY == EnuCCY.BTC).
                                   Sum(t => t.TradeNetValue);
            }
        }

        public double TotalBTCTradeValueSell
        {
            get
            {
                return transactions.Where(t => t.Side == EnuSide.Sell).
                                 Where(t => t.SettlementCCY == EnuCCY.BTC).
                                 Sum(t => t.TradeNetValue);
            }
        }

        public double TotalExchangeSettleTradeValueBuy
        {
            get
            {
                return transactions.Where(t => t.Side == EnuSide.Buy).
                               Where(t => t.SettlementCCY == SettlementCCY).
                               Sum(t => t.TradeNetValue);
            }
        }

        public double TotalExchangeSettleTradeValueSell
        {
            get
            {
                return transactions.Where(t => t.Side == EnuSide.Sell).
                               Where(t => t.SettlementCCY == SettlementCCY).
                               Sum(t => t.TradeNetValue);
            }
        }

        public double RealizedBookValue
        {
            get
            {
                return transactions.Where(t => t.Side == EnuSide.Sell).
                                   Sum(t => t.RealizedBookValue);
            }
        }

        private List<Transaction> transactions;

        public List<Transaction> TransactionCollection
        {
            get { return transactions; }
            set { this.transactions = value; }
        }

        public TradeList()
        {
            transactions = new List<Transaction>();
            //this.SettlementCCY = settlementCCY;
        }

        public double TotalQuantity(string instrumentId, EnuSide side)
        {
            return transactions.Where(x => x.TradedCoin.Id == instrumentId).Where(x => x.Side == side).Sum(x => x.Quantity);
        }

        public double TotalNetValue(string instrumentId, EnuSide side)
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
                                         double qty,
                                         double tradePrice,
                                         EnuCCY settleCcy,
                                         DateTime tradeDate,
                                         double fee,
                                         Exchange exchange)
        {

            Transaction tx;
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
                    tx = new Transaction(AppCore.InstrumentList.GetByInstrumentId(instrumentId), exchange);
                    tx.TxId = (transactions.Count + 1);
                    tx.Type = type;
                    tx.Side = side;
                    tx.SettlementCCY = settleCcy;
                    tx.Quantity = qty;
                    tx.TradePriceSettle = tradePrice;
                    tx.TradeDate = tradeDate;
                    tx.Fee = fee;
                    tx.NumTransaction = 1;
                    this.Attach(tx);
                }
            }
        }

        private void CalculatePL()
        {
			TradedCoin = new List<string>();
            TradedCoinString = "";

            foreach (var s in transactions.Select(x => x.ColumnCoinSymbol).Distinct())
            {
                string symbol = s;
                double accumulated_value = 0;
                double accumulated_qty = 0;
                double current_bookprice = 0;
                //UnrealizedBookValue = 0;

                TradedCoin.Add(symbol);
                TradedCoinString += symbol + " ";

                foreach (var tx in transactions.Where(t => t.ColumnCoinSymbol == symbol).OrderBy(t => t.TradeDate))
                {
                    if (tx.Side == EnuSide.Buy)
                    {
                        //Buy : Update Bookcost
                        current_bookprice = (accumulated_value + tx.TradeNetValue) / (accumulated_qty + tx.Quantity);
                        tx.BookPrice = current_bookprice;
                        accumulated_value += tx.TradeNetValue;
                        accumulated_qty += tx.Quantity;
                    }
                    else if (tx.Side == EnuSide.Sell)
                    {
                        //Sell : Reduce Accumulated value
                        accumulated_value -= tx.TradeNetValue;
                        accumulated_qty -= tx.Quantity;
                        tx.BookPrice = current_bookprice;
                    }
                }
            }
        }

        //public void SwitchTrdeYear(int year)
        //{
        //    CalculateTotalValue(year);
        //}

        //public int TxCountTradeYear(){
        //    return transactions.Where(x => x.TradeDate.Year == this.TradeYear).Count();
        //}

        public double RealizedPL()
        {
			// calculate Realized PL when one side of trase is Base Fiat Currency
			// ignore trades both sides are Crypto for Realized PL calculation now
            //return transactions.Where(x=>x.TradeDate.Year == this.TradeYear).Sum(x => x.RealizedPL);
            return transactions.Sum(x => x.RealizedPL);
        }

        public double UnrealizedPL(){
            return 0;
            //return (TotalQtyBuy - TotalQtySell) * (2300000 - LatestBookCost);
        }

        public void Attach(Transaction tx)
        {
            if (transactions.Any(x => x.TxId == tx.TxId)) Detach(tx);
            transactions.Add(tx);
        }

        public void Detach(Transaction tx)
        {
            transactions.RemoveAll(x => x.TxId == tx.TxId);
        }

        public Transaction GetByIndex(int indexNumber)
        {
            return transactions[indexNumber];
        }

        public int Count()
        {
            return transactions.Count;
        }

        public void AddRange(TradeList tradelist)
        {
            transactions.AddRange(tradelist);
        }

		public IEnumerator<Transaction> GetEnumerator()
		{
			for (int i = 0; i <= transactions.Count - 1; i++) yield return transactions[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
                                         
    }

}
