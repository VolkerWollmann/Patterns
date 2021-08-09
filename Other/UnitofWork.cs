using System;
using System.Collections.Generic;
using System.Linq;

// Source : https://www.codeproject.com/Articles/581487/Unit-of-Work-Design-Pattern
// https://en.wikipedia.org/wiki/Business_delegate_pattern
namespace Patterns.Other
{
	public interface IEntity
	{
		int Id { set; get; }
		void Insert();
		void Update();
	}
    public class Customer : IEntity
    {
        private int _CustomerCode = 0;
        public int Id
        {
            get => _CustomerCode;
            set => _CustomerCode = value;
        }
        private string _CustomerName = "";
        public string CustomerName
        {
            get => _CustomerName;
            set => _CustomerName = value;
        }
        public void Insert()
        {
            Console.WriteLine("Insert " + CustomerName);
        }
        public static Customer Load()
        {
            Customer c = new Customer();
            c.CustomerName = "Customer1";
            return c;
        }
        public void Update()
        {
            Console.WriteLine("Update " + CustomerName);
        }
    }
    internal class UnitOfWork
    {
        private List<Customer> Changed = new List<Customer>();
        private List<IEntity> New = new List<IEntity>();
        public void Add(IEntity obj)
        {
            New.Add(obj);
        }
		public void Commit()
		{
			foreach (IEntity o in Changed)
			{
				o.Update();
			}
			foreach (IEntity o in New)
			{
				o.Insert();
			}
		}
		public List<Customer> Load()
        {
            Changed.Add(Customer.Load());
            return Changed;
        }
    }

    public class UnitOfWorkExample
	{
        public static void UnitOfWork()
		{
            Console.WriteLine();
            Console.WriteLine("Unit of Work");
            Console.WriteLine("=======================");

            Customer customerObj = new Customer();// record 1 Customer
            customerObj.Id = 1000;
            customerObj.CustomerName = "shiv";

            Customer supplierObj = new Customer(); // Record 2 Customer
            supplierObj.Id = 2000;
            supplierObj.CustomerName = "CustomerName";

            UnitOfWork unitOfWork  = new UnitOfWork();
            
            unitOfWork.Add(customerObj); // record 1 added to in memory
            unitOfWork.Add(supplierObj); // record 1 added to in memory
            
            unitOfWork.Load().First().CustomerName += " changed";
           
            unitOfWork.Commit(); // The full in memory collection is sent for final commit 
        }
	}
}
