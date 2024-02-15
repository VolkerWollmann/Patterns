using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Patterns.Examples
{

    internal abstract class Expression
    {
        internal Expression Left { get; set; }
        internal Expression Right { get; set; }

        protected Expression(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }

        internal abstract void Accept(Visitor visitor);

        internal abstract Expression Accept(TransformingVisitor visitor);
    }

    #region Tree elements
    [DebuggerDisplay("Number={Value}")]
    internal class Number : Expression
    {
        public int Value { get; }

        public Number(int number):base(null,null)
        {
            Value = number;
        }


        internal override void Accept(Visitor visitor)
        {
            visitor.DoIt(this);
        }

        internal override Expression Accept(TransformingVisitor visitor)
        {
            return visitor.DoIt(this);
        }
    }

    [DebuggerDisplay("Addition")]
    internal class Addition : Expression
    {
        public Addition(Expression left, Expression right) : base(left, right)
        {

        }

        internal override void Accept(Visitor visitor)
        {
            visitor.DoIt(this);
        }

        internal override Expression Accept(TransformingVisitor visitor)
        {
            return visitor.DoIt(this);
        }
    }

    [DebuggerDisplay("Multiplication")]
    internal class Multiplication : Expression
    {
        public Multiplication(Expression left, Expression right) : base(left, right)
        {

        }

        internal override void Accept(Visitor visitor)
        {
            visitor.DoIt(this);
        }

        internal override Expression Accept(TransformingVisitor visitor)
        {
            return visitor.DoIt(this);
        }
    }
    #endregion

    #region Visitor
    internal abstract class Visitor
    {
        internal virtual void DoIt(Number expression)
        {
            DoIt((Expression)expression);
        }

        internal virtual void DoIt(Addition expression)
        {
            DoIt((Expression)expression);
        }

        internal virtual void DoIt(Multiplication expression)
        {
            DoIt((Expression)expression);
        }

        internal virtual void DoIt(Expression expression)
        {
            if (expression.Left != null)
                expression.Left.Accept(this);
            if (expression.Right != null)
                expression.Right.Accept(this);
        }
    }

    internal class MaxFinder : Visitor
    {
        public int Max { get; private set; } = int.MinValue;

        internal override void DoIt(Number expression)
        {
            if (expression.Value > Max)
                Max = expression.Value;
        }
    }
    #endregion

    #region TransformingVisitor

    internal abstract class TransformingVisitor
    {
        internal virtual Expression DoIt(Number expression)
        {
            return DoIt((Expression) expression);
        }

        internal virtual Expression DoIt(Addition expression)
        {
            return DoIt((Expression)expression);
        }

        internal virtual Expression DoIt(Multiplication expression)
        {
            return DoIt((Expression)expression);
        }

        internal Expression DoIt(Expression expression)
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
        internal override Expression DoIt(Addition expression)
        {
            
            if ((expression.Left is Number left) && (expression.Right is Number right))
                return new Number(left.Value + right.Value);

            base.DoIt((Expression)expression);
            return expression;
        }
    }

    internal class Multiplier : TransformingVisitor
    {
        internal override Expression DoIt(Multiplication expression)
        {
            if ((expression.Left is Number left) && (expression.Right is Number right))
                return new Number(left.Value * right.Value);

            base.DoIt((Expression)expression);
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
                new Addition(three, new Multiplication(four, five)),
                new Multiplication(
                    five, 
                    new Multiplication( eight, two)));

            List<TransformingVisitor> visitors = new List<TransformingVisitor>() { new Adder(), new Multiplier() };
         
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
