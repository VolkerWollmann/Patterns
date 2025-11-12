using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Patterns.Other
{
    // #fluent interface #interface
    // Source: https://www.codeproject.com/Articles/5326456/Fluent-Interface-Pattern-in-Csharp-With-Inheritanc
    //         https://en.wikipedia.org/wiki/Fluent_interface : method chaining
    public class FluentInterfaceTest
    {
        class Employee
        {
            public string FirstName = "";
            public string LastName = "";
            private int Age = 0;

            public Employee SetFirstName(string fName)
            {
                FirstName = fName;
                return this;
            }

            public Employee SetLastName(string lName)
            {
                LastName = lName;
                return this;
            }

            public Employee SetAge(int age)
            {
                Age = age;
                return this;
            }

            public void Print()
            {
                string tmp = String.Format("FirstName:{0}; LastName:{1}; Age:{2}",
                   FirstName, LastName, Age);
                Console.WriteLine(tmp);
            }
        }

        public class Client
        {
            public static void Test()
            {
                Employee empl = new Employee();
                empl.SetFirstName("John").SetLastName("Smith").SetAge(30).Print();

                Assert.AreEqual("John",empl.FirstName);
                Assert.AreEqual("Smith",empl.LastName);
            }
        }
    }
}
