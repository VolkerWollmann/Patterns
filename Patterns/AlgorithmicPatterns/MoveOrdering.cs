using System;
using System.Collections.Generic;
using System.Linq;

// Source : https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning#Improvements
//          https://www.chessprogramming.org/Move_Ordering
// Alpha-beta pruning only cuts a branch off once alpha has met beta, and how quickly
// that happens depends entirely on the order the children are examined in. Looking at
// the strongest move first raises alpha immediately and everything after it falls away;
// looking at the weakest first raises nothing and the search degenerates towards plain
// MinMax. The bounds are b^(d/2) against b^d - with perfect ordering a search reaches
// roughly twice the depth for the same work.

namespace Patterns.AlgorithmicPatterns
{
    public enum MoveOrder
    {
        // Whatever order the tree happens to carry.
        AsGiven,

        // The most promising child first - what an engine's heuristic aims for.
        BestFirst,

        // The least promising first: the pathological case.
        WorstFirst
    }

    public class MoveOrderingExample
    {
        // Alpha-beta as before, but the children of every node are examined in the
        // requested order. The value found never changes - only the work does.
        public static MinMaxSearchResult AlphaBeta(GameTree root, MoveOrder order, bool maximizing = true)
        {
            SearchCounter counter = new SearchCounter();
            int value = AlphaBeta(root, maximizing, int.MinValue, int.MaxValue, order, counter);

            return new MinMaxSearchResult(value, counter.EvaluatedLeaves, counter.PrunedBranches);
        }

        private static int AlphaBeta(
            GameTree node, bool maximizing, int alpha, int beta, MoveOrder order, SearchCounter counter)
        {
            if (node.IsLeaf)
            {
                counter.EvaluatedLeaves++;
                return node.Value;
            }

            IReadOnlyList<GameTree> children = Order(node, maximizing, order);
            int best = maximizing ? int.MinValue : int.MaxValue;

            for (int i = 0; i < children.Count; i++)
            {
                int value = AlphaBeta(children[i], !maximizing, alpha, beta, order, counter);

                if (maximizing)
                {
                    best = Math.Max(best, value);
                    alpha = Math.Max(alpha, best);
                }
                else
                {
                    best = Math.Min(best, value);
                    beta = Math.Min(beta, best);
                }

                if (alpha >= beta)
                {
                    counter.PrunedBranches += children.Count - i - 1;
                    break;
                }
            }

            return best;
        }

        // Ranks the children of a node.
        //
        // A real engine cannot know how good a move is before searching it, so it
        // guesses with something cheap - captures first, last iteration's best move
        // first. Here the whole tree is at hand, so the true value stands in for a
        // heuristic that guesses perfectly. That ranking is deliberately not counted:
        // the point is what ordering does to the search, not what the guess costs.
        private static IReadOnlyList<GameTree> Order(GameTree node, bool maximizing, MoveOrder order)
        {
            if (order == MoveOrder.AsGiven)
                return node.Children;

            List<GameTree> ranked = node.Children
                .OrderBy(child => TrueValue(child, !maximizing))
                .ToList();

            // The maximizer's most promising child is the highest scoring one, the
            // minimizer's the lowest. WorstFirst asks for exactly the opposite.
            bool highestFirst = maximizing == (order == MoveOrder.BestFirst);

            if (highestFirst)
                ranked.Reverse();

            return ranked;
        }

        // Plain MinMax value of a subtree, used only for ranking.
        private static int TrueValue(GameTree node, bool maximizing)
        {
            if (node.IsLeaf)
                return node.Value;

            int best = maximizing ? int.MinValue : int.MaxValue;

            foreach (GameTree child in node.Children)
            {
                int value = TrueValue(child, !maximizing);
                best = maximizing ? Math.Max(best, value) : Math.Min(best, value);
            }

            return best;
        }

        // A tree wide and deep enough for the orderings to pull apart. The eight leaf
        // textbook tree is far too small to show anything.
        public static GameTree RandomTree(int branchingFactor, int depth, Random random)
        {
            if (branchingFactor < 1)
                throw new ArgumentOutOfRangeException(nameof(branchingFactor), "A node needs at least one child.");

            if (depth < 0)
                throw new ArgumentOutOfRangeException(nameof(depth), "Depth cannot be negative.");

            if (depth == 0)
                return GameTree.Leaf(random.Next(0, 100));

            GameTree[] children = Enumerable.Range(0, branchingFactor)
                .Select(_ => RandomTree(branchingFactor, depth - 1, random))
                .ToArray();

            return GameTree.Node(children);
        }

        public static void MoveOrdering()
        {
            const int branchingFactor = 4;
            const int depth = 5;

            GameTree tree = RandomTree(branchingFactor, depth, new Random(42));

            MinMaxSearchResult plain = MinMaxSearchExample.MinMax(tree);

            Console.WriteLine($"One tree, {branchingFactor} moves per position, {depth} deep - {plain.EvaluatedLeaves} leaves.");
            Console.WriteLine("All four searches must agree on the value; only the work differs.");
            Console.WriteLine();
            Console.WriteLine($"  MinMax, no pruning : {plain}");
            Console.WriteLine($"  AlphaBeta as given : {AlphaBeta(tree, MoveOrder.AsGiven)}");
            Console.WriteLine($"  AlphaBeta best move first : {AlphaBeta(tree, MoveOrder.BestFirst)}");
            Console.WriteLine($"  AlphaBeta worst move first: {AlphaBeta(tree, MoveOrder.WorstFirst)}");
        }
    }
}
