using System;
using System.Collections.Generic;

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
        /// The 'vehicle' abstract class
        /// </summary>
        abstract class Vehicle
        {
            public abstract void Ride();
        }

        /// <summary>
        /// A 'Concrete vehicle Bike' class
        /// </summary>
        class Bike : Vehicle
        {
            public override void Ride()
            {
                Console.WriteLine("Ride the bicyle in free time");
            }
        }

        /// <summary>
        /// A 'ConcreteProduct' class
        /// </summary>
        class Car : Vehicle
        {
            public override void Ride()
            {
                Console.WriteLine("The right vehicle for work days.");
            }
        }

        /// <summary>
        /// The 'Creator' abstract class
        /// </summary>
        abstract class Creator
        {
            public abstract Vehicle FactoryMethod(string weekday);
        }


        class VehicleSelector : Creator
        {
            public override Vehicle FactoryMethod(string weekday)
            {
                if ((new List<string> { "Saturday", "Sunday" }).Contains(weekday))
                    return new Bike();

                return new Car();
            }
        }

        /// <summary>
        /// Entry point into console application.
        /// </summary>
        public static void Test()
        {
            // The vehicle does the best choice 
            VehicleSelector vehicleSelector = new VehicleSelector();
            vehicleSelector.FactoryMethod("Monday").Ride();
            vehicleSelector.FactoryMethod("Sunday").Ride();

        }
    }
}