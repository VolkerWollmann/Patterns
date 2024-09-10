using System;

namespace Patterns.BehaviourPatterns
{
	/// https://www.dofactory.com/net/command-design-pattern
	/// The Command design pattern encapsulates a request as an object, 
	/// thereby letting you parametrize clients with different requests, queue or log requests, and support undoable operations.
	///
	/// https://en.wikipedia.org/wiki/Command_pattern
	/// In object-oriented programming, the command pattern is a behavioral design pattern in which an object is used
	/// to encapsulate all information needed to perform an action or trigger an event at a later time.
	/// This information includes the method name, the object that owns the method and values for the method parameters.

	/// <summary>
	/// The 'Command' abstract class
	/// </summary>
	abstract class Command
    {
        protected Receiver Receiver;

        // Constructor
        protected Command(Receiver receiver)
        {
            this.Receiver = receiver;
        }
        public abstract void Execute();
    }

    /// <summary>
    /// The 'ConcreteCommand' class
    /// </summary>
    class ConcreteCommand : Command
    {
        // Constructor
        public ConcreteCommand(Receiver receiver) :
          base(receiver)
        {
        }

        public override void Execute()
        {
            Receiver.Action();
        }
    }

    /// <summary>
    /// The 'Receiver' class
    /// </summary>
    class Receiver
    {
        public void Action()
        {
            Console.WriteLine("Called Receiver.Action()");
        }
    }

    /// <summary>
    /// The 'Invoker' class
    /// </summary>
    class Invoker
    {
        private Command Command;

        public void SetCommand(Command command)
        {
            this.Command = command;
        }

        public void ExecuteCommand()
        {
            Command.Execute();
        }
    }

    /// <summary>
    /// MainApp startup class for Structural 
    /// Command Design Pattern.
    /// https://www.dofactory.com/net/command-design-pattern
    /// </summary>
    public class CommandExample
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>
        public static void Command()
        {
            // Create receiver, command, and invoker
            Receiver receiver = new Receiver();
            Command command = new ConcreteCommand(receiver);
            Invoker invoker = new Invoker();

            // Set and execute command
            invoker.SetCommand(command);
            invoker.ExecuteCommand();
        }
    }

}