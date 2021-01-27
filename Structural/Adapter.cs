using System;

// Source : https://refactoring.guru/design-patterns/adapter/csharp/example#lang-features
namespace StrucutralPatterns.Adapter
{
	// The Target defines the domain-specific interface used by the client code.
	public interface ITarget
	{
		string GetRequest();
	}

	// The Adaptee contains some useful behavior, but its interface is
	// incompatible with the existing client code. The Adaptee needs some
	// adaptation before the client code can use it.
	class Adaptee
	{
		public string GetSpecificRequest()
		{
			return "Specific request.";
		}
	}

	// The Adapter makes the Adaptee's interface compatible with the Target's
	// interface.
	class Adapter : ITarget
	{
		private readonly Adaptee _adaptee;

		public Adapter(Adaptee adaptee)
		{
			this._adaptee = adaptee;
		}

		public string GetRequest()
		{
			return $"This is '{this._adaptee.GetSpecificRequest()}'";
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
