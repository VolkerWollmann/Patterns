using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Patterns.BehaviourPatterns
{
    /// https://www.dofactory.com/net/observer-design-pattern
    ///  Define a one-to-many dependency between objects so that when one object changes state, 
    ///  all its dependents are notified and updated automatically. 

    /// <summary>
    /// The 'Subject' abstract class
    /// </summary>
    abstract class Subject
    {
        private readonly List<Observer> _Observers = new List<Observer>();

        public void Attach(Observer observer)
        {
            _Observers.Add(observer);
        }

        public void Detach(Observer observer)
        {
            _Observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (Observer o in _Observers)
            {
                o.Update();
            }
        }
    }

    /// <summary>
    /// The 'ConcreteSubject' class
    /// </summary>
    class ConcreteSubject : Subject
    {
        // Gets or sets subject state
        public string SubjectState { get; set; } = "";
    }

    /// <summary>
    /// The 'Observer' abstract class
    /// </summary>
    abstract class Observer
    {
        public abstract void Update();
    }

    /// <summary>
    /// The 'ConcreteObserver' class
    /// </summary>
    class ConcreteObserver : Observer
    {
        private readonly string _Name;
        private string ObserverState    = string.Empty;
        private ConcreteSubject _Subject;

        // Constructor
        public ConcreteObserver(
          ConcreteSubject subject, string name)
        {
            this._Subject = subject;
            this._Name = name;
        }

        public override void Update()
        {
            ObserverState = _Subject.SubjectState;
            Console.WriteLine("Observer {0}'s new state is {1}",
              _Name, ObserverState);
        }

        // Gets or sets subject
        public ConcreteSubject Subject
        {
            get => _Subject;
            set => _Subject = value;
        }
    }
    /// <summary>
    /// MainApp startup class for Structural 
    /// Observer Design Pattern.
    /// </summary>
    public class ObserverExample
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>
        public static void Observer()
        {
            // Configure Observer pattern
            ConcreteSubject s = new ConcreteSubject();

            ConcreteObserver concreteObserverX = new ConcreteObserver(s, "X");
            Assert.AreNotEqual(concreteObserverX.Subject, null);
            s.Attach(concreteObserverX);
            s.Attach(new ConcreteObserver(s, "Y"));
            s.Attach(new ConcreteObserver(s, "Z"));

            // Change subject and notify observers
            s.SubjectState = "ABC";
            s.Notify();

            s.Detach(concreteObserverX);
        }
    }
}