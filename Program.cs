﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Patterns.BehaviourPatterns;
using Patterns.CreationalPatterns;
using Patterns.Other;
using Patterns.Structural;
using Patterns.Structural.Decorator;

namespace Patterns
{
	class Program
	{
		static void Main(string[] args)
		{
			Assert.AreEqual(args, null);

			#region Creational Patterns
			//Abstract Factory;
			AbstractFactoryExample.AbstractFactory();

			#endregion

			//Adapter
			AdapterExample.Adapter();

            #region beavioural Patterns
            // Visitor
            Visitor visitor = new Visitor();
			visitor.Main();

			//Decorator
			DecoratorExample.Decorator();

			//Command
			CommandExample.Command();

			//Observer
			ObserverExample.Observer();

			#endregion

			//BusinessDelegate
			BusinessDelegateExample.BusinessDelegate();

			//DataTransferObject
			DataTransferObjectExample transferObjectExample = new DataTransferObjectExample();
			transferObjectExample.StartDemo();

			//UnitOfWork
			UnitOfWorkExample.UnitOfWork();
		}
	}
}
