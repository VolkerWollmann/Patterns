using Microsoft.VisualStudio.TestTools.UnitTesting;
using Patterns.ConcurrencyPatterns;

namespace PatternTests
{
    [TestClass]
    public class ConcurrencyPatternsTests
    {
        [TestMethod]
        public void ProducersConsumers()
        {
            Patterns.ConcurrencyPatterns.ProducersConsumers.RunProducersConsumers();
        }
    }
}