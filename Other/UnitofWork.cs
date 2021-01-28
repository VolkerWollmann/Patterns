using System;
using System.Collections.Generic;
using System.Linq;

// Source : https://www.codeproject.com/Articles/581487/Unit-of-Work-Design-Pattern
// https://en.wikipedia.org/wiki/Business_delegate_pattern
namespace Other.UnitOfWork
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
            get { return _CustomerCode; }
            set { _CustomerCode = value; }
        }
        private string _CustomerName = "";
        public string CustomerName
        {
            get { return _CustomerName; }
            set { _CustomerName = value; }
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

    public class UnitOFWorkExample
	{
        public static void UnitOfWork()
		{
            Console.WriteLine();
            Console.WriteLine("Unit of Work");
            Console.WriteLine("=======================");

            Customer Customerobj = new Customer();// record 1 Customer
            Customerobj.Id = 1000;
            Customerobj.CustomerName = "shiv";

            Customer Supplierobj = new Customer(); // Record 2 Customer
            Supplierobj.Id = 2000;
            Supplierobj.CustomerName = "xxxx";

            UnitOfWork unitOfWork  = new UnitOfWork();
            
            unitOfWork.Add(Customerobj); // record 1 added to inmemory
            unitOfWork.Add(Supplierobj); // record 1 added to inmemory
            
            unitOfWork.Load().First().CustomerName += " changed";
           
            unitOfWork.Commit(); // The full inmemory collection is sent for final committ 
        }
	}
}
