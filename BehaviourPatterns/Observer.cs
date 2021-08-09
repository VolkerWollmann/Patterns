using System;
using System.Collections.Generic;

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
        private List<Observer> _observers = new List<Observer>();

        public void Attach(Observer observer)
        {
            _observers.Add(observer);
        }

        public void Detach(Observer observer)
        {
            _observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (Observer o in _observers)
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
        private string _subjectState;

        // Gets or sets subject state
        public string SubjectState
        {
            get => _subjectState;
            set => _subjectState = value;
        }
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
        private string _name;
        private string ObserverState;
        private ConcreteSubject _subject;

        // Constructor
        public ConcreteObserver(
          ConcreteSubject subject, string name)
        {
            this._subject = subject;
            this._name = name;
        }

        public override void Update()
        {
            ObserverState = _subject.SubjectState;
            Console.WriteLine("Observer {0}'s new state is {1}",
              _name, ObserverState);
        }

        // Gets or sets subject
        public ConcreteSubject Subject
        {
            get => _subject;
            set => _subject = value;
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

            s.Attach(new ConcreteObserver(s, "X"));
            s.Attach(new ConcreteObserver(s, "Y"));
            s.Attach(new ConcreteObserver(s, "Z"));

            // Change subject and notify observers
            s.SubjectState = "ABC";
            s.Notify();
        }
    }
}