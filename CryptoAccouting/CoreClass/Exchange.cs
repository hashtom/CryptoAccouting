using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class Exchange : CoinStorage
    {
        public InstrumentList Coins { get; private set; }
        public List<SymbolMap> ExchangeSymbolMap { get; set; }
        public bool APIReady { get; set; }
		public string Key { get; set; }
		public string Secret { get; set; }
		public TradeList TradeList { get; set; }
        public string LogoFileName { get; set; }

        public Exchange(string code, EnuCoinStorageType storagetype) : base(code, storagetype)
        {
            Coins = new InstrumentList();
            ExchangeSymbolMap = new List<SymbolMap>();
            if (storagetype != EnuCoinStorageType.Exchange) throw new Exception();
        }

        public bool IsListed(string instrumemntID)
        {
            return Coins.Any(x => x.Id == instrumemntID);
        }

        public void AttachListedCoin(Instrument coin)
		{
            if (!Coins.Any(c => c.Id == coin.Id)) Coins.Attach(coin);
		}

        public void AttachSymbolMap(string instrumentID, EnuSymbolMapType symbolmaptype)
        {
            DetachSymbolMap(instrumentID);
            ExchangeSymbolMap.Add(new SymbolMap(instrumentID, symbolmaptype));
        }

        public void DetachSymbolMap(string instrumentID)
        {
            if (ExchangeSymbolMap.Any(x=>x.InstrumentID == instrumentID))
                ExchangeSymbolMap.RemoveAll(x=>x.InstrumentID == instrumentID);
        }

        public string GetSymbolForExchange(string instrumentID)
        {
            var coin = Coins.First(x => x.Id == instrumentID);
            var type = ExchangeSymbolMap.Any(x => x.InstrumentID == instrumentID) ?
                                        ExchangeSymbolMap.First(x => x.InstrumentID == instrumentID).SymbolMapType :
                                        EnuSymbolMapType.Symbol1;

            return type == EnuSymbolMapType.Symbol1 ? coin.Symbol1 : coin.Symbol2;
                                               
        }

    }

    public class SymbolMap
    {
        public string InstrumentID { get; set; }
        public EnuSymbolMapType SymbolMapType { get; set; }

        public SymbolMap(string instrumentID, EnuSymbolMapType symbolmaptype)
        {
            this.InstrumentID = instrumentID;
            this.SymbolMapType = symbolmaptype;
        }
    }

    public enum EnuSymbolMapType
    {
        Symbol1,
        Symbol2
    }
}
