using System.Collections.Generic;
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

        internal abstract void DoIt(Expression expression);

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

        internal override void DoIt(Expression expression)
        {
            if (expression.Left != null)
                expression.Left.Accept(this);
            if (expression.Right != null)
                expression.Right.Accept(this);
        }
    }
    #endregion

    #region TransformingVisitor

    internal abstract class TransformingVisitor
    {
        internal abstract Expression DoIt(Number expression);

        internal abstract Expression DoIt(Addition expression);

        internal abstract Expression DoIt(Multiplication expression);

        internal abstract Expression DoIt(Expression expression);
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

            return expression;
        }

        internal override Expression DoIt(Multiplication expression)
        {
            return this.DoIt((Expression)expression);
        }

        internal override Expression DoIt(Expression expression)
        {
            if (expression.Left != null)
                expression.Left = expression.Left.Accept(this);
            if (expression.Right != null)
                expression.Right.Accept(this);

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
            return this.DoIt((Expression)expression);
        }

        internal override Expression DoIt(Multiplication expression)
        {
            if ((expression.Left is Number left) && (expression.Right is Number right))
                return new Number(left.Value * right.Value);

            return expression;
        }

        internal override Expression DoIt(Expression expression)
        {
            if (expression.Left != null)
                expression.Left = expression.Left.Accept(this);
            if (expression.Right != null)
                expression.Right = expression.Right.Accept(this);

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
            // (1 * 3) + ( 5 * 8 )
            Expression one = new Number(1);
            Expression three = new Number(3);
            Expression five = new Number(5);
            Expression eight = new Number(8);

            Expression expression = new Addition(new Multiplication(one, three), new Multiplication(five, eight));

            List<TransformingVisitor> visitors = new List<TransformingVisitor>() { new Adder(), new Multiplier() };
         
            while( ! (expression is Number) )
            {
                visitors.ForEach(visitor => { expression = expression.Accept(visitor); });
            }

            Assert.AreEqual(((Number)expression).Value, 43);
        }


    }

    
}
