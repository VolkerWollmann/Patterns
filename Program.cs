using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Patterns.BehaviourPatterns;
using Patterns.CreationalPatterns;
using Patterns.Other;
using Patterns.Structural;

namespace Patterns
{
	class Program
	{
		static void Main(string[] args)
		{
			#region Creational Patterns
			//Abstract Facotry;
			AbstractFactoryExample.AbstractFactory();

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
