using System;

namespace Patterns.BehaviourPatterns
{
    /// https://www.dofactory.com/net/decorator-design-pattern
    /// The Decorator design pattern attaches additional responsibilities to an object dynamically. 
    /// This pattern provide a flexible alternative to subclassing for extending functionality.

    /// <summary>
    /// The 'Component' abstract class
    /// </summary>
    abstract class Component
    {
        public abstract void Operation();
    }

    /// <summary>
    /// The 'ConcreteComponent' class
    /// </summary>
    class ConcreteComponent : Component
    {
        public override void Operation()
        {
            Console.WriteLine("ConcreteComponent.Operation()");
        }
    }

    /// <summary>
    /// The 'Decorator' abstract class
    /// </summary>
    abstract class Decorator : Component
    {
        protected Component Component;

        public void SetComponent(Component component)
        {
            this.Component = component;
        }

        public override void Operation()
        {
            if (Component != null)
            {
                Component.Operation();
            }
        }
    }

    /// <summary>
    /// The 'ConcreteDecoratorA' class
    /// </summary>
    class ConcreteDecoratorA : Decorator
    {
        public override void Operation()
        {
            base.Operation();
            Console.WriteLine("ConcreteDecoratorA.Operation()");
        }
    }

    /// <summary>
    /// The 'ConcreteDecoratorB' class
    /// </summary>
    class ConcreteDecoratorB : Decorator
    {
        public override void Operation()
        {
            base.Operation();
            AddedBehavior();
            Console.WriteLine("ConcreteDecoratorB.Operation()");
        }

        void AddedBehavior()
        {
        }
    }

    /// <summary>
    /// MainApp startup class for Structural 
    /// Decorator Design Pattern.
    /// </summary>
    public class DecoratorExample
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>
        public static void Decorator()
        {
            // Create ConcreteComponent and two Decorators
            ConcreteComponent c = new ConcreteComponent();
            ConcreteDecoratorA d1 = new ConcreteDecoratorA();
            ConcreteDecoratorB d2 = new ConcreteDecoratorB();

            // Link decorators
            d1.SetComponent(c);
            d2.SetComponent(d1);

            d2.Operation();

        }
    }

}