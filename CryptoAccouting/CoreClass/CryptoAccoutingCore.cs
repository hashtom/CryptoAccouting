﻿using System; using System.Collections.Generic;  namespace CryptoAccouting {     public static class CryptoAccoutingCore     {         private static Balance myBalance;         //private static AppConfig myConfig;
                                                                                                                                                                                      //

        //public CryptoAccoutingCore(string baseCur)
        //{
        //    myBalance = new Balance();
        //    myConfig = new AppConfig();
        //    BaseCurrency = baseCur;
        //}

        public static Balance FetchMyBalance()         {

            // Test Data
            Balance mybal;             Instrument coin1, coin2, coin3;// Test Data             Position pos1, pos2, pos3; //Test Data              mybal = new Balance(new Exchange(EnuExchangeType.Zaif)); 
            // Test Data Creation
            coin1 = new Instrument() { Symbol = "BTC", LogoFileName = "Images/btc.png" };             coin2 = new Instrument() { Symbol = "ETH", LogoFileName = "Images/eth.png" };             coin3 = new Instrument() { Symbol = "REP", LogoFileName = "Images/rep.png" };
            pos1 = new Position(coin1, "1") { Amount = 850 };             pos2 = new Position(coin2, "2") { Amount = 1000 };             pos3 = new Position(coin3, "3") { Amount = 25000 };             pos1.AttachPriceData(new Price(coin1) { LatestPrice = 780, DayVolume=100000 });             pos2.AttachPriceData(new Price(coin2) { LatestPrice = 0.14, DayVolume = 30000 });             pos3.AttachPriceData(new Price(coin3) { LatestPrice = 0.013, DayVolume = 45560 });                              mybal.AttachPosition(pos1);             mybal.AttachPosition(pos2);             mybal.AttachPosition(pos3);              myBalance = mybal;              return myBalance;         }     } } 