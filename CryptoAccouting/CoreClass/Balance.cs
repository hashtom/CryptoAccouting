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

		public Balance()
        {
            positions = new List<Position>();
        }

        public double LatestFiatValueUSD(){
            return positions.Sum(x => x.LatestFiatValueUSD());
        }

		public double LatestFiatValueBase(CrossRate CrossRate)
		{
            return positions.Sum(x => x.LatestFiatValueBase(CrossRate));
		}

		public double AmountBTC()
		{
            return positions.Sum(x => x.AmountBTC());
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

                Position posbycoin = new Position(ApplicationCore.GetInstrument(symbol),EnuPositionType.CoinLevel)
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
                Position posbyexchange = new Position( EnuPositionType.ExchangeLevel)
                {
                    Id = id,
                    TradedExchange = ex,
                    Amount = positions.Where(x => x.TradedExchange == ex).Sum(x => x.AmountBTC()),
                    FiatValueTotalExchange = positions.Where(x => x.TradedExchange == ex).Sum(x => x.LatestFiatValueUSD()), //todo take multiple currencies into account
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
                DetachPosition(position, false);
            }
            else
            {
                position.Id = positions.Count == 0 ? 1 : positions.Max(x => x.Id) + 1;
            }

			positions.Add(position);

			if (CalcSummary) RecalculatePositionSummary();

		}

		public void DetachPosition(Position position, bool CalcSummary)
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

    }
}
