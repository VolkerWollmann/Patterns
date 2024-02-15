using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Patterns.CreationalPatterns
{
    // Source : https://refactoring.guru/design-patterns/prototype
    //The prototype can create a reasonable copy of it's self
    public class PrototypeExample
    {
        internal class IdInfo
        {
            public int IdNumber;

            public IdInfo(int IdNumber)
            {
                this.IdNumber = IdNumber;
            }
        }

        internal class Person
        {
            public int Age;
            public string Name;
            internal IdInfo IdInfo;

            public Person ShallowCopy()
            {
                return (Person)this.MemberwiseClone();
            }

            public Person DeepCopy()
            {
                Person other = (Person)this.MemberwiseClone();
                other.IdInfo = new IdInfo(IdInfo.IdNumber);
#pragma warning disable CS0618 // Type or member is obsolete
                other.Name = string.Copy(Name);
#pragma warning restore CS0618 // Type or member is obsolete
                return other;
            }
        }

        public static void Test()
        {
            // Create an instance of Person and assign values to its fields.
            Person p1 = new Person();
            p1.Age = 42;
            p1.Name = "Sam";
            p1.IdInfo = new IdInfo(6565);

            // Perform a shallow copy of p1 and assign it to p2.
            Person p2 = p1.ShallowCopy();

            // Display values of p1, p2
            Console.WriteLine("Original values of p1 and p2:");
            Console.WriteLine("   p1 instance values: ");
            DisplayValues(p1);
            Console.WriteLine("   p2 instance values:");
            DisplayValues(p2);

            // Change the value of p1 properties and display the values of p1 and p2.
            p1.Age = 32;
            p1.Name = "Frank";
            p1.IdInfo.IdNumber = 7878;
            Console.WriteLine("\nValues of p1 and p2 after changes to p1:");
            Console.WriteLine("   p1 instance values: ");
            DisplayValues(p1);
            Console.WriteLine("   p2 instance values:");
            DisplayValues(p2);

            // Make a deep copy of p1 and assign it to p3.
            Person p3 = p1.DeepCopy();

            Assert.AreEqual(p3.Name, p1.Name);
            Assert.AreEqual(p1.Age, p3.Age);
            Assert.AreEqual(p1.IdInfo.IdNumber, p3.IdInfo.IdNumber);

            // Change the members of the p1 class to new values to show the deep copy.
            p1.Name = "George";
            p1.Age = 39;
            p1.IdInfo.IdNumber = 8641;
            
            Console.WriteLine("\nValues of p1 and p3 after changes to p1:");
            Console.WriteLine("   p1 instance values: ");
            DisplayValues(p1);
            Console.WriteLine("   p3 instance values:");
            DisplayValues(p3);

            Assert.AreNotEqual(p3.Name, p1.Name);
            Assert.AreNotEqual(p1.Age, p3.Age);
            Assert.AreNotEqual(p1.IdInfo.IdNumber, p3.IdInfo.IdNumber);
            
        }

        internal static void DisplayValues(Person p)
        {
            Console.WriteLine("      Name: {0:s}, Age: {1:d}", p.Name, p.Age);
            Console.WriteLine("      IdInfo.IdNumber: {0:d}", p.IdInfo.IdNumber);
        }
    }
}
