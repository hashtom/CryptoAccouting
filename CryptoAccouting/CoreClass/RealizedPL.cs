using System;

namespace CryptoAccouting.CoreClass
{
    public class RealizedPL
    {
        private Transaction tx;
        public double BookPrice { get; set; }
        //public double RelizedPLValue { get; set; }
        public EnuBaseCCY BaseCCY { get; set; }

        public RealizedPL(Transaction transaction)
        {
            if (tx.BuySell == EnuBuySell.Sell) tx = transaction;
        }

        public DateTime TradeDate(){
            return tx.TradeDate;
        }

        public string TxID(){
            return tx.TxId;
        }

        public double PLValue(){
            return (tx.TradePrice - BookPrice) * tx.Amount;
        }

    }
}
