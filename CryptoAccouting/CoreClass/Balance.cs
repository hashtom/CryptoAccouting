using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class Balance : IEnumerable<Position>
    {
        public string BalanceName { get; set; }
        private List<Position> positions;
		//public EnuExchangeType ExchangeTraded { get; }
		//public DateTime UpdateTime { get; private set; }

		public Balance()
        {
            //ExchangeTraded = exchange_traded;
			//UpdateTime = DateTime.Now;
            positions = new List<Position>();
        }

        public double LatestFiatValue(){
            return positions.Sum(x => x.LatestFiatValue());
        }

		public double LatestBTCValue()
		{
            return positions.Sum(x => x.LatestBTCValue());
		}

        public void AttachPosition(Position position)
		{
            if (positions.Any(x => x.Id == position.Id))
            {
                DetachPosition(position);
            }
            else
            {
                position.Id = positions.Count == 0 ? 1 : positions.Max(x => x.Id) + 1;
            }

			positions.Add(position);
		}

		public void DetachPosition(Position position)
		{
			positions.RemoveAll(x => x.Id == position.Id);
		}

        public Position GetPositionByIndex(int indexNumber){
            return positions[indexNumber];
        }

        public List<Position> GetPositionList(){
            return positions;
        }
   //     public void Add(Position position){
			//if (positions.Any(x => x.Id == position.Id)) DetachPosition(position);
			//positions.Add(position);
        //}

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
