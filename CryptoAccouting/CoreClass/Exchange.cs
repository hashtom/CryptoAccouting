using System;
using System.Collections.Generic;

namespace CryptoAccouting.CoreClass
{
   // public class Exchange
   // {
   //     //public string ExchangeName { get; }
   //     public EnuExchangeType ExchangeType { get; }
   //     public List<Instrument> ListedAssets;
   //     public List<Transaction> Transactions;

   //     public DateTime UpdateTime { get; private set; }

   //     public Exchange(EnuExchangeType excType)
   //     {
			////ExchangeName = exchangeName;
    //        ExchangeType = excType;

    //        ListedAssets = new List<Instrument>();
    //        Transactions = new List<Transaction>();
    //    }

    //    public void AttachAsset(Instrument coin){
    //        //if (ListedAssets.Any(x => x.RizaiOrderId == order.RizaiOrderId)) DetachOrder(order);
    //        ListedAssets.Add(coin);
    //    }

    //    public void AttachTransaction(Transaction tx){
    //        Transactions.Add(tx);
    //    }
    //}

	public enum EnuExchangeType
	{
		Zaif = 1,
		BitFlyer = 2,
		Kraken = 3,
		CoinCheck = 4,
		BitBank = 5
	}
}
