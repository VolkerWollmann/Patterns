using System;

// Source : https://www.geeksforgeeks.org/business-delegate-pattern/
//          https://en.wikipedia.org/wiki/Business_delegate_pattern
namespace Patterns.Other
{

	interface IBusinessService
	{
		void DoProcessing();
	}

	class OneService : IBusinessService
	{
		public void DoProcessing()
		{
			Console.WriteLine("Processed Service One");
		}
	}

	class TwoService : IBusinessService
	{
		public void DoProcessing()
		{
			Console.WriteLine("Processed Service Two");
		}
	}

	class BusinessLookUp
	{
		public IBusinessService GetBusinessService(string serviceType)
		{
			if (string.Equals("One", serviceType, StringComparison.InvariantCultureIgnoreCase))
			{
				return new OneService();
			}
			else
			{
				return new TwoService();
			}
		}
	}

	class BusinessDelegate
	{
		private readonly BusinessLookUp LookupService = new BusinessLookUp();
		private IBusinessService BusinessService; 
		private string ServiceType ="";

		public void SetServiceType(string serviceType)
		{
			this.ServiceType = serviceType;
		}

		public void DoTask()
		{
			BusinessService = LookupService.GetBusinessService(ServiceType);
			BusinessService.DoProcessing();
		}

		public BusinessDelegate()
		{
            BusinessService = LookupService.GetBusinessService(ServiceType);
        }
    }

	class Client
	{
        readonly BusinessDelegate BusinessService;

		public Client(BusinessDelegate businessService)
		{
			this.BusinessService = businessService;
		}

		public void DoTask()
		{
			BusinessService.DoTask();
		}
	}

	public class BusinessDelegateExample
	{
		public static void BusinessDelegate()
		{
			BusinessDelegate businessDelegate = new BusinessDelegate();
			businessDelegate.SetServiceType("One");

			Client client = new Client(businessDelegate);
			client.DoTask();

			businessDelegate.SetServiceType("Two");
			client.DoTask();
		}
	}

}
