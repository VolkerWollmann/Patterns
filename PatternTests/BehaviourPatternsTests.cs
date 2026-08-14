
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
        public void NullObject()
        {
            NullObjectExample.NullObject();
        }

        [Fact]
        public void TheServiceWorksWithoutBeingGivenALog()
        {
            NullObjectExample.OrderService service = new NullObjectExample.OrderService();

            // No log, no null check, no NullReferenceException.
            Assert.Equal(4.5m, service.Place("Bolt", 3, 1.5m));
        }

        [Fact]
        public void ALogChangesNothingAboutTheResult()
        {
            RecordingLog log = new RecordingLog();

            decimal withLog = new NullObjectExample.OrderService(log).Place("Bolt", 3, 1.5m);
            decimal withoutLog = new NullObjectExample.OrderService().Place("Bolt", 3, 1.5m);

            // The null object is neutral: it swallows the messages without altering
            // what the service does.
            Assert.Equal(withLog, withoutLog);
        }

        [Fact]
        public void TheServiceStillLogsWhenThereIsSomethingToLogTo()
        {
            RecordingLog log = new RecordingLog();

            new NullObjectExample.OrderService(log).Place("Bolt", 3, 1.5m);

            Assert.Equal(2, log.Messages.Count);
            Assert.Contains("3 x Bolt", log.Messages[0]);
        }

        [Fact]
        public void TheNullLogIsSharedAndSwallowsEverything()
        {
            Assert.Same(NullObjectExample.NullLog.Instance, NullObjectExample.NullLog.Instance);

            // Stateless, so it can be handed out to anybody without a care.
            NullObjectExample.NullLog.Instance.Write("goes nowhere");
        }

        // The same seam the null object plugs into also serves a test.
        private sealed class RecordingLog : NullObjectExample.ILog
        {
            public List<string> Messages { get; } = new List<string>();

            public void Write(string message)
            {
                Messages.Add(message);
            }
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
