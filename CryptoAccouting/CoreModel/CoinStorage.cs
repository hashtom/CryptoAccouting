using System;
using System.Collections.Generic;
using System.Linq;

namespace CoinBalance.CoreModel
{
    public class CoinStorage
    {
		public string Code { get; set; }
		public string Name { get; set; }
        public double Weight { get; set; }
        public EnuCoinStorageType StorageType { get; set; }
        public Balance BalanceOnStorage { get; private set; }

        public CoinStorage(string code, EnuCoinStorageType storatetype)
        {
            this.Code = code;
            this.Name = code;
            this.StorageType = storatetype;
            BalanceOnStorage = new Balance();
        }

        public double ColumnHolding
        {
            get { return AmountBTC(); }
        }

        public double ColumnValue
        {
            get { return LatestFiatValueBase(); }  // AppCore.NumberFormat(LatestFiatValueBase()); }
        }
       
        public string ColumnWeight
        {
            get { return String.Format("{0:n2}", Weight * 100) + "%"; }
        }

        public void ClearBalanceOnStorage()
        {
            BalanceOnStorage = new Balance();
        }

        public void AttachPosition(Position position)
        {
            BalanceOnStorage.Attach(position);
        }

        public void DetachPosition(Position position)
        {
            BalanceOnStorage.Detach(position);
        }

        public void DetachPositionByCoin(string InstrumentId)
        {
            BalanceOnStorage.DetachPositionByCoin(InstrumentId);
        }

        public void DetachPositionByStorage(CoinStorage storage)
        {
            BalanceOnStorage.DetachPositionByStorage(storage);
        }

        public double Amount()
        {
            return BalanceOnStorage.Amount();
        }

        public double AmountBTC()
        {
            return BalanceOnStorage.AmountBTC();
        }

        public double LatestFiatValueUSD()
        {
            return BalanceOnStorage.LatestFiatValueUSD();
        }

		public double LatestFiatValueBase()
		{
			return BalanceOnStorage.LatestFiatValueBase();
		}

        public bool HasBalance()
        {
            return Math.Abs(Amount()) > 0 ? true : false;
        }

    }

    public enum EnuCoinStorageType
    {
        TBA,
        Hardware,
        Desktop,
        Mobile,
        Web,
        Exchange
    }
}
