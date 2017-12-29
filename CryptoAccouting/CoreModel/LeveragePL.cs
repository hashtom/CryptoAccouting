using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CoinBalance.CoreModel
{

    public class LeveragePL : IEnumerable<LeveragePosition>
    {
        private List<LeveragePosition> positions;


        public IEnumerator<LeveragePosition> GetEnumerator()
        {
            for (int i = 0; i <= positions.Count - 1; i++) yield return positions[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
