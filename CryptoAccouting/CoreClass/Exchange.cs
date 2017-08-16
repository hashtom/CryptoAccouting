using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class Exchange
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<Instrument> ListedCoin { get; set; }
        public bool APIReady { get; set; }
		public string Key { get; set; }
		public string Secret { get; set; }
		public TradeList TradeList { get; set; }
        public Balance BalanceOnExchange { get; set; } 
        public string LogoFileName { get; set; }
        //public DateTime UpdateTime { get; private set; }


        public Exchange(string Code)
        {
            this.Code = Code;
            ListedCoin = new List<Instrument>();
        }

		public void AttachListedCoin(Instrument coin)
		{
            if (!ListedCoin.Any(c => c.Symbol == coin.Symbol)) ListedCoin.Add(coin);
		}

        public void AttachPosition(Position pos)
        {
            if (BalanceOnExchange is null) BalanceOnExchange = new Balance();
            BalanceOnExchange.AttachPosition(pos);
        }

		public void DetachPosition(Position pos)
		{
            BalanceOnExchange.DetachPosition(pos, true);
		}

        //public bool IsNotSelected()
        //{
        //    return (this.ExchangeType == EnuExchangeType.NotSelected);
        //}
    }

	//public enum EnuExchangeType
	//{
        //NotSpecified,
        //Exchange
		//Zaif = 1,
		//BitFlyer = 2,
		//Kraken = 3,
		//CoinCheck = 4,
		//BitBank = 5,
        //Poloniex = 6,
        //Bittrex = 7,
        //CCex = 8,
        //Bter = 9,
        //Livecoin,
        //YoBit,
        //Hitbtc,
        //Bleutrade,
        //Bitstamp,
        //BTCTrade,
        //BitFinex,
        //Korbit,
        //OKCoin,
        //Gemini,
        //GDAX,
        //BTC38,
        //Gatecoin,
        //CEXIO,
        //Quoine,
        //viaBTC,
        //Liqui,
        //itBit,
        //Cexio,
        //Exmo,
        //Indacoin,
        //Nixe,
        //BTCChina
	//}
}
