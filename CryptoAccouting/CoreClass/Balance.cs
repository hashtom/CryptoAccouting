using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAccouting.CoreClass
{
    public class Balance //: IEnumerable<Position>
    {
        public string BalanceName { get; set; }
        public List<Position> positions { get; private set; }
        public List<Position> positionsByCoin { get; private set; }
        public List<Position> positionsByExchange { get; private set; }

		//public EnuExchangeType ExchangeTraded { get; }
		//public DateTime UpdateTime { get; private set; }

		public Balance()
        {
            //ExchangeTraded = exchange_traded;
			//UpdateTime = DateTime.Now;
            positions = new List<Position>();
            //positionsByCoin = new List<Position>();
            //positionsByExchange = new List<Position>();
        }

        public double LatestFiatValue(){
            return positions.Sum(x => x.LatestFiatValue());
        }

		public double LatestBTCValue()
		{
            return positions.Sum(x => x.LatestBTCValue());
		}

        public void SortPositionByHolding(){

            int i = 1;
            positions = positions.OrderByDescending(x => x.AmountBTC()).ToList();

            foreach (var p in positions){
                p.Id = i;
                i++;
            }
        }

        public void RecalculatePositionSummary()
        {
            int id = 0;
            positionsByCoin = new List<Position>();
            positionsByExchange = new List<Position>();

            foreach (var symbol in positions.Select(x => x.Coin.Symbol).Distinct())
            {

                Position posbycoin = new Position(ApplicationCore.GetInstrument(symbol))
                {
                    Id = id,
                    Amount = positions.Where(x => x.Coin.Symbol == symbol).Sum(x => x.Amount),
                    BookPrice = positions.Where(x => x.Coin.Symbol == symbol).Average(x => (x.Amount * x.BookPrice) / x.Amount)
                };
                id++;
                positionsByCoin.Add(posbycoin);
            }

            id = 0;
            foreach (var ex in positions.Select(x => x.TradedExchange).Distinct())
            {
                Position posbyexchange = new Position()
                {
                    Id = id,
                    TradedExchange = ex,
                    Amount = positions.Where(x => x.TradedExchange == ex).Sum(x => x.Amount),
                    BookPrice = positions.Where(x => x.TradedExchange == ex).Average(x => (x.Amount * x.BookPrice) / x.Amount)
                };
                id++;
                positionsByExchange.Add(posbyexchange);
            }

        }

        public void AttachPosition(Position position, bool CalcSummary = true)
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

			if (CalcSummary) RecalculatePositionSummary();

		}

		public void DetachPosition(Position position, bool CalcSummary = true)
		{
			positions.RemoveAll(x => x.Id == position.Id);

            if (CalcSummary) RecalculatePositionSummary();
		}

		public void DetachPositionByCoin(Position position)
		{
            positions.RemoveAll(x => x.Coin.Symbol == position.Coin.Symbol);
            RecalculatePositionSummary();
		}

		public void DetachPositionByExchange(Position position)
		{
            positions.RemoveAll(x => x.TradedExchange == position.TradedExchange);
			RecalculatePositionSummary();
		}

        public Position GetPositionByIndex(int indexNumber){
            return positions[indexNumber];
        }

        //public List<Position> GetPositionList(){
        //    return positions;
        //}
   //     public void Add(Position position){
			//if (positions.Any(x => x.Id == position.Id)) DetachPosition(position);
			//positions.Add(position);
        //}

  //      public int Count()
		//{
		//	return positions.Count;
		//}

  //      public IEnumerator<Position> GetEnumerator()
		//{
  //          for (int i = 0; i <= positions.Count - 1; i++) yield return positions[i];
		//}

		//IEnumerator IEnumerable.GetEnumerator()
		//{
		//	return GetEnumerator();
		//}
    }
}
