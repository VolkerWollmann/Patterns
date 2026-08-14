using System;
using System.Collections.Generic;
using System.Linq;

// Source : https://en.wikipedia.org/wiki/Minimax
//          https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning
// MinMax searches a game tree in which two players alternate: the maximizing player
// picks the highest scoring move, the minimizing opponent the lowest. Both assume the
// other plays perfectly, so the value that bubbles up to the root is the outcome of
// best play by both sides.
// Alpha-beta pruning speeds that up without ever changing the result. Alpha is the
// best score the maximizer can already force, beta the best the minimizer can. Once
// alpha meets beta, the branch being examined can no longer influence the root - a
// rational player would never let the game get there - so the remaining subtrees are
// skipped unexamined.

namespace Patterns.AlgorithmicPatterns
{
    // A node of the game tree. Leaves carry the score of a finished position,
    // inner nodes are the moves leading to them.
    public class GameTree
    {
        private static readonly GameTree[] NoChildren = Array.Empty<GameTree>();

        private GameTree(int value, IReadOnlyList<GameTree> children)
        {
            Value = value;
            Children = children;
        }

        // The score of a finished position. Only meaningful on a leaf - the value of
        // an inner node is what the search has to work out.
        public int Value { get; }

        public IReadOnlyList<GameTree> Children { get; }

        public bool IsLeaf => Children.Count == 0;

        // A finished position, scored from the maximizing player's point of view.
        public static GameTree Leaf(int value)
        {
            return new GameTree(value, NoChildren);
        }

        // A position with moves to choose from.
        public static GameTree Node(params GameTree[] children)
        {
            if (children == null || children.Length == 0)
                throw new ArgumentException("An inner node needs at least one child.", nameof(children));

            return new GameTree(0, children);
        }
    }

    // What a search found, and what it cost to find it.
    public class MinMaxSearchResult
    {
        public MinMaxSearchResult(int value, int evaluatedLeaves, int prunedBranches)
        {
            Value = value;
            EvaluatedLeaves = evaluatedLeaves;
            PrunedBranches = prunedBranches;
        }

        // The outcome of best play by both sides.
        public int Value { get; }

        // How many finished positions the search had to score.
        public int EvaluatedLeaves { get; }

        // How many child subtrees were never entered because of a cutoff.
        public int PrunedBranches { get; }

        public override string ToString()
        {
            return $"value {Value}, {EvaluatedLeaves} leaves evaluated, {PrunedBranches} branches pruned";
        }
    }

    public class MinMaxSearchExample
    {
        // Plain MinMax: every leaf of the tree is scored, no shortcuts.
        public static MinMaxSearchResult MinMax(GameTree root, bool maximizing = true)
        {
            Counter counter = new Counter();
            int value = MinMax(root, maximizing, counter);

            return new MinMaxSearchResult(value, counter.EvaluatedLeaves, counter.PrunedBranches);
        }

        // The same search, but branches that cannot influence the root are skipped.
        // The value is always identical to the one MinMax returns.
        public static MinMaxSearchResult AlphaBeta(GameTree root, bool maximizing = true)
        {
            Counter counter = new Counter();
            int value = AlphaBeta(root, maximizing, int.MinValue, int.MaxValue, counter);

            return new MinMaxSearchResult(value, counter.EvaluatedLeaves, counter.PrunedBranches);
        }

        private static int MinMax(GameTree node, bool maximizing, Counter counter)
        {
            if (node.IsLeaf)
            {
                counter.EvaluatedLeaves++;
                return node.Value;
            }

            int best = maximizing ? int.MinValue : int.MaxValue;

            foreach (GameTree child in node.Children)
            {
                int value = MinMax(child, !maximizing, counter);
                best = maximizing ? Math.Max(best, value) : Math.Min(best, value);
            }

            return best;
        }

        private static int AlphaBeta(GameTree node, bool maximizing, int alpha, int beta, Counter counter)
        {
            if (node.IsLeaf)
            {
                counter.EvaluatedLeaves++;
                return node.Value;
            }

            int best = maximizing ? int.MinValue : int.MaxValue;

            for (int i = 0; i < node.Children.Count; i++)
            {
                int value = AlphaBeta(node.Children[i], !maximizing, alpha, beta, counter);

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

                // The maximizer can already force alpha, the minimizer already beta.
                // Neither would ever choose to walk into what is left of this node,
                // so the remaining subtrees never have to be looked at.
                if (alpha >= beta)
                {
                    counter.PrunedBranches += node.Children.Count - i - 1;
                    break;
                }
            }

            return best;
        }

        // The textbook tree: the maximizer moves first, the opponent answers, and the
        // eight leaves score the resulting positions.
        //
        //                     MAX
        //            /                 \
        //          MIN                 MIN
        //        /     \             /     \
        //      MAX     MAX         MAX     MAX
        //      / \     / \         / \     / \
        //     3   5   6   9       1   2   0  -1
        //
        public static GameTree ExampleTree()
        {
            return GameTree.Node(
                GameTree.Node(
                    GameTree.Node(GameTree.Leaf(3), GameTree.Leaf(5)),
                    GameTree.Node(GameTree.Leaf(6), GameTree.Leaf(9))),
                GameTree.Node(
                    GameTree.Node(GameTree.Leaf(1), GameTree.Leaf(2)),
                    GameTree.Node(GameTree.Leaf(0), GameTree.Leaf(-1))));
        }

        // Counts the work a search does, so both variants can be compared.
        private class Counter
        {
            public int EvaluatedLeaves;
            public int PrunedBranches;
        }

        public static void MinMaxSearch()
        {
            GameTree tree = ExampleTree();

            Console.WriteLine("Searching the same game tree twice - both must agree on the value.");
            Console.WriteLine($"MinMax    : {MinMax(tree)}");
            Console.WriteLine($"AlphaBeta : {AlphaBeta(tree)}");
        }
    }
}
