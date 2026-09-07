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

    public class SoftmaxPolicyTests
    {
        // Pizza disappoints most often, the salad is the best thing on the menu -
        // which is exactly what the guest has to find out.
        private static readonly IReadOnlyList<Dish> Menu = SoftmaxPolicyExample.ExampleMenu();

        [Fact]
        public void SoftmaxPolicy()
        {
            SoftmaxPolicyExample.SoftmaxPolicy();
        }

        [Fact]
        public void ProbabilitiesAreADistributionOrderedLikeTheEstimates()
        {
            IReadOnlyList<double> probabilities =
                SoftmaxPolicyExample.Probabilities(new[] { 0.2, 0.9, 0.5 }, 0.4);

            Assert.Equal(1.0, probabilities.Sum(), 10);

            // Nothing on the menu is ever ruled out - that is the point of the policy.
            Assert.All(probabilities, chance => Assert.True(chance > 0.0));

            // The better the estimate, the likelier the dish.
            Assert.True(probabilities[1] > probabilities[2]);
            Assert.True(probabilities[2] > probabilities[0]);
        }

        [Fact]
        public void ATemperatureNearZeroTurnsThePolicyGreedy()
        {
            IReadOnlyList<double> probabilities = SoftmaxPolicyExample.Probabilities(
                new[] { 0.2, 0.9, 0.5 }, SoftmaxPolicyExample.MinimumTemperature);

            Assert.Equal(1.0, probabilities[1], 6);
        }

        [Fact]
        public void AHighTemperatureSpreadsTheChancesOutEvenly()
        {
            IReadOnlyList<double> probabilities =
                SoftmaxPolicyExample.Probabilities(new[] { 0.2, 0.9, 0.5 }, 1000.0);

            Assert.All(probabilities, chance => Assert.Equal(1.0 / 3.0, chance, 3));
        }

        [Fact]
        public void OnlyTheDifferencesBetweenTheEstimatesMatter()
        {
            // Softmax is shift invariant, which is what lets the implementation
            // subtract the largest estimate to keep exp() in range.
            IReadOnlyList<double> plain =
                SoftmaxPolicyExample.Probabilities(new[] { 0.2, 0.9, 0.5 }, 0.4);

            IReadOnlyList<double> shifted =
                SoftmaxPolicyExample.Probabilities(new[] { 100.2, 100.9, 100.5 }, 0.4);

            for (int dish = 0; dish < plain.Count; dish++)
            {
                Assert.Equal(plain[dish], shifted[dish], 10);
            }
        }

        [Fact]
        public void EveryDishOwnsItsSliceOfTheWheel()
        {
            double[] probabilities = { 0.2, 0.3, 0.5 };

            Assert.Equal(0, SoftmaxPolicyExample.Choose(probabilities, new DrawnAt(0.0)));
            Assert.Equal(0, SoftmaxPolicyExample.Choose(probabilities, new DrawnAt(0.199)));
            Assert.Equal(1, SoftmaxPolicyExample.Choose(probabilities, new DrawnAt(0.2)));
            Assert.Equal(1, SoftmaxPolicyExample.Choose(probabilities, new DrawnAt(0.499)));
            Assert.Equal(2, SoftmaxPolicyExample.Choose(probabilities, new DrawnAt(0.5)));
            Assert.Equal(2, SoftmaxPolicyExample.Choose(probabilities, new DrawnAt(0.999999)));
        }

        [Fact]
        public void TheGuestFindsTheBestDishWithoutEverBeingToldWhichItIs()
        {
            SoftmaxLearningResult result = SoftmaxPolicyExample.Learn(
                Menu, temperature: 1.0, cooling: 0.99, random: new Random(18));

            Assert.Equal("Salat", result.FavouriteDish);

            // And what they learned about it matches what the kitchen really does.
            Assert.True(Math.Abs(result.Estimates[2] - Menu[2].ChanceOfBeingGood) < 0.05);
        }

        [Fact]
        public void TryingThingsOutKeepsEveryDishOnTheTable()
        {
            SoftmaxLearningResult hot = SoftmaxPolicyExample.Learn(
                Menu, temperature: 1.0, random: new Random(18));

            // A hot policy never settles. It keeps ordering the weak dishes, which is
            // what makes its estimates good and its lunches mediocre.
            Assert.All(hot.Orders, count => Assert.True(count > 100));

            for (int dish = 0; dish < Menu.Count; dish++)
            {
                Assert.True(Math.Abs(hot.Estimates[dish] - Menu[dish].ChanceOfBeingGood) < 0.1);
            }

            Assert.True(hot.AverageReward < Menu.Max(dish => dish.ChanceOfBeingGood));
        }

        [Fact]
        public void CoolingDownEatsBetterThanStayingHotOrStartingCold()
        {
            SoftmaxLearningResult hot = SoftmaxPolicyExample.Learn(
                Menu, temperature: 1.0, random: new Random(18));

            SoftmaxLearningResult cold = SoftmaxPolicyExample.Learn(
                Menu, temperature: 0.02, random: new Random(18));

            SoftmaxLearningResult cooling = SoftmaxPolicyExample.Learn(
                Menu, temperature: 1.0, cooling: 0.99, random: new Random(18));

            // The cold guest settles before knowing anything: on this seed the pizza is
            // good on day one, and they never look at the menu again - even though the
            // pizza is the worst of the three.
            Assert.Equal("Pizza", cold.FavouriteDish);
            Assert.Equal(cold.Days, cold.Orders[0]);
            Assert.True(cold.AverageReward < hot.AverageReward);

            // Trying everything first and settling later beats both.
            Assert.True(cooling.AverageReward > hot.AverageReward);
            Assert.True(cooling.AverageReward > cold.AverageReward);
        }

        [Fact]
        public void AnEstimateIsTheMeanOfEveryPlateOfThatDish()
        {
            // One dish is always good, the other never is.
            SoftmaxLearningResult result = SoftmaxPolicyExample.Learn(
                new[] { new Dish("Pasta", 1.0), new Dish("Pizza", 0.0) },
                days: 200,
                temperature: 1.0,
                random: new Random(7));

            Assert.Equal(1.0, result.Estimates[0]);
            Assert.Equal(0.0, result.Estimates[1]);

            // So every good lunch came from the dish the kitchen can cook.
            Assert.Equal(result.Orders[0], result.TotalReward);
            Assert.Equal(result.Days, result.Orders.Sum());
        }

        [Fact]
        public void EveryLunchIsReportedWithTheChancesItWasChosenOn()
        {
            List<SoftmaxLearningStep> lunches = new List<SoftmaxLearningStep>();

            SoftmaxPolicyExample.Learn(
                Menu, days: 50, temperature: 1.0, random: new Random(18), onLunch: lunches.Add);

            Assert.Equal(50, lunches.Count);
            Assert.Equal(1, lunches[0].Day);

            // Nothing is known yet, so the first choice is a uniform one.
            Assert.All(lunches[0].Probabilities, chance => Assert.Equal(1.0 / 3.0, chance, 10));

            Assert.All(lunches, lunch => Assert.Equal(1.0, lunch.Probabilities.Sum(), 10));
        }

        [Fact]
        public void TheTemperatureNeverCoolsDownToZero()
        {
            List<SoftmaxLearningStep> lunches = new List<SoftmaxLearningStep>();

            SoftmaxPolicyExample.Learn(
                Menu, days: 200, temperature: 1.0, cooling: 0.5, random: new Random(18),
                onLunch: lunches.Add);

            // At zero the softmax would divide by zero, so the schedule stops short.
            Assert.All(lunches, lunch => Assert.True(lunch.Temperature >= SoftmaxPolicyExample.MinimumTemperature));
            Assert.Equal(SoftmaxPolicyExample.MinimumTemperature, lunches[^1].Temperature);
        }

        [Fact]
        public void AGuestWhoNeverEatsHasNothingToShow()
        {
            SoftmaxLearningResult result = SoftmaxPolicyExample.Learn(Menu, days: 0);

            Assert.Equal(0.0, result.AverageReward);
            Assert.All(result.Orders, count => Assert.Equal(0, count));
        }

        [Fact]
        public void APolicyNeedsAMenuAndAPositiveTemperature()
        {
            Assert.Throws<ArgumentException>(
                () => SoftmaxPolicyExample.Probabilities(Array.Empty<double>(), 1.0));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => SoftmaxPolicyExample.Probabilities(new[] { 0.5 }, 0.0));

            Assert.Throws<ArgumentException>(() => SoftmaxPolicyExample.Learn(Array.Empty<Dish>()));
            Assert.Throws<ArgumentOutOfRangeException>(() => SoftmaxPolicyExample.Learn(Menu, days: -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => SoftmaxPolicyExample.Learn(Menu, temperature: -1.0));
            Assert.Throws<ArgumentOutOfRangeException>(() => SoftmaxPolicyExample.Learn(Menu, cooling: 1.5));

            // A dish is good some of the time, all of the time or never.
            Assert.Throws<ArgumentOutOfRangeException>(() => new Dish("Pizza", 1.5));
        }

        // A Random that always draws the same number, so one single choice can be
        // pinned down exactly.
        private class DrawnAt : Random
        {
            private readonly double drawn;

            public DrawnAt(double drawn)
            {
                this.drawn = drawn;
            }

            public override double NextDouble()
            {
                return drawn;
            }
        }
    }
}
