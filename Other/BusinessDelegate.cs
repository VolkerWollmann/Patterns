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
		private IBusinessService businessService;
		private string serviceType;

		public void SetServiceType(string serviceType)
		{
			this.serviceType = serviceType;
		}

		public void DoTask()
		{
			businessService = lookupService.GetBusinessService(serviceType);
			businessService.DoProcessing();
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
