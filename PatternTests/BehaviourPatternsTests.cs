
using System;
using System.Collections.Generic;
using Patterns.BehaviourPatterns;
using Xunit;

namespace PatternTests
{
    public class BehaviourPatternsTests
    {
        [Fact]
        public void Visitor()
        {
            // Visitor
            Visitor visitor = new Visitor();
            visitor.Main();
        }

        [Fact]
        public void Strategy()
        {
            StrategyExample.Strategy();
        }

        [Fact]
        public void Command()
        {
            CommandExample.Command();
        }

        [Fact]
        public void Observer()
        {
            ObserverExample.Observer();
        }

        [Fact]
        public void State()
        {
            StateExample.Test();
        }

        [Fact]
        public void ChainOfResponsibility()
        {
            ChainOfResponsibilityExample.Example();
        }

        [Fact]
        public void Iterator()
        {
            IteratorExample.Iterator();
        }

        [Fact]
        public void Interpreter()
        {
            InterpreterExample.Example();
        }

        [Fact]
        public void Mediator()
        {
            MediatorExample.Example();
        }

        [Fact]
        public void Memento()
        {
            MementoExample.Example();
        }

        [Fact]
        public void TemplateMethod()
        {
            TemplateMethodExample.Example();
        }

        [Fact]
        public void IteratorVisitsEveryItemInOrder()
        {
            IReadOnlyList<string> visited = IteratorExample.Iterator();

            Assert.Equal(new[] { "Item A", "Item B", "Item C", "Item D" }, visited);
        }

        [Fact]
        public void NotifyingTheSubjectUpdatesEveryObserver()
        {
            ConcreteSubject subject = new ConcreteSubject();

            ConcreteObserver x = new ConcreteObserver(subject, "X");
            ConcreteObserver y = new ConcreteObserver(subject, "Y");
            subject.Attach(x);
            subject.Attach(y);

            subject.SubjectState = "ABC";
            subject.Notify();

            Assert.Equal("ABC", x.ObserverState);
            Assert.Equal("ABC", y.ObserverState);
        }

        [Fact]
        public void ADetachedObserverStopsBeingUpdated()
        {
            ConcreteSubject subject = new ConcreteSubject();

            ConcreteObserver observer = new ConcreteObserver(subject, "X");
            subject.Attach(observer);

            subject.SubjectState = "first";
            subject.Notify();

            subject.Detach(observer);
            subject.SubjectState = "second";
            subject.Notify();

            // The observer never heard about the second change.
            Assert.Equal("first", observer.ObserverState);
        }

        [Fact]
        public void AContextAlwaysHasAState()
        {
            Assert.Throws<ArgumentNullException>(() => new Context(null!));
        }

        [Fact]
        public void HandlingSwitchesTheContextToTheNextState()
        {
            Context context = new Context(new ConcreteStateA());

            context.Request();
            Assert.IsType<ConcreteStateB>(context.State);

            context.Request();
            Assert.IsType<ConcreteStateA>(context.State);
        }
    }
}
