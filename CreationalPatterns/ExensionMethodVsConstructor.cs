using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Patterns.CreationalPatterns
{
    namespace ExtensionMethodVsConstructor
    {
        public class SampleClass
        {
            internal int _p1;
            internal int _p2;

            public SampleClass()
            {
                _p1 = 0;
                _p2 = 0;
            }
            public SampleClass(int p1) : this()
            {
                _p1 = p1;
            }

            public SampleClass(int p1, int p2) : this(p1)
            {
                _p2 = p2;
            }
        }

        public static class SampleClassExetensions
        {
            internal static SampleClass WithP1(this SampleClass sampleClass, int p1)
            {
                sampleClass._p1 = p1;
                return sampleClass;
            }

            internal static SampleClass WithP2(this SampleClass sampleClass, int p2)
            {
                sampleClass._p2 = p2;
                return sampleClass;
            }
        }

        public class ExtensionMethodVsConstructorExample
        {
            public static void Example()
            {
                SampleClass sampleClass1 = new SampleClass(1);
                SampleClass sampleClass2 = new SampleClass(1,2);
                SampleClass sampleClass3 = new SampleClass().WithP1(1);
                SampleClass sampleClass4 = new SampleClass().WithP1(1).WithP2(2);

                Assert.AreEqual(sampleClass2._p2, 2);
                Assert.AreEqual(sampleClass4._p2, 2);
            }
        }
    }
}
