using System;
using System.Collections.Generic;
using System.Linq;

namespace CoinBalance.CoreClass
{
    public class Exchange : CoinStorage
    {
        public bool APIProvided { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string LogoFileName { get; set; }
        public TradeList TradeList { get; private set; }
        private InstrumentList Coins;
        private List<SymbolMap> ExchangeSymbolMap;

        public Exchange(string code, EnuCoinStorageType storagetype) : base(code, storagetype)
        {
            Coins = new InstrumentList();
            ExchangeSymbolMap = new List<SymbolMap>();
            Key = "";
            Secret = "";
            if (storagetype != EnuCoinStorageType.Exchange) throw new Exception();
        }

        public bool APIKeyAvailable()
        {
            return (Key != "" && Secret != "");
        }

        public void ClearAPIKeys()
        {
            Key = "";
            Secret = "";
        }

        public bool IsListed(string instrumemntID)
        {
            return Coins.Any(x => x.Id == instrumemntID);
        }

        public void AttachListedCoin(Instrument coin)
        {
            if (!Coins.Any(c => c.Id == coin.Id)) Coins.Attach(coin);
        }

        public void AttachSymbolMap(string instrumentID, string Symbol, EnuSymbolMapType symbolmaptype)
        {
            DetachSymbolMap(instrumentID);
            ExchangeSymbolMap.Add(new SymbolMap(instrumentID, Symbol, symbolmaptype));
        }

        public void DetachSymbolMap(string instrumentID)
        {
            if (ExchangeSymbolMap.Any(x => x.InstrumentID == instrumentID))
                ExchangeSymbolMap.RemoveAll(x => x.InstrumentID == instrumentID);
        }

        public string GetIdForExchange(string symbol)
        {
            return ExchangeSymbolMap.Any(x => x.SymbolForExchange == symbol) ? 
                                     ExchangeSymbolMap.First(x => x.SymbolForExchange == symbol).InstrumentID : 
                                      null;
        }

        public bool HasListedCoins()
        {
            return Coins.Count() > 0 ? true : false;
        }

        public string GetSymbolForExchange(string instrumentID)
        {
            if (HasListedCoins())
            {
                var coin = Coins.GetByInstrumentId(instrumentID);
                var type = ExchangeSymbolMap.Any(x => x.InstrumentID == instrumentID) ?
                                            ExchangeSymbolMap.First(x => x.InstrumentID == instrumentID).SymbolMapType :
                                            EnuSymbolMapType.Symbol1;
                return type == EnuSymbolMapType.Symbol1 ? coin.Symbol1 : coin.Symbol2;
            }
            else
            {
                return null;
            }
        }

        public void AttachTradeList(TradeList tradelist)
        {
            this.TradeList = tradelist;
        }

        private class SymbolMap
        {
            public string InstrumentID { get; private set; }
            public string SymbolForExchange { get; private set; }
            public EnuSymbolMapType SymbolMapType { get; private set; }

            public SymbolMap(string instrumentID, string Symbol, EnuSymbolMapType symbolmaptype)
            {
                this.InstrumentID = instrumentID;
                this.SymbolForExchange = Symbol;
                this.SymbolMapType = symbolmaptype;
            }
        }

    }

    public enum EnuSymbolMapType
    {
        Symbol1,
        Symbol2
    }
}
