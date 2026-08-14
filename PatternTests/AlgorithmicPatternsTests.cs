using System;
using Patterns.AlgorithmicPatterns;
using Xunit;

namespace PatternTests
{
    public class AlgorithmicPatternsTests
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
}
