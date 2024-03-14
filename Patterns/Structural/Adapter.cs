using System;

// Source : https://refactoring.guru/design-patterns/adapter/csharp/example#lang-features
//          https://www.dofactory.com/net/adapter-design-pattern
//          The Adapter design pattern converts the interface of a class into another interface clients expect.
//          This design pattern lets classes work together that could not otherwise because of incompatible interfaces.
namespace Patterns.Structural
{
    // The Target defines the domain-specific interface used by the client code.
    public interface ITarget
    {
        string GetRequest();
    }

    // The adapted contains some useful behavior, but its interface is
    // incompatible with the existing client code. The adapted needs some
    // adaptation before the client code can use it.
    class Adapted
    {
        public string GetSpecificRequest()
        {
            return "Specific request.";
        }
    }

    // The Adapter makes the adapted's interface compatible with the Target's
    // interface.
    class Adapter(Adapted adapted) : ITarget
    {
        public string GetRequest()
        {
            return $"This is '{adapted.GetSpecificRequest()}'";
        }
    }

    public class AdapterExample
    {
        public static void Adapter()
        {
            Adapted adapted = new Adapted();
            ITarget target = new Adapter(adapted);

            Console.WriteLine("Adapted interface is incompatible with the client.");
            Console.WriteLine("But with adapter client can call it's method.");

            Console.WriteLine(target.GetRequest());
        }
    }
}
