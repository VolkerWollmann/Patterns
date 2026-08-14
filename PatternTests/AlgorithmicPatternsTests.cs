using System;
using System.Collections.Generic;
using System.Linq;
using Patterns.AlgorithmicPatterns;
using Xunit;

// Mirrors the library's Patterns.AlgorithmicPatterns namespace, which gives the
// Test Explorer a node to group both test classes under.
namespace PatternTests.AlgorithmicPatterns
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

    public class MoveOrderingTests
    {
        [Fact]
        public void MoveOrdering()
        {
            MoveOrderingExample.MoveOrdering();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(2024)]
        public void EveryOrderingAgreesOnTheValue(int seed)
        {
            GameTree tree = MoveOrderingExample.RandomTree(4, 5, new Random(seed));

            int plain = MinMaxSearchExample.MinMax(tree).Value;

            // Ordering is free to change the work, never the answer.
            Assert.Equal(plain, MoveOrderingExample.AlphaBeta(tree, MoveOrder.AsGiven).Value);
            Assert.Equal(plain, MoveOrderingExample.AlphaBeta(tree, MoveOrder.BestFirst).Value);
            Assert.Equal(plain, MoveOrderingExample.AlphaBeta(tree, MoveOrder.WorstFirst).Value);
        }

        // Knuth and Moore showed that alpha-beta with perfect ordering examines the
        // smallest tree any correct search can get away with:
        // b^ceil(d/2) + b^floor(d/2) - 1 leaves. Perfect ordering hits it exactly,
        // whatever the tree happens to contain.
        [Theory]
        [InlineData(2, 4)]
        [InlineData(2, 6)]
        [InlineData(3, 4)]
        [InlineData(4, 4)]
        [InlineData(4, 5)]
        [InlineData(5, 3)]
        public void PerfectOrderingReachesTheMinimalTree(int branchingFactor, int depth)
        {
            int minimalTree =
                (int)(Math.Pow(branchingFactor, Math.Ceiling(depth / 2.0))
                    + Math.Pow(branchingFactor, Math.Floor(depth / 2.0))
                    - 1);

            foreach (int seed in new[] { 1, 42, 2024 })
            {
                GameTree tree = MoveOrderingExample.RandomTree(branchingFactor, depth, new Random(seed));

                MinMaxSearchResult best = MoveOrderingExample.AlphaBeta(tree, MoveOrder.BestFirst);

                Assert.Equal(minimalTree, best.EvaluatedLeaves);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(2024)]
        public void OrderingDecidesHowMuchPruningIsWorth(int seed)
        {
            GameTree tree = MoveOrderingExample.RandomTree(4, 5, new Random(seed));

            int plain = MinMaxSearchExample.MinMax(tree).EvaluatedLeaves;
            int asGiven = MoveOrderingExample.AlphaBeta(tree, MoveOrder.AsGiven).EvaluatedLeaves;
            int bestFirst = MoveOrderingExample.AlphaBeta(tree, MoveOrder.BestFirst).EvaluatedLeaves;
            int worstFirst = MoveOrderingExample.AlphaBeta(tree, MoveOrder.WorstFirst).EvaluatedLeaves;

            // Pruning never costs more than searching everything ...
            Assert.True(asGiven <= plain);
            Assert.True(worstFirst <= plain);

            // ... and no ordering beats the perfect one.
            Assert.True(bestFirst <= asGiven);
            Assert.True(bestFirst <= worstFirst);

            // The bad ordering gives almost the whole gain away: it stays within a
            // tenth of the unpruned search, while the good one is an order of
            // magnitude below it.
            Assert.True(worstFirst > plain * 0.9);
            Assert.True(bestFirst < plain / 10.0);
        }

        [Fact]
        public void ATreeNeedsSensibleDimensions()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => MoveOrderingExample.RandomTree(0, 3, new Random(42)));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => MoveOrderingExample.RandomTree(2, -1, new Random(42)));
        }
    }

    public class BacktrackingTests
    {
        [Fact]
        public void Backtracking()
        {
            BacktrackingExample.Backtracking();
        }

        // The number of ways n queens fit on an n x n board is long known, so the
        // search can be checked against it exactly.
        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        [InlineData(4, 2)]
        [InlineData(5, 10)]
        [InlineData(6, 4)]
        [InlineData(7, 40)]
        [InlineData(8, 92)]
        public void FindsEveryArrangementOfQueens(int boardSize, int expectedSolutions)
        {
            BacktrackingResult result = BacktrackingExample.Solve(boardSize);

            Assert.Equal(expectedSolutions, result.Count);
        }

        [Fact]
        public void NoSolutionPutsTwoQueensInReachOfEachOther()
        {
            BacktrackingResult result = BacktrackingExample.Solve(8);

            foreach (int[] solution in result.Solutions)
            {
                for (int row = 0; row < solution.Length; row++)
                {
                    // Checking a queen against the ones before it covers every pair.
                    Assert.True(
                        BacktrackingExample.IsSafe(row, solution[row], solution),
                        $"Queens attack each other in:\n{BacktrackingExample.Format(solution)}");
                }
            }
        }

        [Fact]
        public void EverySolutionIsDistinct()
        {
            BacktrackingResult result = BacktrackingExample.Solve(8);

            HashSet<string> seen = new HashSet<string>();

            foreach (int[] solution in result.Solutions)
            {
                Assert.True(seen.Add(string.Join(",", solution)), "The same solution was reported twice.");
            }
        }

        [Fact]
        public void PruningLooksAtFarFewerSquaresThanPlacingBlindly()
        {
            BacktrackingResult result = BacktrackingExample.Solve(8);

            // Placing one queen per row without checking anything means 8^8 boards.
            Assert.True(result.ExploredPositions < Math.Pow(8, 8) / 1000);
        }

        [Fact]
        public void TheSearchStopsOnceEnoughSolutionsAreFound()
        {
            BacktrackingResult all = BacktrackingExample.Solve(8);
            BacktrackingResult first = BacktrackingExample.Solve(8, maxSolutions: 1);

            Assert.Equal(1, first.Count);

            // Stopping early is what saves the work - it does not just trim the list.
            Assert.True(first.ExploredPositions < all.ExploredPositions);

            // And the one it stops at is the first of the full run.
            Assert.Equal(all.Solutions[0], first.Solutions[0]);
        }

        [Fact]
        public void ABoardCannotHaveANegativeSize()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => BacktrackingExample.Solve(-1));
        }

        [Fact]
        public void TheSearchReportsWhenItRunsIntoADeadEnd()
        {
            List<BacktrackingStep> steps = new List<BacktrackingStep>();

            BacktrackingExample.Solve(4, onStep: steps.Add);

            // On a 4x4 board the first column of the first row leads nowhere, so the
            // search has to run into dead ends and take decisions back.
            Assert.Contains(steps, step => step.Kind == BacktrackingStepKind.DeadEnd);
            Assert.Contains(steps, step => step.Kind == BacktrackingStepKind.Backtrack);
            Assert.Equal(2, steps.Count(step => step.Kind == BacktrackingStepKind.Solution));
        }

        [Fact]
        public void EveryQueenThatGoesDownComesBackOff()
        {
            List<BacktrackingStep> steps = new List<BacktrackingStep>();

            BacktrackingExample.Solve(5, onStep: steps.Add);

            // A completed search unwinds fully: every choose is matched by its undo.
            Assert.Equal(
                steps.Count(step => step.Kind == BacktrackingStepKind.Place),
                steps.Count(step => step.Kind == BacktrackingStepKind.Backtrack));
        }

        [Fact]
        public void ADeadEndIsAlwaysFollowedByTakingAQueenBack()
        {
            List<BacktrackingStep> steps = new List<BacktrackingStep>();

            BacktrackingExample.Solve(5, onStep: steps.Add);

            for (int i = 0; i < steps.Count - 1; i++)
            {
                if (steps[i].Kind != BacktrackingStepKind.DeadEnd)
                    continue;

                // A dead end hands control straight back to the row above it.
                BacktrackingStep next = steps[i + 1];

                Assert.Equal(BacktrackingStepKind.Backtrack, next.Kind);
                Assert.Equal(steps[i].Row - 1, next.Row);
            }
        }
    }
}
