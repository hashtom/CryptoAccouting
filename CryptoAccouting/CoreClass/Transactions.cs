﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace CryptoAccouting
{
    public class Transactions : IEnumerable<Transaction>
    {

        private List<Transaction> txs;
        public List<Transaction> TransactionCollection
        {
            get { return txs; }
            set { this.txs = value; }
        }

        public Transactions()
        {
            txs = new List<Transaction>();
            //this.GenerateTestTxs();
        }

        //Test
        private void GenerateTestTxs(){

   //         txs.Add(new Transaction(new Instrument() { Symbol = "BTC" }, new Exchange(EnuExchangeType.Zaif))
			//{
			//	Amount = 1000,
			//	BuySell = "Buy",
			//	TradeDate = DateTime.Now,
			//	txid = "A",
			//	TradePrice = 58000
			//});
			//txs.Add(new Transaction(new Instrument() { Symbol = "BTC" }, new Exchange(EnuExchangeType.Zaif))
			//{
			//	Amount = 2000,
			//	BuySell = "Buy",
			//	TradeDate = DateTime.Now,
			//	txid = "B",
			//	TradePrice = 34000
			//});
			//txs.Add(new Transaction(new Instrument() { Symbol = "BTC" }, new Exchange(EnuExchangeType.Zaif))
			//{
			//	Amount = 3000,
			//	BuySell = "Buy",
			//	TradeDate = DateTime.Now,
			//	txid = "C",
			//	TradePrice = 130500
			//});

        }

        public void AggregateTransaction(Instrument coin, EnuExchangeType exType, string buysell, double amount,
                                         double tradePrice, DateTime tradeDate, EnuTxAggregateFlag flag = EnuTxAggregateFlag.Daliy ){

            if (txs.Any(t => (t.CoinName == coin.Symbol && t.BuySell == buysell && t.TradeDate.Date == tradeDate.Date)))
            {
                var tx = txs.Where(t => (t.Coin == coin && t.BuySell == buysell && t.TradeDate.Date == tradeDate.Date)).First();
                tx.Amount += amount;
                tx.TradePrice = ((amount * tradePrice) + tx.TradeValue) / tx.Amount;
                tx.UpdateTime = DateTime.Now;
            }
            else
            {
				var tx = new Transaction(coin, EnuExchangeType.Zaif);
				tx.txid = (txs.Count + 1).ToString();
				tx.BuySell = buysell;
				tx.Amount = amount;
				tx.TradePrice = tradePrice;
				tx.TradeDate = tradeDate;
				this.AttachTransaction(tx);
            }
        }

        public void AttachTransaction(Transaction tx)
        {
            if (txs.Any(x => x.txid == tx.txid)) DetachPosition(tx);
            txs.Add(tx);
        }

        public void DetachPosition(Transaction tx)
        {
            txs.RemoveAll(x => x.txid == tx.txid);
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

		public enum EnuTxAggregateFlag
		{
			Daliy,
			Weekly,
            Monthly
		}
    }
}
