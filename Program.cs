using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreationalPatterns.AbstractFactory;
using BehaviourPatterns.Visitor;
using BehaviourPatterns.Decorator;
using BehaviourPatterns.Command;
using BehaviourPatterns.Observer;
using StrucutralPatterns.Adapter;
using Other.BusinessDelegate;
using Other.DataTransferObject;
using Ohter.UnitOfWork;
using CreationalPatterns.FactoryMethod;

namespace Patterns
{
	class Program
	{
		static void Main(string[] args)
		{
			#region Creational Patterns
			//Abstract Facotry;
			AbstractFactoryExample.AbstractFactory();

			//Factory Method;
			FactoryMethodExample.FactoryMethod();
			
			#endregion

			//Adapter
			AdapterExample.Adapter();

            #region beavioural Patterns
            // Visitor
            Vistor visitor = new Vistor();
			visitor.Main();

			//Decorator
			DecoratorExample.Decorator();

			//Command
			CommandExample.Command();

			//Observer
			ObserverExample observerExample = new ObserverExample();
			observerExample.Main();

			#endregion

			//BusinessDelegate
			BusinessDelegateExample businessDelegateExample = new BusinessDelegateExample();
			businessDelegateExample.Main();

			//DataTransferObject
			DataTransferObjectExample transferObjectExample = new DataTransferObjectExample();
			transferObjectExample.StartDemo();

			//UnitOfWork
			UnitOFWorkExample unitOFWorkExample = new UnitOFWorkExample();
			unitOFWorkExample.Demo();
		}
	}
}
