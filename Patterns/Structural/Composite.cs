﻿using System;
using System.Collections.Generic;

namespace Patterns.Structural
{
    /// <summary>
    /// MainApp startup class for Structural 
    /// Composite Design Pattern.
    /// https://www.dofactory.com/net/composite-design-pattern
    /// https://en.wikipedia.org/wiki/Composite_pattern
    /// </summary>
    /// The Composite design pattern composes objects into tree structures to represent part-whole hierarchies. 
    /// This pattern lets clients treat individual objects and compositions of objects uniformly.
    /// <summary>

    /// The 'Component' abstract class
    /// </summary>
    abstract class Component
    {
        protected string Name;

        // Constructor
        protected Component(string name)
        {
            this.Name = name;
        }

        public abstract void Add(Component c);
        public abstract void Remove(Component c);
        public abstract void Display(int depth);
    }

    /// <summary>
    /// The 'Composite' class
    /// </summary>
    class Composite : Component
    {
        private readonly List<Component> Children = new List<Component>();

        // Constructor
        public Composite(string name)
          : base(name)
        {

        }

        public override void Add(Component component)
        {
            Children.Add(component);
        }

        public override void Remove(Component component)
        {
            Children.Remove(component);
        }

        public override void Display(int depth)
        {
            Console.WriteLine(new String('-', depth) + Name);

            // Recursively display child nodes
            foreach (Component component in Children)
            {
                component.Display(depth + 2);
            }
        }
    }

    /// <summary>
    /// The 'Leaf' class
    /// </summary>
    class Leaf : Component
    {
        // Constructor
        public Leaf(string name)
          : base(name)
        {
        }

        public override void Add(Component c)
        {
            Console.WriteLine("Cannot add to a leaf");
        }

        public override void Remove(Component c)
        {
            Console.WriteLine("Cannot remove from a leaf");
        }

        public override void Display(int depth)
        {
            Console.WriteLine(new String('-', depth) + Name);
        }
    }
    public class CompositeExample
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>

        public static void Composite()
        {
            // Create a tree structure
            Composite root = new Composite("root");
            root.Add(new Leaf("Leaf A"));
            root.Add(new Leaf("Leaf B"));

            Composite comp = new Composite("Composite X");
            comp.Add(new Leaf("Leaf XA"));
            comp.Add(new Leaf("Leaf XB"));

            root.Add(comp);

            root.Add(new Leaf("Leaf C"));

            // Add and remove a leaf
            Leaf leaf = new Leaf("Leaf D");
            root.Add(leaf);
            root.Remove(leaf);

            // Recursively display tree
            root.Display(1);
        }
    }
}