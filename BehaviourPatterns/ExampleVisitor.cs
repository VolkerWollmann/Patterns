﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourPatterns.VisitorExample
{

    public abstract class Expression
    {
        public Expression Left { get; private set; }
        public Expression Right { get; private set; }

        public Expression(Expression left, Expression right)
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
    public interface IVisitor
    {
        void DoIt(Expression expression);
    }

    public class MaxFinder : IVisitor
    {
        public int Max { get; private set; } = int.MinValue;

        public void DoIt(Expression expression)
        {
            Number n = expression as Number;
            if (n != null)
                Max = Math.Max(Max, n.Value);
        }
    }
    #endregion

    #region ITransformingVisitor

    public interface ITransformingVisitor
    {
        Expression DoIt(Expression expression);
    }

    public class Adder : ITransformingVisitor
    {
        public Expression DoIt(Expression expression)
        {
            Addition addtion = expression as Addition;
            if (addtion == null)
                return expression;

            Number left = expression.Left as Number;
            Number right = expression.Right as Number;

            if ((left != null) && (right != null))
                   return new Number(left.Value + right.Value);

            return expression;
        }
    }

    public class Multiplier : ITransformingVisitor
    {
        public Expression DoIt(Expression expression)
        {
            Multiplication addtion = expression as Multiplication;
            if (addtion == null)
                return expression;

            Number left = expression.Left as Number;
            Number right = expression.Right as Number;
            
            if ((left != null) && (right != null))
                return new Number(left.Value * right.Value);

            return expression;
        }
    }

    #endregion

    #region Tree elements
    public class Number : Expression
    {
        public int Value { get; private set; }

        public Number(int number):base(null,null)
        {
            Value = number;
        }
    }

    public class Addition : Expression
    {
        public Addition(Expression left, Expression right) : base(left, right)
        {

        }
    }

    public class Subtraction : Expression
    {
        public Subtraction(Expression left, Expression right) : base(left, right)
        {

        }
    }

    public class Multiplication : Expression
    {
        public Multiplication(Expression left, Expression right) : base(left, right)
        {

        }
    }

    public class Division : Expression
    {
        public Division(Expression left, Expression right) : base(left, right)
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

            MaxFinder maxfinder = new MaxFinder();
            expression.Accept(maxfinder);

            Assert.AreEqual<int>(maxfinder.Max, 8);
        }

        public static void TransformingVisitor()
        {
            // (1 * 3) + ( 5 * 8 )
            Expression one = new Number(1);
            Expression three = new Number(3);
            Expression five = new Number(5);
            Expression eight = new Number(8);

            Expression expression = new Addition(new Multiplication(one, three), new Multiplication(five, eight));

            List<ITransformingVisitor> vistors = new List<ITransformingVisitor>() { new Adder(), new Multiplier() };
         
            while( ! (expression is Number) )
            {
                vistors.ForEach(vistor => { expression = expression.Accept(vistor); });
            }

            Assert.AreEqual<int>((expression as Number).Value, 43);
        }


    }

    
}
