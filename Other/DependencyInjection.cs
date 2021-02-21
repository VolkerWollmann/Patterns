using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Other.DependencyInjection
{
    public class DependencyInjectionExample
    {
        // https://en.wikipedia.org/wiki/Dependency_injection
        // In software engineering, dependency injection is a technique 
        // in which an object receives other objects that it depends on.

        // example: https://de.wikipedia.org/wiki/Dependency_Injection
        private interface IDependency
        {
            void DoSomeThing();
        }

        private class FirstDependency : IDependency
        {
            void IDependency.DoSomeThing()
            {
                Console.WriteLine("Do Something by {0}", this.GetType().Name);
            }
        }

        private class Dependent
        {
            IDependency dependency;
            internal void UseDependency()
            {
                dependency.DoSomeThing();
            }
            public Dependent(IDependency actualDependency)
            {
                dependency = actualDependency;
            }
        }

        public static void DependencyInjection()
        {
            // actual injection
            IDependency actualDependency = new FirstDependency();
            Dependent dependent = new Dependent(actualDependency);
            dependent.UseDependency();
        }
    }
}
