using System;

namespace Patterns.AlgorithmicPatterns
{
    public class AlphaBetaExample
    {
        // Returns the best value for the maximizing player
        public static int AlphaBeta(int[] values, int depth, int nodeIndex,
            bool maximizingPlayer, int alpha, int beta)
        {
            // Leaf node reached
            if (depth == 0)
                return values[nodeIndex];

            if (maximizingPlayer)
            {
                int best = int.MinValue;

                // Two children: left and right
                for (int i = 0; i < 2; i++)
                {
                    int childValue = AlphaBeta(
                        values,
                        depth - 1,
                        nodeIndex * 2 + i,
                        false,
                        alpha,
                        beta
                    );

                    best = Math.Max(best, childValue);
                    alpha = Math.Max(alpha, best);

                    // Beta cutoff
                    if (beta <= alpha)
                    {
                        Console.WriteLine("Pruned branch at node " + nodeIndex);
                        break;
                    }
                }

                return best;
            }
            else
            {
                int best = int.MaxValue;

                for (int i = 0; i < 2; i++)
                {
                    int childValue = AlphaBeta(
                        values,
                        depth - 1,
                        nodeIndex * 2 + i,
                        true,
                        alpha,
                        beta
                    );

                    best = Math.Min(best, childValue);
                    beta = Math.Min(beta, best);

                    // Alpha cutoff
                    if (beta <= alpha)
                    {
                        Console.WriteLine("Pruned branch at node " + nodeIndex);
                        break;
                    }
                }

                return best;
            }
        }

        public static void Example()
        {
            /*
                 Maximizer
                     A
                  /     \
               Min       Min
              /  \       /  \
             3    5     6    9

            Leaf values are stored left-to-right.
            */

            int depth = 4;
            Random random = new Random();

            int[] leafValues = Enumerable.Range(0, (int)Math.Pow(2, depth))
                .Select(_ => random.Next(0, 100)) // 0–99
                .ToList().ToArray();

            int result = AlphaBeta(
                leafValues,
                depth,
                0,
                true,
                int.MinValue,
                int.MaxValue
            );

            Console.WriteLine("Optimal value: " + result);
        }
    }
}