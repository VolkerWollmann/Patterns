using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Patterns.BehaviourPatterns
{
    // https://www.dofactory.com/net/iterator-design-pattern
    //  Provide a way to access the elements of an aggregate object sequentially 
    //  without exposing its underlying representation. 
    /// <summary>
    
    /// The 'Aggregate' abstract class
    /// </summary>
    abstract class Aggregate
    {
        public abstract Iterator CreateIterator();
    }

    /// <summary>
    /// The 'ConcreteAggregate' class
    /// </summary>
    class ConcreteAggregate : Aggregate
    {
        private readonly ArrayList Items = new ArrayList();

        public override Iterator CreateIterator()
        {
            return new ConcreteIterator(this);
        }

        // Gets item count
        public int Count => Items.Count;

        // Indexer
        public object? this[int index]
        {
            get => Items[index];
            set => Items.Insert(index, value);
        }
    }

    /// <summary>
    /// The 'Iterator' abstract class
    /// </summary>
    abstract class Iterator
    {
        public abstract object? First();
        public abstract object? Next();
        public abstract bool IsDone();
        public abstract object? CurrentItem();
    }

    /// <summary>
    /// The 'ConcreteIterator' class
    /// </summary>
    class ConcreteIterator : Iterator
    {
        private readonly ConcreteAggregate Aggregate;
        private int Current;

        // Constructor
        public ConcreteIterator(ConcreteAggregate aggregate)
        {
            this.Aggregate = aggregate;
        }

        // Gets first iteration item
        public override object First()
        {
            return Aggregate[0]!;
        }

        // Gets next iteration item
        public override object? Next()
        {
            object? ret = null;
            if (Current < Aggregate.Count - 1)
            {
                ret = Aggregate[++Current];
            }

            return ret;
        }

        // Gets current iteration item
        public override object? CurrentItem()
        {
            return Aggregate[Current];
        }

        // Gets whether iterations are complete
        public override bool IsDone()
        {
            return (Current+1) >= Aggregate.Count;
        }
    }
    /// <summary>
    /// MainApp startup class for Structural 
    /// Iterator Design Pattern.
    /// </summary>
    public class IteratorExample
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>
        public static void Iterator()
        {
            ConcreteAggregate aggregate = new ConcreteAggregate
            {
                [0] = "Item A",
                [1] = "Item B",
                [2] = "Item C",
                [3] = "Item D"
            };

            // Create Iterator and provide aggregate
            Iterator iterator = aggregate.CreateIterator();

            Console.WriteLine("Iterating over collection:");

            Console.WriteLine(iterator.First());
            while (!iterator.IsDone())
            {
                Console.WriteLine(iterator.CurrentItem());
                iterator.Next();
            }

            Assert.AreEqual(true, iterator.IsDone() );
        }
    }
}