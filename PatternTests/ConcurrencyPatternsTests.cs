using Patterns.ConcurrencyPatterns;
using Xunit;

namespace PatternTests
{
    public class ConcurrencyPatternsTests
    {
        [Fact]
        public void ProducersConsumers()
        {
            Patterns.ConcurrencyPatterns.ProducersConsumers.RunProducersConsumers();
        }
    }
}