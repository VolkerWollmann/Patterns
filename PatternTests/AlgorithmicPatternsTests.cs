using System;
using Patterns.AlgorithmicPatterns;
using Xunit;

namespace PatternTests
{
    public class GeneticSearchTests
    {
        // The target every search test evolves towards.
        private const string Target = "CAROLIN NOTHEIS";

        [Fact]
        public void GeneticSearch()
        {
            GeneticSearchExample.GeneticSearch();
        }

        [Fact]
        public void GeneticSearchFindsTheTarget()
        {
            // A fixed seed keeps the run reproducible.
            GeneticSearchResult result = GeneticSearchExample.Search(Target, random: new Random(42));

            Assert.True(result.Solved);
            Assert.Equal(Target, result.Best);
            Assert.Equal(Target.Length, result.Fitness);
        }

        [Fact]
        public void FitnessCountsGenesAtTheRightPosition()
        {
            Assert.Equal(4, GeneticSearchExample.Fitness("GENE", "GENE"));
            Assert.Equal(2, GeneticSearchExample.Fitness("GXNX", "GENE"));
            Assert.Equal(0, GeneticSearchExample.Fitness("XXXX", "GENE"));
        }

        [Fact]
        public void SearchGivesUpWhenTheGenerationBudgetIsSpent()
        {
            // One single generation is never enough to evolve this from noise.
            GeneticSearchResult result = GeneticSearchExample.Search(
                Target,
                maxGenerations: 1,
                random: new Random(42));

            Assert.False(result.Solved);
            Assert.Equal(1, result.Generation);
        }

        [Fact]
        public void SnapshotsCarryTheThreeFittestCandidatesByDefault()
        {
            GeneticSearchResult result = GeneticSearchExample.Search(
                Target,
                random: new Random(42));

            Assert.Equal(3, result.Top.Count);
            Assert.Equal(result.Best, result.Fittest.Chromosome);

            // The snapshot is ordered, fittest first.
            Assert.True(result.Top[0].Fitness >= result.Top[1].Fitness);
            Assert.True(result.Top[1].Fitness >= result.Top[2].Fitness);
        }

        [Fact]
        public void SnapshotsCanCarryTheWholePopulation()
        {
            GeneticSearchResult result = GeneticSearchExample.Search(
                Target,
                populationSize: 20,
                topCount: 20,
                random: new Random(42));

            Assert.Equal(20, result.Top.Count);
        }

        [Fact]
        public void SearchRejectsAnEmptyTarget()
        {
            Assert.Throws<ArgumentException>(() => GeneticSearchExample.Search(string.Empty));
        }
    }

    public class MinMaxSearchTests
    {
        [Fact]
        public void MinMaxSearch()
        {
            MinMaxSearchExample.MinMaxSearch();
        }

        [Fact]
        public void MinMaxFindsTheValueOfBestPlay()
        {
            // The maximizer can force 5: the right half of the tree is worth only 0
            // to it, because the opponent would answer with the 0 leaf there.
            MinMaxSearchResult result = MinMaxSearchExample.MinMax(MinMaxSearchExample.ExampleTree());

            Assert.Equal(5, result.Value);

            // Without pruning every leaf has to be scored.
            Assert.Equal(8, result.EvaluatedLeaves);
            Assert.Equal(0, result.PrunedBranches);
        }

        [Fact]
        public void PruningReachesTheSameValueWithLessWork()
        {
            GameTree tree = MinMaxSearchExample.ExampleTree();

            MinMaxSearchResult minMax = MinMaxSearchExample.MinMax(tree);
            MinMaxSearchResult alphaBeta = MinMaxSearchExample.AlphaBeta(tree);

            // This is what pruning guarantees: the result never changes.
            Assert.Equal(minMax.Value, alphaBeta.Value);

            Assert.True(alphaBeta.EvaluatedLeaves < minMax.EvaluatedLeaves);
            Assert.True(alphaBeta.PrunedBranches > 0);
        }

        [Fact]
        public void PruningSkipsTheBranchesThatCannotMatter()
        {
            MinMaxSearchResult result = MinMaxSearchExample.AlphaBeta(MinMaxSearchExample.ExampleTree());

            // Scored are 3, 5, 6, 1 and 2. The leaf 9 is cut off once 6 already beats
            // what the minimizer can hold, and the whole (0, -1) subtree is skipped
            // once the right half cannot reach the 5 the maximizer already has.
            Assert.Equal(5, result.EvaluatedLeaves);
            Assert.Equal(2, result.PrunedBranches);
        }

        [Fact]
        public void SearchingFromTheMinimizerFlipsTheOutcome()
        {
            GameTree tree = MinMaxSearchExample.ExampleTree();

            MinMaxSearchResult minMax = MinMaxSearchExample.MinMax(tree, maximizing: false);
            MinMaxSearchResult alphaBeta = MinMaxSearchExample.AlphaBeta(tree, maximizing: false);

            Assert.Equal(minMax.Value, alphaBeta.Value);
            Assert.NotEqual(5, minMax.Value);
        }

        [Fact]
        public void AnInnerNodeNeedsChildren()
        {
            Assert.Throws<ArgumentException>(() => GameTree.Node());
        }
    }
}
