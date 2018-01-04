//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;

//namespace CoinBalance.CoreModel
//{
//    public class RealizedPLList: IEnumerable<RealizedPL>
//    {
//        private List<RealizedPL> pl;

//        public RealizedPLList()
//        {
//        }

//        public IEnumerator<RealizedPL> GetEnumerator()
//        {
//            for (int i = 0; i <= pl.Count - 1; i++) yield return pl[i];
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }
//    }
//}
