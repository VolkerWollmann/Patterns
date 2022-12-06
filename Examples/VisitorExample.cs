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
        internal abstract void DoIt(Number expression);

        internal abstract void DoIt(Addition expression);

        internal abstract void DoIt(Multiplication expression);

        internal void DoIt(Expression expression)
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

        internal override void DoIt(Addition expression)
        {
            this.DoIt((Expression)expression);
        }

        internal override void DoIt(Multiplication expression)
        {
            this.DoIt((Expression)expression);
        }
    }
    #endregion

    #region TransformingVisitor

    internal abstract class TransformingVisitor
    {
        internal abstract Expression DoIt(Number expression);

        internal abstract Expression DoIt(Addition expression);
        
        internal abstract Expression DoIt(Multiplication expression);

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
        internal override Expression DoIt(Number expression)
        {
            return expression;
        }

        internal override Expression DoIt(Addition expression)
        {
            
            if ((expression.Left is Number left) && (expression.Right is Number right))
                return new Number(left.Value + right.Value);

            base.DoIt((Expression)expression);
            return expression;
        }

        internal override Expression DoIt(Multiplication expression)
        {
            base.DoIt((Expression)expression);
            return expression;
        }
    }

    internal class Multiplier : TransformingVisitor
    {
        internal override Expression DoIt(Number expression)
        {
            return expression;
        }

        internal override Expression DoIt(Addition expression)
        {
            base.DoIt((Expression)expression);
            return expression;

        }

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
