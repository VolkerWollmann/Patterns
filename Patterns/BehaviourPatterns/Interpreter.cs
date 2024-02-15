using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.BehaviourPatterns
{
    /// <summary>
    /// Source : https://en.wikipedia.org/wiki/Interpreter_pattern
    /// In computer programming, the interpreter pattern is a design pattern that specifies how to
    /// evaluate sentences in a language.
    /// The basic idea is to have a class for each symbol (terminal or nonterminal)
    /// in a specialized computer language. The syntax tree of a sentence in the language
    /// is an instance of the composite pattern and is used to evaluate (interpret)
    /// the sentence for a client.
    ///
    /// WO: similar to a concrete visitor
    /// </summary>
    public class InterpreterExample
    {
        class Context
        {
            public Stack<string> Result = new Stack<string>();
        }

        interface Expression
        {
            void Interpret(Context context);
        }

        abstract class OperatorExpression : Expression
        {
            public Expression Left { private get; set; }
            public Expression Right { private get; set; }

            public void Interpret(Context context)
            {
                Left.Interpret(context);
                string leftValue = context.Result.Pop();

                Right.Interpret(context);
                string rightValue = context.Result.Pop();

                DoInterpret(context, leftValue, rightValue);
            }

            protected abstract void DoInterpret(Context context, string leftValue, string rightValue);
        }

        class EqualsExpression : OperatorExpression
        {
            protected override void DoInterpret(Context context, string leftValue, string rightValue)
            {
                context.Result.Push(leftValue == rightValue ? "true" : "false");
            }
        }

        class OrExpression : OperatorExpression
        {
            protected override void DoInterpret(Context context, string leftValue, string rightValue)
            {
                context.Result.Push(leftValue == "true" || rightValue == "true" ? "true" : "false");
            }
        }

        class MyExpression : Expression
        {
            public string Value { private get; set; }

            public void Interpret(Context context)
            {
                context.Result.Push(Value);
            }
        }
        public static void Example()
        {
            var context = new Context();
            var input = new MyExpression();

            var expression = new OrExpression
            {
                Left = new EqualsExpression
                {
                    Left = input,
                    Right = new MyExpression { Value = "4" }
                },
                Right = new EqualsExpression
                {
                    Left = input,
                    Right = new MyExpression { Value = "four" }
                }
            };

            input.Value = "four";
            expression.Interpret(context);
            // Output: "true" 
            var result = context.Result.Pop();
            Console.WriteLine(result);

            input.Value = "44";
            expression.Interpret(context);
            // Output: "false"
            var result2 = context.Result.Pop();
            Console.WriteLine(result2);
        }
    }
}
