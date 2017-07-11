using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class Balance : IEnumerable<Position>
    {
        public string BalanceName { get; set; }
        public EnuExchangeType ExchangeTraded { get; }
        private List<Position> positions;
        //public DateTime UpdateTime { get; private set; }

        public Balance(EnuExchangeType exchange_traded)
        {
            ExchangeTraded = exchange_traded;
			//UpdateTime = DateTime.Now;
            positions = new List<Position>();
        }

        public void AttachPosition(Position position)
		{
            if (positions.Any(x => x.Id == position.Id)) DetachPosition(position);
			positions.Add(position);
		}

		public void DetachPosition(Position position)
		{
			positions.RemoveAll(x => x.Id == position.Id);
		}

        public Position GetPositionByIndex(int indexNumber){
            //Position[] pos = positions.ToArray();
            //return pos[indexNumber];
            return positions[indexNumber];
        }

        public int Count()
		{
			return positions.Count;
		}

        public IEnumerator<Position> GetEnumerator()
		{
            for (int i = 0; i <= positions.Count - 1; i++) yield return positions[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
    }
}
