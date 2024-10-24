using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.BehaviourPatterns
{
    public class ChainOfResponsibilityExample
    {
        // #ChainOfResponsibility
        // https://en.wikipedia.org/wiki/Chain-of-responsibility_pattern
        // Each processing object contains logic that defines the types of command objects that it can handle;
        // the rest are passed to the next processing object in the chain.
        // A mechanism also exists for adding new processing objects to the end of this chain.
        // 
        // Example : https://refactoring.guru/design-patterns/chain-of-responsibility/csharp/example#lang-features

        // The Handler interface declares a method for building the chain of
        // handlers. It also declares a method for executing a request.
        public interface IHandler
        {
            IHandler SetNext(IHandler handler);

            object? Handle(object? request);
        }

        // The default chaining behavior can be implemented inside a base handler
        // class.
        abstract class AbstractHandler : IHandler
        {
            private IHandler? _nextHandler;

            public IHandler SetNext(IHandler handler)
            {
                this._nextHandler = handler;

                // Returning a handler from here will let us link handlers in a
                // convenient way like this:
                // monkey.SetNext(squirrel).SetNext(dog);
                return handler;
            }

            public virtual object? Handle(object? request)
            {
                if (this._nextHandler != null)
                {
                    return this._nextHandler.Handle(request);
                }
                else
                {
                    return null;
                }
            }
        }

        class MonkeyHandler : AbstractHandler
        {
            public override object? Handle(object? request)
            {
                if ((request as string) == "Banana")
                {
                    return $"Monkey: I'll eat the {request.ToString()}.\n";
                }
                else
                {
                    return base.Handle(request);
                }
            }
        }

        class SquirrelHandler : AbstractHandler
        {
            public override object? Handle(object? request)
            {
                if (request?.ToString() == "Nut")
                {
                    return $"Squirrel: I'll eat the {request.ToString()}.\n";
                }
                else
                {
                    return base.Handle(request);
                }
            }
        }

        class DogHandler : AbstractHandler
        {
            public override object? Handle(object? request)
            {
                if (request?.ToString() == "MeatBall")
                {
                    return $"Dog: I'll eat the {request.ToString()}.\n";
                }
                else
                {
                    return base.Handle(request);
                }
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        class Client
        {
            // The client code is usually suited to work with a single handler. In
            // most cases, it is not even aware that the handler is part of a chain.
            public static void ClientCode(AbstractHandler handler)
            {
                foreach (var food in new List<string> { "Nut", "Banana", "Cup of coffee" })
                {
                    Console.WriteLine($"Client: Who wants a {food}?");

                    var result = handler.Handle(food);

                    if (result != null)
                    {
                        Console.Write($"   {result}");
                    }
                    else
                    {
                        Console.WriteLine($"   {food} was left untouched.");
                    }
                }
            }
        }

        public static void Example()
        {
            // The other part of the client code constructs the actual chain.
            var monkey = new MonkeyHandler();
            var squirrel = new SquirrelHandler();
            var dog = new DogHandler();

            monkey.SetNext(squirrel).SetNext(dog); //fluent interface

            // The client should be able to send a request to any handler, not
            // just the first one in the chain.
            Console.WriteLine("Chain: Monkey > Squirrel > Dog\n");
            Client.ClientCode(monkey);
            Console.WriteLine();

            Console.WriteLine("Subchain: Squirrel > Dog\n");
            Client.ClientCode(squirrel);
        }
    }
}

