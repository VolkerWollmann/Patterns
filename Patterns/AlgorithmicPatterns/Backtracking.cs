using System;
using System.Collections.Generic;
using System.Text;

// Source : https://en.wikipedia.org/wiki/Backtracking
//          https://en.wikipedia.org/wiki/Eight_queens_puzzle
// Backtracking builds a solution one decision at a time and takes a decision back as
// soon as it cannot lead anywhere: choose - explore - undo. That undo is what makes it
// backtracking rather than brute force, and abandoning a partial solution early prunes
// away every complete solution that would have started with it.
// The eight queens puzzle is the classic case: place one queen per row so that no two
// share a column or a diagonal.

namespace Patterns.AlgorithmicPatterns
{
    public enum BacktrackingStepKind
    {
        // A queen was placed on a safe square.
        Place,

        // No safe square was left in this row - the reason a search backtracks.
        DeadEnd,

        // A queen was taken off again, undoing an earlier decision.
        Backtrack,

        // Every row is filled.
        Solution
    }

    // One decision the search took, so the choose - explore - undo cycle can be
    // watched from outside.
    public class BacktrackingStep
    {
        public BacktrackingStep(BacktrackingStepKind kind, int row, int column)
        {
            Kind = kind;
            Row = row;
            Column = column;
        }

        public BacktrackingStepKind Kind { get; }

        public int Row { get; }

        // The column involved, or -1 where no square was left.
        public int Column { get; }

        public override string ToString()
        {
            // Indenting by row draws the search tree as it is walked.
            string indent = new string(' ', Row * 2);

            switch (Kind)
            {
                case BacktrackingStepKind.Place:
                    return $"{indent}row {Row}: queen to column {Column}";

                case BacktrackingStepKind.DeadEnd:
                    return $"{indent}row {Row}: no safe column left";

                case BacktrackingStepKind.Backtrack:
                    return $"{indent}row {Row}: take the queen back off column {Column}";

                case BacktrackingStepKind.Solution:
                    return $"{indent}all rows filled - solution";

                default:
                    return $"{indent}{Kind}";
            }
        }
    }

    // The solutions found, and what it cost to find them.
    public class BacktrackingResult
    {
        public BacktrackingResult(IReadOnlyList<int[]> solutions, int exploredPositions)
        {
            Solutions = solutions;
            ExploredPositions = exploredPositions;
        }

        // One entry per solution. Index is the row, value the column the queen sits in.
        public IReadOnlyList<int[]> Solutions { get; }

        // How many squares the search looked at before settling.
        public int ExploredPositions { get; }

        public int Count => Solutions.Count;

        public override string ToString()
        {
            return $"{Count} solutions, {ExploredPositions} positions explored";
        }
    }

    public class BacktrackingExample
    {
        // Places boardSize queens on a boardSize x boardSize board.
        // maxSolutions stops the search early - useful when one solution is enough.
        // onStep reports every decision, including the ones taken back again.
        public static BacktrackingResult Solve(
            int boardSize,
            int maxSolutions = int.MaxValue,
            Action<BacktrackingStep>? onStep = null)
        {
            if (boardSize < 0)
                throw new ArgumentOutOfRangeException(nameof(boardSize), "A board cannot have a negative size.");

            if (maxSolutions < 0)
                throw new ArgumentOutOfRangeException(nameof(maxSolutions), "Cannot ask for a negative number of solutions.");

            List<int[]> solutions = new List<int[]>();
            int[] queens = new int[boardSize];
            Counter counter = new Counter();

            Place(0, queens, solutions, counter, boardSize, maxSolutions, onStep);

            return new BacktrackingResult(solutions, counter.ExploredPositions);
        }

        private static void Place(
            int row, int[] queens, List<int[]> solutions, Counter counter,
            int boardSize, int maxSolutions, Action<BacktrackingStep>? onStep)
        {
            if (solutions.Count >= maxSolutions)
                return;

            // Every row is filled - this is a complete solution.
            if (row == boardSize)
            {
                solutions.Add((int[])queens.Clone());
                onStep?.Invoke(new BacktrackingStep(BacktrackingStepKind.Solution, row, -1));
                return;
            }

            bool placedAnyQueen = false;

            for (int column = 0; column < boardSize; column++)
            {
                counter.ExploredPositions++;

                // The square is attacked, so no complete solution can start this way.
                // Skipping it prunes the entire subtree below it.
                if (!IsSafe(row, column, queens))
                    continue;

                placedAnyQueen = true;

                // choose
                queens[row] = column;
                onStep?.Invoke(new BacktrackingStep(BacktrackingStepKind.Place, row, column));

                // explore
                Place(row + 1, queens, solutions, counter, boardSize, maxSolutions, onStep);

                // undo - this is what makes it backtracking
                queens[row] = 0;
                onStep?.Invoke(new BacktrackingStep(BacktrackingStepKind.Backtrack, row, column));

                if (solutions.Count >= maxSolutions)
                    return;
            }

            // Nowhere left to go in this row, so the caller has to take its own
            // decision back.
            if (!placedAnyQueen)
                onStep?.Invoke(new BacktrackingStep(BacktrackingStepKind.DeadEnd, row, -1));
        }

        // A square is safe when no queen placed in an earlier row shares its column or
        // one of its diagonals. Rows never clash, because only one queen goes per row.
        public static bool IsSafe(int row, int column, int[] queens)
        {
            for (int earlierRow = 0; earlierRow < row; earlierRow++)
            {
                int earlierColumn = queens[earlierRow];

                if (earlierColumn == column)
                    return false;

                // Same diagonal: the row distance equals the column distance.
                if (Math.Abs(earlierColumn - column) == row - earlierRow)
                    return false;
            }

            return true;
        }

        // Draws a solution as a board, one row per line.
        public static string Format(int[] solution)
        {
            StringBuilder board = new StringBuilder();

            foreach (int queenColumn in solution)
            {
                for (int column = 0; column < solution.Length; column++)
                {
                    board.Append(column == queenColumn ? " Q" : " .");
                }

                board.AppendLine();
            }

            return board.ToString();
        }

        // Counts the work the search does.
        private class Counter
        {
            public int ExploredPositions;
        }

        public static void Backtracking()
        {
            // Small enough to follow every decision. Eight queens would be some
            // fifteen thousand steps.
            const int tracedSize = 4;

            Console.WriteLine($"Tracing {tracedSize} queens - indentation is the row being filled:");
            BacktrackingResult traced = Solve(tracedSize, onStep: step => Console.WriteLine(step));
            Console.WriteLine($"{tracedSize} queens: {traced}");
            Console.WriteLine();

            const int boardSize = 8;

            BacktrackingResult result = Solve(boardSize);

            // Placing one queen per row without ever checking would mean trying every
            // combination; pruning cuts that down to the positions actually explored.
            double bruteForce = Math.Pow(boardSize, boardSize);

            Console.WriteLine($"{boardSize} queens: {result}");
            Console.WriteLine($"Placing blindly would have meant {bruteForce:N0} boards.");
            Console.WriteLine();
            Console.WriteLine("First solution:");
            Console.Write(Format(result.Solutions[0]));
        }
    }
}
