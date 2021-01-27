using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        public void Visit(IVisitor visitor)
        {
            visitor.DoIt(Left);
            visitor.DoIt(Right);
        }

        public Expression Visit(ITransformingVisitor visitor)
        {
            Left = visitor.DoIt(Left);
            Right = visitor.DoIt(Right);

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
            expression.Visit(maxfinder);

            Assert.Equals(maxfinder.Max, 8);
        }

        public static void TransformingVisitor()
        {
            // 3 + ( 5 * 8 )
            Expression three = new Number(3);
            Expression five = new Number(5);
            Expression eight = new Number(8);

            Expression expression = new Addition(three, new Multiplication(five, eight));

            Adder adder = new Adder();
            Multiplier multiplier = new Multiplier();

            while( ! (expression is Number) )
            {
                expression = expression.Visit(adder);
                expression = expression.Visit(multiplier);
            }

            Assert.Equals((expression as Number).Value, 8);
        }


    }

    
}
