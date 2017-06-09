using System;
using System.Collections.Generic;

namespace CryptoAccouting
{
    public class Exchange
    {
        public string ExchangeName { get; }
        public string ExchangeCode { get; }
        public List<Instrument> ListedAssets;
        public DateTime UpdateTime { get; private set; }

        public Exchange(string exchangeName, string exchangeCode)
        {
            ExchangeCode = exchangeCode;
            ExchangeName = exchangeName;
            ListedAssets = new List<Instrument>();
        }

        public void AttachAsset(ref Instrument asset){
            //if (ListedAssets.Any(x => x.RizaiOrderId == order.RizaiOrderId)) DetachOrder(order);
            ListedAssets.Add(asset);
        }
    }
}
