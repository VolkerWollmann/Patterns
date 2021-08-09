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
		private BusinessLookUp lookupService = new BusinessLookUp();
		private IBusinessService BusinessService;
		private string ServiceType;

		public void SetServiceType(string serviceType)
		{
			this.ServiceType = serviceType;
		}

		public void DoTask()
		{
			BusinessService = lookupService.GetBusinessService(ServiceType);
			BusinessService.DoProcessing();
		}
	}

	class Client
	{
		BusinessDelegate businessService;

		public Client(BusinessDelegate businessService)
		{
			this.businessService = businessService;
		}

		public void DoTask()
		{
			businessService.DoTask();
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
