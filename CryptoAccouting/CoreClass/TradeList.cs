using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CryptoAccouting.UIClass;
//using System.Collections.ObjectModel;

namespace CryptoAccouting.CoreClass
{
    public class TradeList : IEnumerable<Transaction>
    {
        public Instrument TradedCoin { get; set; } // Per single coin
        public string TradeYear { get; set; }
        public EnuCCY CCY_Valution { get; set; } // Fiat Only
		public double TotalQtyBuy { get; set; }
		public double TotalQtySell { get; set; }
        public int TxCountBuy { get; set; }
        public int TxCountSell { get; set; }
        public double TotalValueBuy { get; set;}
        public double TotalValueSell { get; set; }
		public double RealizedBookValue { get; set; }
        public double UnrealizedBookValue { get; set; } 
        public double AverageBookPrice { get; set; }
		public EnuExchangeType TradedExchange { get; set; }

        private List<Transaction> transactions;

		public List<Transaction> TransactionCollection
		{
			get { return transactions; }
			set { this.transactions = value; }
		}

        public TradeList(EnuCCY cur_valuation, string symbol)
        {
            transactions = new List<Transaction>();
            this.CCY_Valution = cur_valuation;
            this.TradedCoin = ApplicationCore.GetInstrument(symbol);
        }

        public void ReEvaluate()
		{
            TotalQtyBuy = transactions.Where(t => t.BuySell == EnuBuySell.Buy).Sum(t => t.Quantity);
            TotalQtySell = transactions.Where(t => t.BuySell == EnuBuySell.Sell).Sum(t => t.Quantity);
            TxCountBuy = transactions.Where(t => t.BuySell == EnuBuySell.Buy).Count();
            TxCountSell = transactions.Where(t => t.BuySell == EnuBuySell.Sell).Count();
            TotalValueBuy = transactions.Where(t => t.BuySell == EnuBuySell.Buy).Sum(t => t.TradeValue);
            TotalValueSell = transactions.Where(t => t.BuySell == EnuBuySell.Sell).Sum(t => t.TradeValue);
            CalculatePL();
		}

        public void AggregateTransaction(Instrument coin, EnuExchangeType exType, EnuBuySell buysell, double qty, double tradePrice,
                                         DateTime tradeDate, int fee, EnuTxAggregateFlag flag = EnuTxAggregateFlag.Daliy)
        {
            Transaction tx;

            if (transactions.Any(t => (t.TradecCoinSymbol == coin.Symbol && t.BuySell == buysell && t.TradeDate.Date == tradeDate.Date)))
            {
                tx = transactions.Single(t => (t.TradecCoinSymbol == coin.Symbol && t.BuySell == buysell && t.TradeDate.Date == tradeDate.Date));

                double newqty;
                newqty = tx.Quantity + qty;

                tx.TradePrice = (tx.TradePrice * tx.Quantity + tradePrice * qty) / newqty;
                tx.Quantity = newqty;
                tx.Fee += fee;
            }
            else
            {
                tx = new Transaction(coin,ApplicationCore.GetExchange(exType));
                tx.TxId = (transactions.Count + 1).ToString();
                tx.BuySell = buysell;
                tx.Quantity = qty;
                tx.TradePrice = tradePrice;
                tx.TradeDate = tradeDate;
                tx.Fee = fee;
                this.AttachTransaction(tx);
            }

        }

        private void CalculatePL()
        {
			double accumulated_value = 0;
            double accumulated_qty = 0;
			double current_bookprice = 0;
            RealizedBookValue = 0;
            UnrealizedBookValue = 0;

            foreach (var tx in transactions.OrderBy(t=>t.TradeDate))
            {

                if (tx.BuySell == EnuBuySell.Buy)
				{
					//Buy : Update Bookcost
					current_bookprice = (accumulated_value + tx.TradeValueGrossCost) / (accumulated_qty + tx.Quantity);
					tx.BookPrice = current_bookprice;
					accumulated_value += tx.TradeValueGrossCost;
                    accumulated_qty += tx.Quantity;
				}
                else if (tx.BuySell == EnuBuySell.Sell)
                {
                    //Sell : Reduce Accumulated value
                    accumulated_value -= tx.TradeValueGrossCost;
                    accumulated_qty -= tx.Quantity;
                    tx.BookPrice = current_bookprice;
                    RealizedBookValue += (tx.Quantity * tx.BookPrice);
                }

            }
            this.AverageBookPrice = current_bookprice;

        }

        public double RealizedPL()
        {
			// calculate Realized PL when one side of trase is Base Fiat Currency
			// ignore trades both sides are Crypto for Realized PL calculation 
			return transactions.Where(x => x.BuySell == EnuBuySell.Sell).Count() == 0 ? 0 : transactions.Sum(x => x.RealizedPL);
        }

        public double UnrealizedPL(){
            return 0;
            //return (TotalQtyBuy - TotalQtySell) * (2300000 - LatestBookCost);
        }

        public void AttachTransaction(Transaction tx)
        {
            if (transactions.Any(x => x.TxId == tx.TxId)) DetachTransaction(tx);
            transactions.Add(tx);
        }

        public void DetachTransaction(Transaction tx)
        {
            transactions.RemoveAll(x => x.TxId == tx.TxId);
        }

        public Transaction GetTransactionByIndex(int indexNumber)
        {
            return transactions[indexNumber];
        }

        public int Count()
        {
            return transactions.Count;
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

	public enum EnuTxAggregateFlag
	{
		Daliy,
		Weekly,
		Monthly
	}
}
