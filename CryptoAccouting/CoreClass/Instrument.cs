﻿using System;
using System.Collections.Generic;

namespace CryptoAccouting.CoreClass
{
    public class Instrument
    {

        public string Symbol { get; set; }
        public string Name { get; set; }
        public InstrumentType Type { get; set; }
        public DateTime ListedDate { get; set; }
//        public List<Exchange> Exchanges;
		public Price PriceValue { get; set; }
        public string LogoFileName { get; set; }
        public DateTime UpdateTime { get; }

        public Instrument()
        {
			//Exchanges = new List<Exchange>();
            // PriceValue = fetchPrice() //todo
        }
    }

	public enum InstrumentType
	{
		Crypto,
		Cash,
        Other
	}
}
