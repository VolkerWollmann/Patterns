using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Other
{
    public class ServiceLocatorExample
    {
        // #Service Locator
        // http://www.stefanoricciardi.com/2009/09/25/service-locator-pattern-in-csharpa-simple-example/
        // A Service Locator is a common design pattern that allows decoupling clients of services (described by a public interface) from the concrete class implementing those services. 

        private interface IServiceLocator
        {
            T GetService<T>();
        }

		#region Services : Interfaces
		private interface IServiceA
        {

        }

		private interface IServiceB
		{

		}

		private interface IServiceC
		{

		}

		#endregion

		#region Services : Implementations
		private class ServiceA : IServiceA
		{
		}

		private class ServiceB : IServiceB
		{
		}

		private class ServiceC : IServiceC
		{
		}

		#endregion

		private class ServiceLocator : IServiceLocator
		{
			// map that contains pairs of interfaces and
			// references to concrete implementations
			private IDictionary<object, object> services;

			internal ServiceLocator()
			{
				services = new Dictionary<object, object>();

				// fill the map
				this.services.Add(typeof(IServiceA), new ServiceA());
				this.services.Add(typeof(IServiceB), new ServiceB());
				this.services.Add(typeof(IServiceC), new ServiceC());
			}

			public T GetService<T>()
			{
				try
				{
					return (T)services[typeof(T)];
				}
				catch (KeyNotFoundException)
				{
					throw new ApplicationException("The requested service is not registered");
				}
			}
		}

		public static void ServiceLocatorTest()
        {
			IServiceLocator locator = new ServiceLocator();

			// only know about the interface
			IServiceA myServiceA = locator.GetService<IServiceA>();
		}

	}
}
