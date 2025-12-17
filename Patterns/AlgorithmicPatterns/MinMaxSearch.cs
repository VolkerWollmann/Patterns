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

    internal class Move
    {
        internal SideToMove SideToMove;
        internal int PiecesToTake;
        internal int Value;

        internal Move(SideToMove sideToMove, int piecesToTake, int value)
        {
            SideToMove = sideToMove;
            PiecesToTake = piecesToTake;
            Value = value;
        }

        internal static Move WorstMove(SideToMove sideToMove)
        {
            if (sideToMove == SideToMove.Maximize)
            {
                // worst move for maximizing side is very low value
                return new Move(SideToMove.Maximize, 1, -100);
            }
            else
            {
                // worst move for minimizing side is very high value
                return new Move(SideToMove.Minimize, 1, 100);
            }
        }

        public override string ToString()
        {
            return $"{SideToMove} (Move: {PiecesToTake}, Value: {Value})";
        }
    }

    internal class MoveList : List<Move>
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

        internal void AddAtBeginning( Move move )
        {
            this.Insert(0, move);
        }
    }

    /// <summary>
    /// Take one to three.
    /// The one who (must) take the last looses
    /// </summary>
    internal class Game
    {
        public SideToMove SideToMove { get; set; } = SideToMove.Maximize;
        public Dictionary<SideToMove, IStrategy> Strategies = new ();
        
        readonly Random _random;

        public int Pieces;

        public Game(IStrategy maxStrategy, IStrategy minStrategy)
        {
            _random = new Random();
            Pieces = _random.Next(20, 30);
            Strategies.Add( SideToMove.Maximize, maxStrategy);
            Strategies.Add( SideToMove.Minimize, minStrategy);
            
        }

        internal List<Move> GetAvailableMoves()
        {
            var moves = new List<Move>();

            for (int i = 0; i < (Pieces > 3 ? 3 : Pieces); i++)
            {
                moves.Add(new Move(SideToMove, i, 0));
            }

            return moves;
        }

        internal Move GetWorstMove()
        {
            return Move.WorstMove(SideToMove);
        }

        private void ToggleSide()
        {
            SideToMove = SideToMove == SideToMove.Maximize
                ? SideToMove.Minimize
                : SideToMove.Maximize;
        }

        internal void ApplyMove(Move move )
        {
            Pieces += move.PiecesToTake;
            ToggleSide();
        }

        internal void UndoMove(Move move)
        {
            ToggleSide();
            Pieces -= move.PiecesToTake;
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
        internal int CompareMoves(Move move1, Move move2)
        {
            if (SideToMove == SideToMove.Maximize)
                return -move1.Value.CompareTo(move2.Value);
            else
                return move1.Value.CompareTo(move2.Value);
        }
    }

    internal interface IStrategy
    {
        int Evaluate(Game game);
    }
    
    internal class RandomStrategy : IStrategy
    {
        private readonly Random _random = new Random();
        public int Evaluate(Game game)
        {
            return _random.Next(-10, 10);
        }
    }

    internal class MinMaxSearch
    {
        public MoveList FindBestMove(Game game, int depth)
        {
            if (depth == 0)
            {
                return [new Move(game.SideToMove, 0, game.Pieces)];
            }

            var moves = game.GetAvailableMoves();

            Move bestMove = game.GetWorstMove();
            MoveList bestAnswerList = new MoveList();
            
            for (int i = 1; i < moves.Count; i++)
            {
                Move move = moves[i];

                game.ApplyMove(move);

                MoveList result = FindBestMove(game, depth - 1);

                game.UndoMove(move);

                if (game.CompareMoves(result[0], bestMove) > 0)
                {
                        bestMove = move;
                        bestAnswerList = result;
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
            IStrategy strategy = new RandomStrategy();
            var game = new Game(strategy, strategy);
            var search = new MinMaxSearch();
            
            MoveList bestMoveList = search.FindBestMove(game, 3);
            string result = bestMoveList.ToString();
            Console.WriteLine($"Best move {game.SideToMove}:");
            Console.WriteLine($"{result}");

        }
    }
}
