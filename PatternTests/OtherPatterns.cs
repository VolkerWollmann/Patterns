
using Patterns.Other;
using Xunit;
using FI = Patterns.Other.FluentInterfaceTest;

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

        [Fact]
        public void ChainingSettersFillsEveryField()
        {
            FI.Employee employee = new FI.Employee();

            FI.Employee returned = employee.SetFirstName("John").SetLastName("Smith").SetAge(30);

            Assert.Equal("John", employee.FirstName);
            Assert.Equal("Smith", employee.LastName);
            Assert.Equal(30, employee.Age);

            // Every setter hands back the same instance - that is what makes the
            // chain work.
            Assert.Same(employee, returned);
        }

        [Fact]
        public void ADtoSurvivesTheRoundTripThroughXml()
        {
            DemoDto dto = new DemoDto
            {
                DemoId = "1",
                DemoName = "Data Transfer Object Demonstration Program",
                DemoProgrammer = "Kenny Young"
            };

            string xml = DtoSerializerHelper.SerializeDto(dto);
            DemoDto restored = (DemoDto)DtoSerializerHelper.DeserializeXml(xml, new DemoDto());

            Assert.Equal(dto.DemoId, restored.DemoId);
            Assert.Equal(dto.DemoName, restored.DemoName);
            Assert.Equal(dto.DemoProgrammer, restored.DemoProgrammer);
        }
    }
}
