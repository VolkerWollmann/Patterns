﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BehaviourPatterns.Visitor;
using BehaviourPatterns.VisitorExample;
using BehaviourPatterns.Strategy;
using BehaviourPatterns.Command;
using StrucutralPatterns.Adapter;
using StrucutralPatterns.Composite;

namespace PatternTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Visitor()
        {
            // Visitor
            Vistor visitor = new Vistor();
            visitor.Main();
        }

        [TestMethod]
        public void Visitor_Simple()
        {
            VisitorExample.SimpleVisitor();
        }

        [TestMethod]
        public void Visitor_Transforming()
        {
            VisitorExample.TransformingVisitor();
        }

        [TestMethod]
        public void Strategy()
        {
            StrategyExample.Strategy();
        }

        [TestMethod]
        public void Command()
        {
            ComandExample.Command();
        }

        [TestMethod]
        public void Adapter()
        {
            AdapterExample.Adapter();
        }

        [TestMethod]
        public void Composite()
        {
            CompositeExample.Composite();
        }
    }
}
