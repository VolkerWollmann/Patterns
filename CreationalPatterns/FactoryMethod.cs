using System;

namespace Patterns.CreationalPatterns
{
    // #Factory method
    // #gof #book design patterns
    // https://www.dofactory.com/net/factory-method-design-pattern
    // Summary:  Define an interface for creating an object, but let subclasses decide which class to instantiate. 
    // Factory Method lets a class defer instantiation to subclasses.
    // 
    // User of Factory method knows about base class/Interface . Factory method may create a derived class instantiation or class instantiation, which fulfills interface

    /// <summary>
    /// MainApp startup class for Structural 
    /// Factory Method Design Pattern.
    /// </summary>
    public class FactoryMethodExample
    {
        /// <summary>
        /// The 'Product' abstract class
        /// </summary>
        abstract class Product
        {
        }

        /// <summary>
        /// A 'ConcreteProduct' class
        /// </summary>
        class ConcreteProductA : Product
        {
        }

        /// <summary>
        /// A 'ConcreteProduct' class
        /// </summary>
        class ConcreteProductB : Product
        {
        }

        /// <summary>
        /// The 'Creator' abstract class
        /// </summary>
        abstract class Creator
        {
            public abstract Product FactoryMethod();
        }

        /// <summary>
        /// A 'ConcreteCreator' class
        /// </summary>
        class ConcreteCreatorA : Creator
        {
            public override Product FactoryMethod()
            {
                return new ConcreteProductA();
            }
        }

        /// <summary>
        /// A 'ConcreteCreator' class
        /// </summary>
        class ConcreteCreatorB : Creator
        {
            public override Product FactoryMethod()
            {
                return new ConcreteProductB();
            }
        }

        /// <summary>
        /// Entry point into console application.
        /// </summary>
        public static void Test()
        {
            // An array of creators
            Creator[] creators = new Creator[2];

            creators[0] = new ConcreteCreatorA();
            creators[1] = new ConcreteCreatorB();

            // Iterate over creators and create products
            foreach (Creator creator in creators)
            {
                Product product = creator.FactoryMethod();
                Console.WriteLine("Created {0}",
                  product.GetType().Name);
            }
        }
    }
}