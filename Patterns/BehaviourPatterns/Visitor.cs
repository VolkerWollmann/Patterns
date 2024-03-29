﻿using System;
using System.Collections.Generic;

// Source : https://refactoring.guru/design-patterns/visitor/csharp/example
//          https://www.dofactory.com/net/visitor-design-pattern
//          The Visitor design pattern represents an operation to be performed on the elements of an object structure.
//          This pattern lets you define a new operation without changing the classes of the elements on which it operates.

namespace Patterns.BehaviourPatterns
{
	// The AcceptVisitor interface declares an `accept` method that should take the
	// base visitor interface as an argument.
	public interface IAcceptVisitor
	{
		void Accept(IVisitor visitor);
	}

	// Each Concrete Component must implement the `IAcceptVisitor.Accept` method in such a way
	// that it calls the visitor's method corresponding to the component's
	// class.
	public class ConcreteComponentA : IAcceptVisitor
	{
		// Note that we're calling `VisitConcreteComponentA`, which matches the
		// current class name. This way we let the visitor know the class of the
		// component it works with.
		public void Accept(IVisitor visitor)
		{
			visitor.VisitConcreteComponentA(this);
		}

		// Concrete Components may have special methods that don't exist in
		// their base class or interface. The Visitor is still able to use these
		// methods since it's aware of the component's concrete class.
		public string ExclusiveMethodOfConcreteComponentA()
		{
			return "A";
		}
	}

	public class ConcreteComponentB : IAcceptVisitor
	{
		// Same here: VisitConcreteComponentB => ConcreteComponentB
		public void Accept(IVisitor visitor)
		{
			visitor.VisitConcreteComponentB(this);
		}

		public string SpecialMethodOfConcreteComponentB()
		{
			return "B";
		}
	}

	// The Visitor Interface declares a set of visiting methods that correspond
	// to component classes. The signature of a visiting method allows the
	// visitor to identify the exact class of the component that it's dealing
	// with.
	public interface IVisitor
	{
		void VisitConcreteComponentA(ConcreteComponentA element);

		void VisitConcreteComponentB(ConcreteComponentB element);
	}

	// Concrete Visitors implement several versions of the same algorithm, which
	// can work with all concrete component classes.
	//
	// You can experience the biggest benefit of the Visitor pattern when using
	// it with a complex object structure, such as a Composite tree. In this
	// case, it might be helpful to store some intermediate state of the
	// algorithm while executing visitor's methods over various objects of the
	// structure.
	class ConcreteVisitor1 : IVisitor
	{
		public void VisitConcreteComponentA(ConcreteComponentA element)
		{
			Console.WriteLine(element.ExclusiveMethodOfConcreteComponentA() + " + ConcreteVisitor1");
		}

		public void VisitConcreteComponentB(ConcreteComponentB element)
		{
			Console.WriteLine(element.SpecialMethodOfConcreteComponentB() + " + ConcreteVisitor1");
		}
	}

	class ConcreteVisitor2 : IVisitor
	{
		public void VisitConcreteComponentA(ConcreteComponentA element)
		{
			Console.WriteLine(element.ExclusiveMethodOfConcreteComponentA() + " + ConcreteVisitor2");
		}

		public void VisitConcreteComponentB(ConcreteComponentB element)
		{
			Console.WriteLine(element.SpecialMethodOfConcreteComponentB() + " + ConcreteVisitor2");
		}
	}

	public class Client
	{
		// The client code can run visitor operations over any set of elements
		// without figuring out their concrete classes. The accept operation
		// directs a call to the appropriate operation in the visitor object.
		public static void ClientCode(List<IAcceptVisitor> components, IVisitor visitor)
		{
			foreach (var component in components)
			{
				component.Accept(visitor);
			}
		}
	}

	public class Visitor
	{
		public void Main()
		{
			List<IAcceptVisitor> components = new List<IAcceptVisitor>
				{
					 new ConcreteComponentA(),
					 new ConcreteComponentB()
				};

			Console.WriteLine("The client code works with all visitors via the base Visitor interface:");
			var visitor1 = new ConcreteVisitor1();
			Client.ClientCode(components, visitor1);

			Console.WriteLine();

			Console.WriteLine("It allows the same client code to work with different types of visitors:");
			var visitor2 = new ConcreteVisitor2();
			Client.ClientCode(components, visitor2);
			Console.WriteLine("");
		}
	}
}