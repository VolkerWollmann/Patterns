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
        public int Id { get; set; }

        public string CustomerName { get; set; } = "";

        public void Insert()
        {
            Console.WriteLine("Insert " + CustomerName);
        }
        public static Customer Load()
        {
            Customer c = new Customer
            {
                CustomerName = "Customer1"
            };
            return c;
        }
        public void Update()
        {
            Console.WriteLine("Update " + CustomerName);
        }
    }
    internal class UnitOfWork
    {
        private readonly List<Customer> Changed = new List<Customer>();
        private readonly List<IEntity> New = new List<IEntity>();
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

            Customer customerObj = new Customer
            {
                Id = 1000,
                CustomerName = "shiv"
            };// record 1 Customer

            Customer supplierObj = new Customer
            {
                Id = 2000,
                CustomerName = "CustomerName"
            }; // Record 2 Customer

            UnitOfWork unitOfWork  = new UnitOfWork();
            
            unitOfWork.Add(customerObj); // record 1 added to in memory
            unitOfWork.Add(supplierObj); // record 1 added to in memory
            
            unitOfWork.Load().First().CustomerName += " changed";
           
            unitOfWork.Commit(); // The full in memory collection is sent for final commit 
        }
	}
}
