﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Patterns.Examples
{

    internal abstract class Expression
    {
        public Expression Left { get; private set; }
        public Expression Right { get; private set; }

        protected Expression(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }

        public void Accept(IVisitor visitor)
        {
            if (Left != null)
                Left.Accept(visitor);
            if (Right != null)
                Right.Accept(visitor);
            visitor.DoIt(this);
        }

        public Expression Accept(ITransformingVisitor visitor)
        {
            if (Left != null)
                Left = Left.Accept(visitor);
            if (Right != null)
                Right = Right.Accept(visitor);
            return visitor.DoIt(this);
        }
    }

    #region IVisitor
    internal interface IVisitor
    {
        void DoIt(Expression expression);
    }

    internal class MaxFinder : IVisitor
    {
        public int Max { get; private set; } = int.MinValue;

        public void DoIt(Expression expression)
        {
            if (expression is Number n)
                Max = Math.Max(Max, n.Value);
        }
    }
    #endregion

    #region ITransformingVisitor

    internal interface ITransformingVisitor
    {
        Expression DoIt(Expression expression);
    }

    internal class Adder : ITransformingVisitor
    {
        public Expression DoIt(Expression expression)
        {
            if (!(expression is Addition))
                return expression;

            if ((expression.Left is Number left) && (expression.Right is Number right))
                   return new Number(left.Value + right.Value);

            return expression;
        }
    }

    internal class Multiplier : ITransformingVisitor
    {
        public Expression DoIt(Expression expression)
        {
            if (!(expression is Multiplication))
                return expression;

            if ((expression.Left is Number left) && (expression.Right is Number right))
                return new Number(left.Value * right.Value);

            return expression;
        }
    }

    #endregion

    #region Tree elements
    internal class Number : Expression
    {
        public int Value { get; }

        public Number(int number):base(null,null)
        {
            Value = number;
        }
    }

    internal class Addition : Expression
    {
        public Addition(Expression left, Expression right) : base(left, right)
        {

        }
    }

    internal class Multiplication : Expression
    {
        public Multiplication(Expression left, Expression right) : base(left, right)
        {

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
            // (1 * 3) + ( 5 * 8 )
            Expression one = new Number(1);
            Expression three = new Number(3);
            Expression five = new Number(5);
            Expression eight = new Number(8);

            Expression expression = new Addition(new Multiplication(one, three), new Multiplication(five, eight));

            List<ITransformingVisitor> visitors = new List<ITransformingVisitor>() { new Adder(), new Multiplier() };
         
            while( ! (expression is Number) )
            {
                visitors.ForEach(visitor => { expression = expression.Accept(visitor); });
            }

            Assert.AreEqual(((Number)expression).Value, 43);
        }


    }

    
}
