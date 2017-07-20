using System;

namespace CryptoAccouting.CoreClass
{
    public class ProfitLoss
    {
        public double BookPrice { get; set; }
        public EnuCCY BaseCCY { get; set; }
        //public double Fee { get; set; }

		private Transaction tx;

        public ProfitLoss(Transaction transaction)
        {
            if (transaction.BuySell == EnuBuySell.Sell) tx = transaction;
        }

        public DateTime TradeDate(){
            return tx.TradeDate;
        }

        public string TxID(){
            return tx.TxId;
        }

        public double RealizedPL(){
            return (tx.TradePrice - BookPrice) * tx.Quantity - tx.Fee;
        }

    }
}
