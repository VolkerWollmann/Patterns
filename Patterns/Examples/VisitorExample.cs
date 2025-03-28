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
            visitor.Visit(this);
        }

        public override Expression Accept(TransformingVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    [DebuggerDisplay("( {Left} + {Right} )")]
    internal class Addition(Expression left, Expression right) : Expression(left, right)
    {
        public override void Accept(Visitor visitor)
        {
            visitor.Visit((Expression) this);
        }

        public override Expression Accept(TransformingVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    [DebuggerDisplay("( {Left} * {Right} )")]
    internal class Multiplication(Expression left, Expression right) : Expression(left, right)
    {
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }

        public override Expression Accept(TransformingVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
    #endregion

    #region Visitors

    internal interface IVisitor
    {
        internal void Visit(Number number);
        internal void Visit(Addition addition);
        internal void Visit(Multiplication multiplication);
        internal void Visit(Expression expression);
    }
    internal abstract class Visitor : IVisitor
    {
        public virtual void Visit(Number expression)
        {
            Visit((Expression)expression);
        }

        public virtual void Visit(Addition expression)
        {
            Visit((Expression)expression);
        }

        public virtual void Visit(Multiplication expression)
        {
            Visit((Expression)expression);
        }

        public virtual void Visit(Expression expression)
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

        public override void Visit(Number expression)
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
        internal Expression Visit(Number number);
        internal Expression Visit(Addition addition);
        internal Expression Visit(Multiplication multiplication);
        internal Expression Visit(Expression expression);
    }
    internal abstract class TransformingVisitor : ITransformingVisitor
    {
        public virtual Expression Visit(Number expression)
        {
            return Visit((Expression) expression);
        }

        public virtual Expression Visit(Addition expression)
        {
            return Visit((Expression)expression);
        }

        public virtual Expression Visit(Multiplication expression)
        {
            return Visit((Expression)expression);
        }

        public Expression Visit(Expression expression)
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
        public override Expression Visit(Addition expression)
        {
            
            if ((expression.Left is Number left) && (expression.Right is Number right))
                return new Number(left.Value + right.Value);

            base.Visit((Expression)expression);
            return expression;
        }
    }

    internal class Multiplier : TransformingVisitor
    {
        public override Expression Visit(Multiplication expression)
        {
            if ((expression.Left is Number left) && (expression.Right is Number right))
                return new Number(left.Value * right.Value);

            base.Visit((Expression)expression);
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
