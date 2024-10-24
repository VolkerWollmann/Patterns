using System;
using System.Collections.Generic;

// Source : https://refactoring.guru/design-patterns/strategy
//          https://www.dofactory.com/net/strategy-design-pattern
// The Strategy design pattern defines a family of algorithms, encapsulate each one, and make them interchangeable.
// This pattern lets the algorithm vary independently from clients that use it.


namespace Patterns.BehaviourPatterns
{
    // The Context defines the interface of interest to clients.
    
    public class StrategyContext
    {
        // The Context maintains a reference to one of the Strategy objects. The
        // Context does not know the concrete class of a strategy. It should
        // work with all strategies via the Strategy interface.
        private IStrategy? Strategy;

        // Usually, the Context accepts a strategy through the constructor, but
        // also provides a setter to change it at runtime.
        //public StrategyContext(IStrategy strategy)
        //{
        //    this.Strategy = strategy;
        //}

        // Usually, the Context allows replacing a Strategy object at runtime.
        public void SetStrategy(IStrategy strategy)
        {
            this.Strategy = strategy;
        }

        // The Context delegates some work to the Strategy object instead of
        // implementing multiple versions of the algorithm on its own.
        public void DoSomeBusinessLogic()
        {
            Console.WriteLine("Context: Sorting data using the strategy (not sure how it'll do it)");
            var result = this.Strategy?.DoAlgorithm(new List<string> { "a", "b", "c", "d", "e" });

            string resultStr = string.Empty;

            if ((result as List<string>) != null)
            {
                foreach (var element in (List<string>)result)
                {
                    resultStr += element + ",";
                }
            }

            Console.WriteLine(resultStr);
        }
    }

    // The Strategy interface declares operations common to all supported
    // versions of some algorithm.
    //
    // The Context uses this interface to call the algorithm defined by Concrete
    // Strategies.
    public interface IStrategy
    {
        object? DoAlgorithm(object data);
    }

    // Concrete Strategies implement the algorithm while following the base
    // Strategy interface. The interface makes them interchangeable in the
    // Context.
    internal class ConcreteStrategyA : IStrategy
    {
        public object? DoAlgorithm(object data)
        {
            var list = data as List<string>;
            list?.Sort();

            return list;
        }
    }

    internal class ConcreteStrategyB : IStrategy
    {
        public object? DoAlgorithm(object data)
        {
            var list = data as List<string>;
            if (list != null)
            {
                list.Sort();
                list.Reverse();
            }

            return list;
        }
    }

    public class StrategyExample
    {
        public static void Strategy()
        {
            // The client code picks a concrete strategy and passes it to the
            // context. The client should be aware of the differences between
            // strategies in order to make the right choice.
            var context = new StrategyContext();

            Console.WriteLine("Client: Strategy is set to normal sorting.");
            context.SetStrategy(new ConcreteStrategyA());
            context.DoSomeBusinessLogic();

            Console.WriteLine();

            Console.WriteLine("Client: Strategy is set to reverse sorting.");
            context.SetStrategy(new ConcreteStrategyB());
            context.DoSomeBusinessLogic();
        }
    }
}