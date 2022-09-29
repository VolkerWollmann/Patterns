using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.CreationalPatterns
{
    public class SingletonExample
    {
        // #Singleton
        // 
        //example : https://refactoring.guru/design-patterns/singleton/csharp/example#lang-features
        public sealed class Singleton
        {
            // The Singleton's constructor should always be private to prevent
            // direct construction calls with the `new` operator.
            private Singleton() { }

            // The Singleton's instance is stored in a static field. There there are
            // multiple ways to initialize this field, all of them have various pros
            // and cons. In this example we'll show the simplest of these ways,
            // which, however, doesn't work really well in multithreaded program.
            private static Singleton _instance;

            // This is the static method that controls the access to the singleton
            // instance. On the first run, it creates a singleton object and places
            // it into the static field. On subsequent runs, it returns the client
            // existing object stored in the static field.
            public static Singleton GetInstance()
            {
                if (_instance == null)
                {
                    _instance = new Singleton();
                }
                return _instance;
            }

            // Finally, any singleton should define some business logic, which can
            // be executed on its instance.
            public void someBusinessLogic()
            {
                Console.WriteLine("This is the singleton");
            }
        }

        public static void Example()
        {
            // The client code.
            Singleton s1 = Singleton.GetInstance();
            Singleton s2 = Singleton.GetInstance();

            if (s1 == s2)
            {
                Console.WriteLine("Singleton works, both variables contain the same instance.");
            }
            else
            {
                Console.WriteLine("Singleton failed, variables contain different instances.");
            }
        }
    }
}