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
        public string TradeYear { get; set; }
        public EnuCCY BaseCurrency { get; set; }
        public EnuCrypto BaseCrypto { get; set; }
		public double TotalQtyBuy { get; set; }
		public double TotalQtySell { get; set; }
        public int TxCountBuy { get; set; }
        public int TxCountSell { get; set; }
        public double TotalValueBuy { get; set;}
        public double TotalValueSell { get; set; }
		public double LatestBookCost { get; set; }
		public EnuExchangeType TradedExchange { get; set; }

        private List<Transaction> txs;
        //private List<ProfitLoss> RealizedPLHistory;

		public List<Transaction> TransactionCollection
		{
			get { return txs; }
			set { this.txs = value; }
		}

        public TradeList(EnuCCY baseCurrency, EnuCrypto baseCrypto)
        {
            txs = new List<Transaction>();
            BaseCurrency = baseCurrency;
            BaseCrypto = baseCrypto;
        }

        public void ReEvaluate()
		{
            TotalQtyBuy = txs.Where(t => t.BuySell == EnuBuySell.Buy).Sum(t => t.Quantity);
            TotalQtySell = txs.Where(t => t.BuySell == EnuBuySell.Sell).Sum(t => t.Quantity);
            TxCountBuy = txs.Where(t => t.BuySell == EnuBuySell.Buy).Count();
            TxCountSell = txs.Where(t => t.BuySell == EnuBuySell.Sell).Count();
            TotalValueBuy = txs.Where(t => t.BuySell == EnuBuySell.Buy).Sum(t => t.Quantity * t.TradePrice);
            TotalValueSell = txs.Where(t => t.BuySell == EnuBuySell.Sell).Sum(t => t.Quantity * t.TradePrice);
            calculatePL();
		}

        public void AggregateTransaction(Instrument coin, EnuExchangeType exType, EnuBuySell buysell, int qty, double tradePrice,
                                         DateTime tradeDate, int fee, EnuTxAggregateFlag flag = EnuTxAggregateFlag.Daliy)
        {
            Transaction tx;

            if (txs.Any(t => (t.Symbol == coin.Symbol && t.BuySell == buysell && t.TradeDate.Date == tradeDate.Date)))
            {
                tx = txs.Where(t => (t.Symbol == coin.Symbol && t.BuySell == buysell && t.TradeDate.Date == tradeDate.Date)).First();

                int newqty;
                newqty = tx.Quantity + qty;

                tx.TradePrice = (tx.TradePrice * tx.Quantity + tradePrice * qty) / newqty;
                tx.Quantity = newqty;
                tx.Fee += fee;
                //tx.UpdateTime = DateTime.Now;
            }
            else
            {
                tx = new Transaction(coin,ApplicationCore.GetExchange(exType));
                tx.TxId = (txs.Count + 1).ToString();
                tx.BuySell = buysell;
                tx.Quantity = qty;
                tx.TradePrice = tradePrice;
                tx.TradeDate = tradeDate;
                tx.Fee = fee;
                this.AttachTransaction(tx);
            }

            //昇順（過去->現在）への順番で取引がコールされる前提
            //this.updatePL(tx);

        }

        private void calculatePL()
        {
			double accumulated_value = 0;
			double current_bookprice = 0;

            foreach (var tx in txs.OrderBy(t=>t.TradeDate))
            {

                if (tx.BuySell == EnuBuySell.Sell)
                {
                    //Sell : Reduce Accumulated trade value
                    accumulated_value -= (tx.Quantity * tx.TradePrice + tx.Fee);
                }
                else if (tx.BuySell == EnuBuySell.Buy)
                {
                    current_bookprice = (accumulated_value + (tx.Quantity * tx.TradePrice - tx.Fee)) / accumulated_value * current_bookprice;
					tx.BookPrice = current_bookprice;
                    accumulated_value += (tx.Quantity * tx.TradePrice - tx.Fee);
                }

            }

            this.LatestBookCost = current_bookprice; //直近のBookPrice

        }

        public double RealizedPL()
        {
			// calculate Realized PL when one side of trase is Base Fiat Currency
			// ignore trades both sides are Crypto for Realized PL calculation 
			return txs.Where(x => x.BuySell == EnuBuySell.Sell).Count() == 0 ? 0 : txs.Sum(x => x.RealizedPLBase);
        }

        public double UnrealizedPL(){

            return (TotalQtyBuy - TotalQtySell) * (2300000 - LatestBookCost);
        }

        public void AttachTransaction(Transaction tx)
        {
            if (txs.Any(x => x.TxId == tx.TxId)) DetachTransaction(tx);
            txs.Add(tx);
        }

        public void DetachTransaction(Transaction tx)
        {
            txs.RemoveAll(x => x.TxId == tx.TxId);
        }

        public Transaction GetTransactionByIndex(int indexNumber)
        {
            return txs[indexNumber];
        }

        public int Count()
        {
            return txs.Count;
        }

		public IEnumerator<Transaction> GetEnumerator()
		{
			for (int i = 0; i <= txs.Count - 1; i++) yield return txs[i];
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
