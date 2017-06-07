using System;
using System.Collections.Generic;

namespace CryptoAccouting
{
    public class Exchange
    {
        public string ExchangeName { get; }
        public string ExchangeCode { get; }
        public List<AssetAttribute> ListedAssets;

        public Exchange(string exchangeName, string exchangeCode)
        {
            ExchangeCode = exchangeCode;
            ExchangeName = exchangeName;
            ListedAssets = new List<AssetAttribute>();
        }

        public void AttachAsset(ref AssetAttribute asset){
            //if (ListedAssets.Any(x => x.RizaiOrderId == order.RizaiOrderId)) DetachOrder(order);
            ListedAssets.Add(asset);
        }
    }
}
