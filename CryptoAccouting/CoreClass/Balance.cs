using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting
{
    public class Balance : IEnumerable<Position>
    {
        public string BalanceID { get; set; }
        public DateTime UpdateTime { get; set; }
        private List<Position> positions;

        public Balance()
        {
			UpdateTime = DateTime.Now;
            positions = new List<Position>();
        }

        public void AttachPosition(Position position)
		{
            if (positions.Any(x => x.PositionID == position.PositionID)) DetachOrder(position);
			positions.Add(position);
		}

		public void DetachPosition(Position position)
		{
			positions.RemoveAll(x => x.PositionID == position.PositionID);
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
