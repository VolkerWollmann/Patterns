﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.BehaviourPatterns
{
    public class MediatorExample
    {
        // https://refactoring.guru/design-patterns/mediator
        // Idea:Mediator is a behavioral design pattern that lets you reduce
        // chaotic dependencies between objects.
        // The pattern restricts direct communications between the objects
        // and forces them to collaborate only via a mediator object.

        // The Mediator interface declares a method used by components to notify the
        // mediator about various events. The Mediator may react to these events and
        // pass the execution to other components.
        public interface IMediator
        {
            void Notify(object sender, string ev);
        }

        // Concrete Mediators implement cooperative behavior by coordinating several
        // components.
        class ConcreteMediator : IMediator
        {
            private Component1 _component1;

            private Component2 _component2;

            public ConcreteMediator(Component1 component1, Component2 component2)
            {
                this._component1 = component1;
                this._component1.SetMediator(this);
                this._component2 = component2;
                this._component2.SetMediator(this);
            }

            public void Notify(object sender, string ev)
            {
                if (ev == "A")
                {
                    Console.WriteLine("Mediator reacts on A and triggers following operations:");
                    this._component2.DoC();
                }
                if (ev == "D")
                {
                    Console.WriteLine("Mediator reacts on D and triggers following operations:");
                    this._component1.DoB();
                    this._component2.DoC();
                }
            }
        }

        // The Base Component provides the basic functionality of storing a
        // mediator's instance inside component objects.
        class BaseComponent
        {
            protected IMediator? Mediator;

            public BaseComponent(IMediator? mediator = null)
            {
                this.Mediator = mediator;
            }

            public void SetMediator(IMediator mediator)
            {
                this.Mediator = mediator;
            }
        }

        // Concrete Components implement various functionality. They don't depend on
        // other components. They also don't depend on any concrete mediator
        // classes.
        class Component1 : BaseComponent
        {
            public void DoA()
            {
                Console.WriteLine("Component 1 does A.");

                this.Mediator?.Notify(this, "A");
            }

            public void DoB()
            {
                Console.WriteLine("Component 1 does B.");

                this.Mediator?.Notify(this, "B");
            }
        }

        class Component2 : BaseComponent
        {
            public void DoC()
            {
                Console.WriteLine("Component 2 does C.");

                this.Mediator?.Notify(this, "C");
            }

            public void DoD()
            {
                Console.WriteLine("Component 2 does D.");

                this.Mediator?.Notify(this, "D");
            }
        }


        public static void Example()
        {
            // The client code.
            Component1 component1 = new Component1();
            Component2 component2 = new Component2();
            new ConcreteMediator(component1, component2);

            Console.WriteLine("Client triggers operation A.");
            component1.DoA();

            Console.WriteLine();

            Console.WriteLine("Client triggers operation D.");
            component2.DoD();
        }
    }
}

