﻿using System;

// Source : https://refactoring.guru/design-patterns/adapter/csharp/example#lang-features
//          https://www.dofactory.com/net/adapter-design-pattern
//          The Adapter design pattern converts the interface of a class into another interface clients expect.
//          This design pattern lets classes work together that couldn‘t otherwise because of incompatible interfaces.
namespace Patterns.Structural
{
    // The Target defines the domain-specific interface used by the client code.
    public interface ITarget
    {
        string GetRequest();
    }

    // The adaptee contains some useful behavior, but its interface is
    // incompatible with the existing client code. The adaptee needs some
    // adaptation before the client code can use it.
    class Adaptee
    {
        public string GetSpecificRequest()
        {
            return "Specific request.";
        }
    }

    // The Adapter makes the adaptee's interface compatible with the Target's
    // interface.
    class Adapter : ITarget
    {
        private readonly Adaptee Adaptee;

        public Adapter(Adaptee adaptee)
        {
            Adaptee = adaptee;
        }

        public string GetRequest()
        {
            return $"This is '{Adaptee.GetSpecificRequest()}'";
        }
    }

    public class AdapterExample
    {
        public static void Adapter()
        {
            Adaptee adaptee = new Adaptee();
            ITarget target = new Adapter(adaptee);

            Console.WriteLine("Adaptee interface is incompatible with the client.");
            Console.WriteLine("But with adapter client can call it's method.");

            Console.WriteLine(target.GetRequest());
        }
    }
}
