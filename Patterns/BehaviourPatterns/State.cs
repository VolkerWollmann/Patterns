﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Patterns.BehaviourPatterns
{
    /// https://www.dofactory.com/net/state-design-pattern
    /// The State design pattern allows an object to alter its behavior when its internal state changes. 
    /// The object will appear to change its class.

    /// <summary>
    /// State Design Pattern
    /// </summary>

    /// <summary>
    /// The 'State' abstract class
    /// </summary>
    public abstract class State
    {
        public abstract void Handle(Context context);
    }
    /// <summary>
    /// A 'ConcreteState' class
    /// </summary>
    public class ConcreteStateA : State
    {
        public override void Handle(Context context)
        {
            context.State = new ConcreteStateB();
        }
    }
    /// <summary>
    /// A 'ConcreteState' class
    /// </summary>
    public class ConcreteStateB : State
    {
        public override void Handle(Context context)
        {
            context.State = new ConcreteStateA();
        }
    }
    /// <summary>
    /// The 'Context' class
    /// </summary>
    public class Context
    {
        private State _state;
        // Constructor
        public Context(State state)
        {
            this.State = state;
            Assert.IsNotNull(this._state);
        }
        // Gets or sets the state
        public State State
        {
            get => _state;
            set
            {
                _state = value;
                Console.WriteLine("State: " + _state.GetType().Name);
            }
        }
        public void Request()
        {
            _state.Handle(this);
        }
    }

    public class StateExample
    {
        public static void Test()
        {
            // Setup context in a state
            var context = new Context(new ConcreteStateA());
            // Issue requests, which toggles state
            context.Request();
            context.Request();
            context.Request();
            context.Request();
            
        }
    }
}