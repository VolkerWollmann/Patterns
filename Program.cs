using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreationalPatterns.AbstractFactory;
using CreationalPatterns.FactoryMethod;
using BehaviourPatterns.Visitor;
using BehaviourPatterns.Decorator;
using BehaviourPatterns.Command;
using StrucutralPatterns.Adapter;
using Other.BusinessDelegate;
using Other.DataTransferObject;

namespace Patterns
{
	class Program
	{
		static void Main(string[] args)
		{
			//Abstract Facotry;
			AbstactFactoryClient abstractFactoryClient = new AbstactFactoryClient();
			abstractFactoryClient.Main();

			//Factory Method;
			FactoryMethod factoryMethod = new FactoryMethod();
			factoryMethod.Main();

			//Adapter
			AdapterExample adapterExample = new AdapterExample();
			adapterExample.Main();

			// Visitor
			Vistor visitor = new Vistor();
			visitor.Main();

			//Decorator
			DecoratorExample decoratorExample = new DecoratorExample();
			decoratorExample.Main();

			//BusinessDelegate
			BusinessDelegateExample businessDelegateExample = new BusinessDelegateExample();
			businessDelegateExample.Main();

			//DataTransferObject
			DataTransferObjectExample transferObjectExample = new DataTransferObjectExample();
			transferObjectExample.StartDemo();

			//Command
			ComandExample comandExample = new ComandExample();
			comandExample.Main();
		}
	}
}
