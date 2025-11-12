using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Patterns.Examples
{
    #region Tree elements

    #region Accept interfaces
    internal interface IAcceptVisitor
    {
        internal void Accept(Visitor visitor);
    }

    internal interface IAcceptTransformingVisitor
    {
        internal Expression Accept(TransformingVisitor visitor);
    }
    #endregion

    internal abstract class Expression(Expression? left, Expression? right) : IAcceptVisitor, IAcceptTransformingVisitor
    {
        internal Expression? Left { get; set; } = left;
        internal Expression? Right { get; set; } = right;

        public abstract void Accept(Visitor visitor);

        public abstract Expression Accept(TransformingVisitor visitor);
    }

    [DebuggerDisplay("{Value}")]
    internal class Number(int number) : Expression(null, null)
    {
        public int Value { get; } = number;


        public override void Accept(Visitor visitor)
        {
            visitor.VisitNumber(this);
        }

        public override Expression Accept(TransformingVisitor visitor)
        {
            return visitor.VisitNumber(this);
        }
    }

    [DebuggerDisplay("( {Left} + {Right} )")]
    internal class Addition(Expression left, Expression right) : Expression(left, right)
    {
        public override void Accept(Visitor visitor)
        {
            visitor.VisitAddition(this);
        }

        public override Expression Accept(TransformingVisitor visitor)
        {
            return visitor.VisitAddition(this);
        }
    }

    [DebuggerDisplay("( {Left} - {Right} )")]
    internal class Subtraction : Expression
    {
        public Subtraction(Expression left, Expression right) : base(left, right) { }
        public override void Accept(Visitor visitor)
        {
            visitor.VisitSubtraction(this);
        }
        public override Expression Accept(TransformingVisitor visitor)
        {
            return visitor.VisitSubtraction(this);
        }
    }

    [DebuggerDisplay("( {Left} * {Right} )")]
    internal class Multiplication(Expression left, Expression right) : Expression(left, right)
    {
        public override void Accept(Visitor visitor)
        {
            visitor.VisitMultiplication(this);
        }

        public override Expression Accept(TransformingVisitor visitor)
        {
            return visitor.VisitMultiplication(this);
        }
    }

    [DebuggerDisplay("( {Left} / {Right} )")]
    internal class Division(Expression left, Expression right) : Expression(left, right)
    {
        public override void Accept(Visitor visitor)
        {
            visitor.VisitDivision(this);
        }
        public override Expression Accept(TransformingVisitor visitor)
        {
            return visitor.VisitDivision(this);
        }
    }

    #endregion

    #region Visitors

    internal interface IVisitor
    {
        internal void VisitNumber(Number number);
        internal void VisitAddition(Addition addition);
        internal void VisitSubtraction(Subtraction subtraction);
        internal void VisitMultiplication(Multiplication multiplication);
        internal void VisitDivision(Division division);
        internal void VisitExpression(Expression expression);
    }
    internal abstract class Visitor : IVisitor
    {
        public virtual void VisitNumber(Number expression)
        {
            VisitExpression((Expression)expression);
        }

        public virtual void VisitAddition(Addition expression)
        {
	        VisitExpression((Expression)expression);
        }

        public virtual void VisitSubtraction(Subtraction expression)
        {
            VisitExpression((Expression)expression);
        }


        public virtual void VisitMultiplication(Multiplication expression)
        {
	        VisitExpression((Expression)expression);
        }

        public virtual void VisitDivision(Division expression)
        {
            VisitExpression((Expression)expression);
        }

        public virtual void VisitExpression(Expression expression)
        {
            if (expression.Left != null)
                expression.Left.Accept(this);
            if (expression.Right != null)
                expression.Right.Accept(this);
        }
    }

    #region MaxFinder Visitor
    internal class MaxFinder : Visitor
    {
        public int Max { get; private set; } = int.MinValue;

        public override void VisitNumber(Number expression)
        {
            if (expression.Value > Max)
                Max = expression.Value;
        }
    }
    #endregion
    #endregion

    #region TransformingVisitor

    internal interface ITransformingVisitor
    {
		internal Expression VisitNumber(Number number);
		internal Expression VisitAddition(Addition addition);

        internal Expression VisitSubtraction(Subtraction subtraction);
        internal Expression VisitMultiplication(Multiplication multiplication);
        internal Expression VisitDivision(Division division);
        internal Expression VisitExpression(Expression expression);
	}
    internal abstract class TransformingVisitor : ITransformingVisitor
    {
        public virtual Expression VisitNumber(Number expression)
        {
            return VisitExpression((Expression) expression);
        }

        public virtual Expression VisitAddition(Addition expression)
        {
            return VisitExpression((Expression)expression);
        }

        public virtual Expression VisitSubtraction(Subtraction expression)
        {
            return VisitExpression((Expression)expression);
        }

        public virtual Expression VisitMultiplication(Multiplication expression)
        {
            return VisitExpression((Expression)expression);
        }
        public virtual Expression VisitDivision(Division expression)
        {
            return VisitExpression((Expression)expression);
        }
        public Expression VisitExpression(Expression expression)
        {
            if (expression.Left != null)
                expression.Left = expression.Left.Accept(this);
            if (expression.Right != null)
                expression.Right = expression.Right.Accept(this);

            return expression;
        }
    }

    internal class Adder : TransformingVisitor
    {
        public override Expression VisitAddition(Addition expression)
        {
            
            if ((expression.Left is Number left) && (expression.Right is Number right))
                return new Number(left.Value + right.Value);

            base.VisitExpression((Expression)expression);
            return expression;
        }
    }

    internal class Subtractor : TransformingVisitor
    {
        public override Expression VisitSubtraction(Subtraction expression)
        {

            if ((expression.Left is Number left) && (expression.Right is Number right))
                return new Number(left.Value - right.Value);

            base.VisitExpression((Expression)expression);
            return expression;
        }
    }

    internal class Multiplier : TransformingVisitor
    {
        public override Expression VisitMultiplication(Multiplication expression)
        {
            if ((expression.Left is Number left) && (expression.Right is Number right))
                return new Number(left.Value * right.Value);

            base.VisitExpression((Expression)expression);
            return expression;
        }
    }

    internal class Divider : TransformingVisitor
    {
        public override Expression VisitDivision(Division expression)
        {
            if ((expression.Left is Number left) && (expression.Right is Number right))
            {
                if (right.Value == 0)
                    throw new DivideByZeroException("Division by zero is not allowed.");
                return new Number(left.Value / right.Value);
            }
            base.VisitExpression((Expression)expression);
            return expression;
        }
    }

    #endregion

    public class VisitorExample
    {
        public static void SimpleVisitor()
        {
            // 3 + ( 5 * 8 )
            Expression three = new Number(3);
            Expression five = new Number(5);
            Expression eight = new Number(8);

            Expression expression = new Addition(three, new Multiplication(five, eight));

            MaxFinder maxFinder = new MaxFinder();
            expression.Accept(maxFinder);

            Assert.AreEqual(maxFinder.Max, 8);
        }

        #region parser
        // Parser class
        internal class ExpressionParser
		{
			private readonly Queue<string> _tokens;
			public ExpressionParser(string input)
			{
				_tokens = new Queue<string>(Tokenize(input));
			}
			private IEnumerable<string> Tokenize(string input)
			{
				var tokens = new List<string>();
				var current = string.Empty;
				foreach (var c in input)
				{
					if (char.IsWhiteSpace(c)) continue;
					if (char.IsDigit(c))
					{
						current += c;
					}
					else
					{
						if (!string.IsNullOrEmpty(current))
						{
							tokens.Add(current);
							current = string.Empty;
						}
						tokens.Add(c.ToString());
					}
				}
				if (!string.IsNullOrEmpty(current))
				{
					tokens.Add(current);
				}
				return tokens;
			}
			public Expression Parse()
			{
				return ParseExpression();
			}
			private Expression ParseExpression()
			{
				var left = ParseTerm();
				while (_tokens.Count > 0 && (_tokens.Peek() == "+" || _tokens.Peek() == "-"))
				{
					var op = _tokens.Dequeue();
					var right = ParseTerm();
					if (op == "+")
					{
						left = new Addition(left, right);
					}
                    else if (op == "-")
                    {
                        left = new Subtraction(left, right);
                    }
                }
				return left;
			}
			private Expression ParseTerm()
			{
				var left = ParseFactor();
                // Handle multiplication and division with stronger binding
                while (_tokens.Count > 0 && (_tokens.Peek() == "*" || _tokens.Peek() == "/"))
				{
					var op = _tokens.Dequeue();
					var right = ParseFactor();
					if (op == "*")
					{
						left = new Multiplication(left, right);
					}
				}
				return left;
			}
			private Expression ParseFactor()
			{
				var token = _tokens.Dequeue();
				if (token == "(")
				{
					var expression = ParseExpression();
					_tokens.Dequeue(); // Consume ")"
					return expression;
				}
				return new Number(int.Parse(token));
			}
		}
        #endregion

        public static void TransformingVisitor(string expressionString, int expectedResult)
        {
			var parser = new ExpressionParser(expressionString);
			var expression = parser.Parse();

			List<TransformingVisitor> visitors = [new Adder(), new Multiplier(), new Subtractor()];
         
            // Act : Calculate the expression with the visitors
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while( ! (expression is Number) )
            {
                visitors.ForEach(visitor => { expression = expression.Accept(visitor); });
            }

            // Assert : Visitors did the right job
            Assert.AreEqual(((Number)expression).Value, expectedResult);
        }
    }
}
