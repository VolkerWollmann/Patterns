using System;

namespace BehaviourPatterns.Command
{
    /// https://www.dofactory.com/net/command-design-pattern
    /// The Command design pattern encapsulates a request as an object, 
    /// thereby letting you parameterize clients with different requests, queue or log requests, and support undoable operations.

    /// <summary>
    /// The 'Command' abstract class
    /// </summary>
    abstract class Command
    {
        protected Receiver receiver;

        // Constructor
        public Command(Receiver receiver)
        {
            this.receiver = receiver;
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
            receiver.Action();
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
        private Command _command;

        public void SetCommand(Command command)
        {
            this._command = command;
        }

        public void ExecuteCommand()
        {
            _command.Execute();
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