using System;

namespace Patterns.Structural
{
    // https://www.dofactory.com/net/facade-design-pattern
    // Provide a unified interface to a set of interfaces in a subsystem. 
    // Facade defines a higher-level interface that makes the subsystem easier to use. 
    
    /// <summary>
    /// The 'Subsystem ClassA' class
    /// </summary>
    class SubSystemOne
    {
        public void MethodOne()
        {
            Console.WriteLine(" SubSystemOne Method");
        }
    }

    /// <summary>
    /// The 'Subsystem ClassB' class
    /// </summary>
    class SubSystemTwo
    {
        public void MethodTwo()
        {
            Console.WriteLine(" SubSystemTwo Method");
        }
    }

    /// <summary>
    /// The 'Subsystem ClassC' class
    /// </summary>
    class SubSystemThree
    {
        public void MethodThree()
        {
            Console.WriteLine(" SubSystemThree Method");
        }
    }

    /// <summary>
    /// The 'Subsystem ClassD' class
    /// </summary>
    class SubSystemFour
    {
        public void MethodFour()
        {
            Console.WriteLine(" SubSystemFour Method");
        }
    }

    /// <summary>
    /// The 'Facade' class
    /// </summary>
    class Facade
    {
        private readonly SubSystemOne One;
        private readonly SubSystemTwo Two;
        private readonly SubSystemThree Three;
        private readonly SubSystemFour Four;

        public Facade()
        {
            One = new SubSystemOne();
            Two = new SubSystemTwo();
            Three = new SubSystemThree();
            Four = new SubSystemFour();
        }

        public void MethodA()
        {
            Console.WriteLine("\nMethodA() ---- ");
            One.MethodOne();
            Two.MethodTwo();
            Four.MethodFour();
        }

        public void MethodB()
        {
            Console.WriteLine("\nMethodB() ---- ");
            Two.MethodTwo();
            Three.MethodThree();
        }
    }

    /// <summary>
    /// MainApp startup class for Structural
    /// Facade Design Pattern.
    /// </summary>
    public class FacadeExample
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>
        public static void Facade()
        {
            Facade facade = new Facade();

            facade.MethodA();
            facade.MethodB();
        }
    }
}

