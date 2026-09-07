using Patterns.AlgorithmicPatterns;
using Xunit;

// Mirrors the library's Patterns.AlgorithmicPatterns namespace, which gives the
// Test Explorer a node to group the test classes under. Each one runs its demo, so
// every algorithm has a place to step into with the debugger.
namespace PatternTests.AlgorithmicPatterns
{
    public class GeneticSearchTests
    {
        [Fact]
        public void GeneticSearch()
        {
            GeneticSearchExample.GeneticSearch();
        }
    }

    public class MinMaxSearchTests
    {
        [Fact]
        public void MinMaxSearch()
        {
            MinMaxSearchExample.MinMaxSearch();
        }
    }

    public class MoveOrderingTests
    {
        [Fact]
        public void MoveOrdering()
        {
            MoveOrderingExample.MoveOrdering();
        }
    }

    public class BacktrackingTests
    {
        [Fact]
        public void Backtracking()
        {
            BacktrackingExample.Backtracking();
        }
    }

    public class SoftmaxPolicyTests
    {
        [Fact]
        public void SoftmaxPolicy()
        {
            SoftmaxPolicyExample.SoftmaxPolicy();
        }
    }
}
