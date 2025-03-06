using System;

namespace Patterns.Examples
{
    /// https://www.dofactory.com/net/state-design-pattern
    /// The State design pattern allows an object to alter its behavior when its internal state changes. 
    /// The object will appear to change its class.

    public class StateExample
    {

        /// <summary>
        /// State Design Pattern
        /// </summary>


        /// <summary>
        /// The 'State' abstract class
        /// </summary>

        public abstract class State
        {
            protected double Interest;
            protected double LowerLimit;
            protected double UpperLimit;

            // Properties

            public Account? Account { get; set; }

            public double Balance { get; set; }

            public abstract void Deposit(double amount);
            public abstract void Withdraw(double amount);
            public abstract void PayInterest();

        }


        /// <summary>
        /// A 'ConcreteState' class
        /// <remarks>
        /// Red indicates that account is overdrawn 
        /// </remarks>
        /// </summary>

        public class RedState : State
        {
            private double ServiceFee;

            // Constructor

            public RedState(State state)
            {
                this.Balance = state.Balance;
                this.Account = state.Account;
                Initialize();
            }

            private void Initialize()
            {
                // Should come from a data source

                Interest = 0.0;
                LowerLimit = -100.0;
                UpperLimit = 0.0;
                ServiceFee = 15.00;
            }

            public override void Deposit(double amount)
            {
                Balance += amount;
                StateChangeCheck();
            }

            public override void Withdraw(double amount)
            {
                Balance -= amount - ServiceFee;
                Console.WriteLine("No funds available for withdrawal!");
            }

            public override void PayInterest()
            {
                // No interest is paid
            }

            private void StateChangeCheck()
            {
                if (Balance > UpperLimit)
                {
                    if (Account != null)
                        Account.State = new SilverState(this);
                }
            }
        }

        /// <summary>
        /// A 'ConcreteState' class
        /// <remarks>
        /// Silver indicates a non-interest bearing state
        /// </remarks>
        /// </summary>

        public class SilverState : State
        {
            // Overloaded constructors

            public SilverState(State state) :
                this(state.Balance, state.Account)
            {
            }

            public SilverState(double balance, Account? account)
            {
                this.Balance = balance;
                this.Account = account;
                Initialize();
            }

            private void Initialize()
            {
                // Should come from a data source
                Interest = 0.0;
                LowerLimit = 0.0;
                UpperLimit = 1000.0;
            }

            public override void Deposit(double amount)
            {
                Balance += amount;
                StateChangeCheck();
            }

            public override void Withdraw(double amount)
            {
                Balance -= amount;
                StateChangeCheck();
            }

            public override void PayInterest()
            {
                Balance += Interest * Balance;
                StateChangeCheck();
            }

            private void StateChangeCheck()
            {
                if (Account != null)
                {
                    if (Balance < LowerLimit)
                    {
                        Account.State = new RedState(this);
                    }
                    else if (Balance > UpperLimit)
                    {
                        Account.State = new GoldState(this);
                    }
                }
            }
        }

        /// <summary>
        /// A 'ConcreteState' class
        /// <remarks>
        /// Gold indicates an interest bearing state
        /// </remarks>
        /// </summary>

        public class GoldState : State
        {
            // Overloaded constructors
            public GoldState(State state)
                : this(state.Balance, state.Account)
            {
            }

            public GoldState(double balance, Account? account)
            {
                this.Balance = balance;
                this.Account = account;
                Initialize();
            }

            private void Initialize()
            {
                // Should come from a database
                Interest = 0.05;
                LowerLimit = 1000.0;
                UpperLimit = 10000000.0;
            }

            public override void Deposit(double amount)
            {
                Balance += amount;
                StateChangeCheck();
            }

            public override void Withdraw(double amount)
            {
                Balance -= amount;
                StateChangeCheck();
            }

            public override void PayInterest()
            {
                Balance += Interest * Balance;
                StateChangeCheck();
            }

            private void StateChangeCheck()
            {
                if (Account != null)
                {
                    if (Balance < 0.0)
                    {
                        Account.State = new RedState(this);
                    }
                    else if (Balance < LowerLimit)
                    {
                        Account.State = new SilverState(this);
                    }
                    else
                    {
                        Account.State = new GoldState(this);
                    }
                }
            }
        }

        /// <summary>
        /// The 'Context' class
        /// </summary>

        public class Account
        {
            private readonly string Owner;

            // Constructor

            public Account(string owner)
            {
                // New accounts are 'Silver' by default
                this.Owner = owner;
                this.State = new SilverState(0.0, this);
            }

            public double Balance => (State != null ) ? State.Balance : 0.0;

            public State State { get; set; }

            public void Deposit(double amount)
            {
                State.Deposit(amount);
                Console.WriteLine("Account:" + Owner);
                Console.WriteLine("Deposited {0:C} --- ", amount);
                Console.WriteLine(" Balance = {0:C}", this.Balance);
                Console.WriteLine(" Status  = {0}",
                    this.State.GetType().Name);
                Console.WriteLine("");
            }

            public void Withdraw(double amount)
            {
                State.Withdraw(amount);
                Console.WriteLine("Withdrew {0:C} --- ", amount);
                Console.WriteLine(" Balance = {0:C}", this.Balance);
                Console.WriteLine(" Status  = {0}\n",
                    this.State.GetType().Name);
            }

            public void PayInterest()
            {
                State.PayInterest();
                Console.WriteLine("Interest Paid --- ");
                Console.WriteLine(" Balance = {0:C}", this.Balance);
                Console.WriteLine(" Status  = {0}\n",
                    this.State.GetType().Name);
            }
        }

        public static void Test()
        {
            // Open a new account

            Account account = new Account("Jim Johnson");

            // Apply financial transactions

            account.Deposit(500.0);
            account.Deposit(300.0);
            account.Deposit(550.0);
            account.PayInterest();
            account.Withdraw(2000.00);
            account.Withdraw(1100.00);

            // Wait for user


        }
    }
}
