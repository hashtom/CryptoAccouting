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
        public EnuCrypt BaseCrypt { get; set; }
		public double TotalQtyBuy { get; set; }
		public double TotalQtySell { get; set; }
        public int TxCountBuy { get; set; }
        public int TxCountSell { get; set; }
        public double TotalValueBuy { get; set;}
        public double TotalValueSell { get; set; }
		public double BookPrice { get; set; }
		public EnuExchangeType TradedExchange { get; set; }
		
        private List<Transaction> txs;
        private List<ProfitLoss> RealizedPLHistory;

		public List<Transaction> TransactionCollection
		{
			get { return txs; }
			set { this.txs = value; }
		}

        public TradeList()
        {
            txs = new List<Transaction>();
            RealizedPLHistory = new List<CoreClass.ProfitLoss>();
        }


        public void ReEvaluate(EnuCCY baseFiat = EnuCCY.JPY, EnuCrypt baseCrypt = EnuCrypt.BTC)
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
                tx = new Transaction(coin, EnuExchangeType.Zaif);
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
            double cur_bookprice = 0;
            double out_amount = 0;

            foreach (var tx in txs.OrderBy(t=>t.TradeDate))
            {
                if (tx.BuySell == EnuBuySell.Sell)
                {
                    // SELL: attach PL instanse 
                    if (!RealizedPLHistory.Where(p => p.TxID() == tx.TxId).Any())//重複PLオブジェクトがないか事前にチェック！
                    {
                        var pl = new ProfitLoss(tx);
                        pl.BaseCCY = AppSetting.BaseCurrency;
                        pl.BookPrice = cur_bookprice;
                        RealizedPLHistory.Add(pl);
                    }
                }
                else if (tx.BuySell == EnuBuySell.Buy)
                {
                    //BUY: Update Average Book Price
                    var newqty = out_amount + tx.Quantity;
                    cur_bookprice = (out_amount * cur_bookprice + (tx.Quantity * tx.TradePrice - tx.Fee)) / newqty;
                    out_amount = newqty;
                }

            }

            this.BookPrice = cur_bookprice; //直近のBookPrice

        }

  //      public double OutstandingAmountToToDate(Transaction tx){
  //          if (txs.Count == 0) {
  //              return 0;
  //          }
  //          else
  //          {
  //              return txs.Where(t => (t.TradeDate < tx.TradeDate) && t.BuySell == EnuBuySell.Buy).Sum(t => t.Quantity) -
  //                         txs.Where(t => (t.TradeDate < tx.TradeDate) && t.BuySell == EnuBuySell.Sell).Sum(t => t.Quantity);
  //          }
  //      }

  //      private double BookPriceToToDate(Transaction tx){
  //          if (RealizedPLHistory.Count == 0)
  //          {
  //              return tx.TradePrice;
  //          }
  //          else
  //          {
  //              RealizedPLHistory.Where(p => (p.TradeDate() <= tx.TradeDate)).OrderBy(p => p.TradeDate());
  //              var maxdate = RealizedPLHistory.Where(p => (p.TradeDate() <= tx.TradeDate)).Max(t=>t.TradeDate());
  //              return RealizedPLHistory.Where(p => p.TradeDate() == maxdate).First().BookPrice;
  //          }
		//}

        public double RealizedPL(){
            if (RealizedPLHistory.Count == 0)
            {
                return 0;
            }
            else
            {
                return RealizedPLHistory.Sum(p => p.RealizedPL());
            }
        }

        public double UnrealizedPL(){

            return (TotalQtyBuy + TotalQtySell) * (2900000 - BookPrice);
        }

  //      public int CountBuy(){

  //          return txs.Where(t => t.BuySell == EnuBuySell.Buy).Count();
  //      }

		//public int CountSell()
		//{
  //          return txs.Where(t => t.BuySell == EnuBuySell.Sell).Count();
		//}

		//public int QtyBuy()
		//{
  //          return txs.Where(t => t.BuySell == EnuBuySell.Buy).Sum(t=>t.Quantity);
		//}

		//public int QtySell()
		//{
  //          return txs.Where(t => t.BuySell == EnuBuySell.Sell).Sum(t => t.Quantity);
		//}

		//public int Qty()
		//{
		//	return txs.Sum(t => t.Quantity);
		//}

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
