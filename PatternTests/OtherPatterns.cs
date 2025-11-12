
using Patterns.Other;
using Xunit;

namespace PatternTests
{
    public class OtherPatternsTests
    {
        [Fact]
        public void DataTransferObject()
        {
            DataTransferObjectExample dataTransferObjectExample = new DataTransferObjectExample();
            dataTransferObjectExample.StartDemo();
        }

        [Fact]
        public void UnitOfWork()
        {
            UnitOfWorkExample.UnitOfWork();
        }

        [Fact]
        public void BusinessDelegate()
        {
            BusinessDelegateExample.BusinessDelegate();
        }

        [Fact]
        public void DependencyInjection()
        {
            DependencyInjectionExample.DependencyInjection();
        }

        [Fact]
        public void FluentInterface()
        {
            FluentInterfaceTest.Client.Test();
        }

        [Fact]
        public void ServiceLocator()
        {
            ServiceLocatorExample.ServiceLocatorTest();
        }
    }
}
