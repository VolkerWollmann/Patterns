using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Testing.Platform.Extensions.Messages;
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
        internal MoveList StrongestVariant = [];
        internal Move(SideToMove sideToMove, int piecesToTake, int value)
        {
            SideToMove = sideToMove;
            PiecesToTake = piecesToTake;
            Value = value;
        }

        /// <summary>
        /// For evaluation of board only
        /// </summary>
        /// <param name="sideToMove"></param>
        /// <param name="value"></param>
        internal Move(SideToMove sideToMove, int value) : this(sideToMove, -1, value)
        {
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

        internal void AddReply(Move replyMove)
        {
            this.Value = replyMove.Value;
            this.StrongestVariant = replyMove.StrongestVariant;
        }

        public override string ToString()
        {
            string sideToMove = SideToMove.ToString().Substring(0,3);
            string s = $"{sideToMove} (Move: {PiecesToTake}, Value: {Value})";
            return s;
        }

        private string ShortString()
        {
            string sideToMove = SideToMove.ToString().Substring(0, 3);
            return $"({sideToMove}: {PiecesToTake} Val: {Value} )";
        }

        public string ToStringWithResponse()
        {
            string s = ToString();
            s += "[";
            foreach (var move in StrongestVariant)
            {
                s = s + move.ShortString();
            }

            s += "]";
            return s;
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
            Pieces = 15;
            Strategies.Add( SideToMove.Maximize, maxStrategy);
            Strategies.Add( SideToMove.Minimize, minStrategy);
            
        }

        internal List<Move> GetAvailableMoves()
        {
            var moves = new List<Move>();

            for (int i = 1; i <= (Pieces > 3 ? 3 : Pieces); i++)
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
            Pieces -= move.PiecesToTake;
            ToggleSide();
        }

        internal void UndoMove(Move move)
        {
            ToggleSide();
            Pieces += move.PiecesToTake;
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
            int result;
            if (SideToMove == SideToMove.Maximize)
                result = move1.Value.CompareTo(move2.Value);
            else
                result = -move1.Value.CompareTo(move2.Value);

            return result;
        }

        internal int Evaluate()
        {
            int value = Strategies[SideToMove].Evaluate(this);
            return value;
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
            if (game.Pieces == 1)
            {
                return (game.SideToMove == SideToMove.Maximize) ? -10 : 10;
            }
            
            return _random.Next(-10, 10);
        }
    }

    internal class StrongStrategy : IStrategy
    {
        private readonly Random _random = new Random();
        public int Evaluate(Game game)
        {
            if (game.Pieces == 1)
            {
                return (game.SideToMove == SideToMove.Maximize) ? -10 : 10;
            }

            if (game.Pieces % 4 == 1)
            {
                return (game.SideToMove == SideToMove.Maximize) ? -10 : 10;
            }
            else
            {
                return (game.SideToMove == SideToMove.Maximize) ? 10 : -10;
            }    
        }
    }

    internal class MinMaxSearch
    {
        public Move FindBestMove(Game game, int depth)
        {
            if ( (depth == 0) || (game.Pieces == 0))
            {
                return new Move(game.SideToMove, game.Evaluate());
            }

            var moves = game.GetAvailableMoves();

            Move bestMove = game.GetWorstMove();
            
            for (int i = 1; i < moves.Count; i++)
            {
                Move move = moves[i];

                game.ApplyMove(move);

                Move replyMove = FindBestMove(game, depth - 1);

                game.UndoMove(move);

                if (game.CompareMoves(replyMove, bestMove) > 0)
                {
                        bestMove = move;
                        bestMove.AddReply(replyMove);
                        bestMove.StrongestVariant.AddAtBeginning(bestMove);
                }
            }

            return bestMove;
        }
    }
    public class MinMaxSearchExample
    {
        public static void Test()
        { 
            var game = new Game(new StrongStrategy(), new RandomStrategy());
            var search = new MinMaxSearch();
            
            Move bestMoveList = search.FindBestMove(game, 4);
            string result = bestMoveList.ToStringWithResponse();
            Console.WriteLine($"Best move {game.SideToMove}:");
            Console.WriteLine($"{result}");

        }
    }
}
