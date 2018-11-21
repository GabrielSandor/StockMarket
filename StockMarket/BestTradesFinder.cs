namespace StockMarket
{
    public class BestTradesFinder
    {
        public BestTrade Find(double[] ticks)
        {
            if (ticks.Length <= 2)
            {
                return new BestTrade { Success = false };
            }

            var maxDiff = ticks[2] - ticks[0];
            var minimum = ticks[0];
            var indexOfMin = 0;
            var indexOfBuy = 0;

            double buy = minimum, sell = ticks[2];

            for (var i = 1; i < ticks.Length; i++)
            {
                var current = ticks[i];

                if (i - indexOfMin > 1)
                {
                    var diff = current - minimum;

                    if (diff > maxDiff)
                    {
                        maxDiff = diff;
                        buy = minimum;
                        indexOfBuy = indexOfMin;
                        sell = current;
                    }
                }
                else if (i - indexOfBuy > 1)
                {
                    var diff = current - buy;

                    if (diff > maxDiff)
                    {
                        maxDiff = diff;
                        sell = current;
                    }
                }

                if (current < minimum)
                {
                    minimum = current;
                    indexOfMin = i;
                }
            }

            return new BestTrade
            {
                Buy = buy,
                Sell = sell
            };
        }
    }

    public class BestTrade
    {
        public bool Success { get; set; }

        public double Buy { get; set; }
        public double Sell { get; set; }

        public BestTrade()
        {
            Success = true;
        }
    }
}
