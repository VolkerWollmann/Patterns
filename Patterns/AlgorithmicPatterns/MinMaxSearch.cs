using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Patterns.Examples;

namespace Patterns.AlgorithmicPatterns
{
    internal enum SideToMove
    {
        Maximize,
        Minimize
    }

    internal class EvaluatedMove
    {
        internal SideToMove SideToMove;
        internal int Move;
        internal int Value;

        internal EvaluatedMove(SideToMove sideToMove, int move, int value)
        {
            SideToMove = sideToMove;
            Move = move;
            Value = value;
        }

        internal static EvaluatedMove WorstMove(SideToMove sideToMove)
        {
            if (sideToMove == SideToMove.Maximize)
                return new EvaluatedMove("Worst for Maximize", 0, -100);
            else
                return new EvaluatedMove("Worst for Minimize", 0, 100);
        }

        public override string ToString()
        {
            return $"{MoveName} (Move: {Move}, Value: {Value})";
        }
    }

    internal class MoveList : List<EvaluatedMove>
    {
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var move in this)
            {
                sb.AppendLine(move.ToString());
            }
            return sb.ToString();
        }

        internal void AddAtBeginning( EvaluatedMove move )
        {
            this.Insert(0, move);
        }
    }

    internal class Game
    {
        public SideToMove SideToMove { get; set; } = SideToMove.Maximize;
        Random random = new Random();

        public int Value;

        internal List<EvaluatedMove> GetAvailableMoves()
        {
            var moves = new List<EvaluatedMove>();
            for (int i = 0; i < 3; i++)
            {
                // generate inclusive range -10 .. 10
                moves.Add( new EvaluatedMove("Move " + i,  random.Next(-10, 11), 0));
            }
            return moves;
        }

        internal EvaluatedMove GetWorstMove()
        {
            return EvaluatedMove.WorstMove(SideToMove);
        }

        private void ToggleSide()
        {
            SideToMove = SideToMove == SideToMove.Maximize
                ? SideToMove.Minimize
                : SideToMove.Maximize;
        }

        internal void ApplyMove(EvaluatedMove move )
        {
            Value += move.Move;
            ToggleSide();
        }

        internal void UndoMove(EvaluatedMove move)
        {
            ToggleSide();
            Value -= move.Move;
        }


        /// <summary>
        /// Returns comparison of two moves based on side to move.
        /// > 0    : if move1 is better than move2 in favor for side to move
        /// = 0    : if equal
        /// less 0 : if move2 is better than move1 in favor for side to move
        /// </summary>
        /// <param name="move1"></param>
        /// <param name="move2"></param>
        /// <returns></returns>
        internal int CompareMoves(int move1, int move2)
        {
            if (SideToMove == SideToMove.Maximize)
                return -move1.CompareTo(move2);
            else
                return move1.CompareTo(move2);
        }
    }

    internal class MinMaxSearch
    {
        public MoveList FindBestMove(Game game, int depth)
        {
            if (depth == 0)
            {
                return [new EvaluatedMove("End", 0, game.Value)];
            }

            var moves = game.GetAvailableMoves();

            EvaluatedMove bestMove = game.GetWorstMove();
            MoveList bestAnswerList = new MoveList();
            
            for (int i = 1; i < moves.Count; i++)
            {
                EvaluatedMove move = moves[i];

                game.ApplyMove(move);

                MoveList result = FindBestMove(game, depth - 1);

                game.UndoMove(move);

                if (game.SideToMove == SideToMove.Maximize)
                {
                    if (result[0].Value > bestMove.Value)
                    {
                        bestMove = move;
                        bestAnswerList = result;
                    }
                }
                else
                {
                    if (result[0].Value < bestMove.Value)
                    {
                        bestMove = move;
                        bestAnswerList = result;
                    }
                }
            }

            bestAnswerList.AddAtBeginning(bestMove);
            return bestAnswerList;
        }
    }
    public class MinMaxSearchExample
    {
        public static void Test()
        {
            var game = new Game();
            var search = new MinMaxSearch();
            
            MoveList bestMoveList = search.FindBestMove(game, 2);
            string result = bestMoveList.ToString();
            Console.WriteLine("Best move (maximizing):");
            Console.WriteLine($"{result}");

        }
    }
}
