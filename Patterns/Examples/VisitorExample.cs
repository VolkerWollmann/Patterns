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
    #endregion

    #region Visitors

    internal interface IVisitor
    {
        internal void VisitNumber(Number number);
        internal void VisitAddition(Addition addition);
        internal void VisitMultiplication(Multiplication multiplication);
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

        public virtual void VisitMultiplication(Multiplication expression)
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
		internal Expression VisitMultiplication(Multiplication multiplication);
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

        public virtual Expression VisitMultiplication(Multiplication expression)
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

        public static void TransformingVisitor()
        {
            // Arrange : (3 + ( 4 * 5 ) ) + ( 5 * ( 8 * 2 ) )
            Expression four= new Number(4);
            Expression three = new Number(3);
            Expression five = new Number(5);
            Expression eight = new Number(8);
            Expression two = new Number(2);

            Expression expression = new Addition(
                new Addition(   
	                three, 
	                new Multiplication(
							four, 
							five)),
                new Multiplication(
                    five, 
                    new Multiplication( 
								eight, 
								two)));

            List<TransformingVisitor> visitors = [new Adder(), new Multiplier()];
         
            // Act : Calculate the expression with the visitors
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while( ! (expression is Number) )
            {
                visitors.ForEach(visitor => { expression = expression.Accept(visitor); });
            }

            // Assert : Visitors did the right job
            Assert.AreEqual(((Number)expression).Value, 103);
        }
    }
}
