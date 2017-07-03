using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting
{
    public class Transactions : IEnumerable<Transaction>
    {

        private List<Transaction> txs;

        public Transactions()
        {
            txs = new List<Transaction>();
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
    }
}
