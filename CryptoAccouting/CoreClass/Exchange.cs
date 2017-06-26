using System;
using System.Collections.Generic;

namespace CryptoAccouting
{
    public class Exchange
    {
        public string ExchangeName { get; }
        public string ExchangeCode { get; }
        public List<Instrument> ListedAssets;
        public List<Transaction> Transactions;

        public DateTime UpdateTime { get; private set; }

        public Exchange(string exchangeName, string exchangeCode)
        {
            ExchangeCode = exchangeCode;
            ExchangeName = exchangeName;
            ListedAssets = new List<Instrument>();
            Transactions = new List<Transaction>();
        }

        public void AttachAsset(Instrument coin){
            //if (ListedAssets.Any(x => x.RizaiOrderId == order.RizaiOrderId)) DetachOrder(order);
            ListedAssets.Add(coin);
        }

        public void AttachTransaction(Transaction tx){
            Transactions.Add(tx);
        }
    }
}
