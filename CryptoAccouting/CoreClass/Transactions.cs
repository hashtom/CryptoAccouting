using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Collections.ObjectModel;

namespace CryptoAccouting.CoreClass
{
    public class Transactions : IEnumerable<Transaction>
    {
        private List<Transaction> txs;
        public List<Transaction> TransactionCollection
        {
            get { return txs; }
            set { this.txs = value; }
        }
		public double TotalAmountBougt { get; set; }
		public double TotalAmountSold { get; set; }
		public double TotalAmountOutstanding { get; set; }
        public double BookPrice { get; set; } 
		//public double RealizedPL { get; set; }
		//public double UnrealizedPL { get; set; }
        public Exchange TradedExchange { get; set; }
        private List<RealizedPL> RealizedPLHistory;

        public Transactions()
        {
            txs = new List<Transaction>();
            RealizedPLHistory = new List<CoreClass.RealizedPL>();
        }


		public void ReEvaluate()
		{
            TotalAmountBougt = txs.Where(t => t.BuySell == EnuBuySell.Buy).Sum(t => t.Amount);
            TotalAmountSold = txs.Where(t => t.BuySell == EnuBuySell.Sell).Sum(t => t.Amount);
            TotalAmountOutstanding = TotalAmountBougt - TotalAmountSold;

		}

        public void AggregateTransaction(Instrument coin, EnuExchangeType exType, EnuBuySell buysell, double amount,
                                         double tradePrice, DateTime tradeDate, EnuTxAggregateFlag flag = EnuTxAggregateFlag.Daliy)
        {
            Transaction tx;

            if (txs.Any(t => (t.CoinName == coin.Symbol && t.BuySell == buysell && t.TradeDate.Date == tradeDate.Date)))
            {
                tx = txs.Where(t => (t.Coin == coin && t.BuySell == buysell && t.TradeDate.Date == tradeDate.Date)).First();
                tx.Amount += amount;
                tx.TradePrice = ((amount * tradePrice) + tx.TradeValue) / tx.Amount;
                tx.UpdateTime = DateTime.Now;
            }
            else
            {
                tx = new Transaction(coin, EnuExchangeType.Zaif);
                tx.TxId = (txs.Count + 1).ToString();
                tx.BuySell = buysell;
                tx.Amount = amount;
                tx.TradePrice = tradePrice;
                tx.TradeDate = tradeDate;
                this.AttachTransaction(tx);
            }

			//昇順（過去->現在）への順番で取引がコールされる前提
            this.updatePL(tx);

        }

        private void updatePL(Transaction tx)  // this must be called by ascending transaction date order
        {

            if (tx.BuySell == EnuBuySell.Sell)
            {
                // SELL: attach PL instanse 
                if (!txs.Where(t => t.TxId == tx.TxId).Any())//重複PLオブジェクトがないか事前にチェック！
				{
                    var pl = new RealizedPL(tx);
                    pl.BaseCCY = AppConfig.BaseCurrency;
                    pl.BookPrice = this.BookPrice;
                    RealizedPLHistory.Add(pl);
                }
            }
            else
            {
                //BUY: Update Average Book Price
                var amounttd = OutstandingAmountToToDate(tx);
                var bookpricetd = BookPriceToToDate(tx);
                BookPrice = (amounttd * bookpricetd + tx.Amount * tx.TradePrice) / (amounttd + tx.Amount);
            }

        }

        public double OutstandingAmountToToDate(Transaction tx){
            if (txs.Count == 0) {
                return 0;
            }
            else
            {
                return txs.Where(t => (t.TradeDate < tx.TradeDate) && t.BuySell == EnuBuySell.Buy).Sum(t => t.Amount) -
                           txs.Where(t => (t.TradeDate < tx.TradeDate) && t.BuySell == EnuBuySell.Sell).Sum(t => t.Amount);
            }
        }

        private double BookPriceToToDate(Transaction tx){
            if (RealizedPLHistory.Count == 0)
            {
                return tx.TradePrice;
            }
            else
            {
                RealizedPLHistory.Where(p => (p.TradeDate() <= tx.TradeDate)).OrderBy(p => p.TradeDate());
                var maxdate = RealizedPLHistory.Where(p => (p.TradeDate() <= tx.TradeDate)).Max(t=>t.TradeDate());
                return RealizedPLHistory.Where(p => p.TradeDate() == maxdate).First().BookPrice;
            }
		}

        public double RealizedPL(){
            if (RealizedPLHistory.Count == 0)
            {
                return 0;
            }
            else
            {
                return RealizedPLHistory.Sum(p => p.PLValue());
            }
        }

        public double UnrealizedPL(){

            return TotalAmountOutstanding * (290000 - BookPrice);
        }

        public void AttachTransaction(Transaction tx)
        {
            if (txs.Any(x => x.TxId == tx.TxId)) DetachPosition(tx);
            txs.Add(tx);
        }

        public void DetachPosition(Transaction tx)
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
