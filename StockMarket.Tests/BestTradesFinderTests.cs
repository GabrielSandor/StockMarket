using NUnit.Framework;

namespace StockMarket.Tests
{
    [TestFixture]
    public class BestTradesFinderTests
    {
        private BestTradesFinder _bestTradesFinder;

        [SetUp]
        public void SetUp()
        {
            _bestTradesFinder = new BestTradesFinder();
        }

        [Test]
        public void TestFailure1()
        {
            var ticks = new[] { 19.35, 19.30 };

            var bestTrade = _bestTradesFinder.Find(ticks);

            Assert.IsFalse(bestTrade.Success);
        }

        [Test]
        public void TestFailure2()
        {
            var ticks = new[] { 19.35 };

            var bestTrade = _bestTradesFinder.Find(ticks);

            Assert.IsFalse(bestTrade.Success);
        }

        [Test]
        public void Test1()
        {
            var ticks = new[] { 19.35, 19.30, 18.88, 18.93, 18.95, 19.03, 19.00, 18.97, 18.97, 18.98 };

            var bestTrade = _bestTradesFinder.Find(ticks);

            Assert.AreEqual(18.88, bestTrade.Buy);
            Assert.AreEqual(19.03, bestTrade.Sell);
        }

        [Test]
        public void Test2()
        {
            var ticks = new[]
            {
                9.20, 8.03, 10.02, 8.08, 8.14, 8.10, 8.31, 8.28, 8.35, 8.34, 8.39, 8.45, 8.38, 8.38, 8.32, 8.36, 8.28,
                8.28, 8.38, 8.48, 8.49, 8.54, 8.73, 8.72, 8.76, 8.74, 8.87, 8.82, 8.81, 8.82, 8.85, 8.85, 8.86, 8.63,
                8.70, 8.68, 8.72, 8.77, 8.69, 8.65, 8.70, 8.98, 8.98, 8.87, 8.71, 9.17, 9.34, 9.28, 8.98, 9.02, 9.16,
                9.15, 9.07, 9.14, 9.13, 9.10, 9.16, 9.06, 9.10, 9.15, 9.11, 8.72, 8.86, 8.83, 8.70, 8.69, 8.73, 8.73,
                8.67, 8.70, 8.69, 8.81, 8.82, 8.83, 8.91, 8.80, 8.97, 8.86, 8.81, 8.87, 8.82, 8.78, 8.82, 8.77, 8.54,
                8.32, 8.33, 8.32, 8.51, 8.53, 8.52, 8.41, 8.55, 8.31, 8.38, 8.34, 8.34, 8.19, 8.17, 8.16
            };

            var bestTrade = _bestTradesFinder.Find(ticks);

            Assert.AreEqual(8.03, bestTrade.Buy);
            Assert.AreEqual(9.34, bestTrade.Sell);
        }

        [Test]
        public void Test3()
        {
            var ticks = new double[] { 2, 3, 10, 6, 4, 8, 1 };

            var bestTrade = _bestTradesFinder.Find(ticks);

            Assert.AreEqual(2, bestTrade.Buy);
            Assert.AreEqual(10, bestTrade.Sell);
        }

        [Test]
        public void Test4()
        {
            var ticks = new double[] { 7, 9, 5, 6, 3, 2 };

            var bestTrade = _bestTradesFinder.Find(ticks);

            Assert.AreEqual(7, bestTrade.Buy);
            Assert.AreEqual(5, bestTrade.Sell);
        }

        [Test]
        public void Test5()
        {
            var ticks = new double[] { 10, 10, 10, 10.0, 10.00 };

            var bestTrade = _bestTradesFinder.Find(ticks);

            Assert.AreEqual(10, bestTrade.Buy);
            Assert.AreEqual(10, bestTrade.Sell);
        }

        [Test]
        public void Test6()
        {
            var ticks = new double[] { 7, 5, 9 };

            var bestTrade = _bestTradesFinder.Find(ticks);

            Assert.AreEqual(7, bestTrade.Buy);
            Assert.AreEqual(9, bestTrade.Sell);
        }
    }
}
