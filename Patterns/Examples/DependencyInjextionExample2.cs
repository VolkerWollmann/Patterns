using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Examples
{

    #region DependencyInjectionExample2
    internal class EmailService
    {
        public void SendEmail(string message)
        {
            Console.WriteLine("Sending email: " + message);
        }
    }

    internal class NotificationManager
    {
        private EmailService _emailService;

        public NotificationManager()
        {
            _emailService = new EmailService(); // tightly coupled
        }

        public void SendNotification(string message)
        {
            _emailService.SendEmail(message);
        }

    }

    public interface IMessageService
    {
        void SendMessage(string message);
    }

    public class EmailService2 : IMessageService
    {
        public void SendMessage(string message)
        {
            Console.WriteLine("Email sent: " + message);
        }
    }

    public class LooselyCoupledNotificationManager
    {
        private readonly IMessageService _messageService;

        // Constructor injection
        public LooselyCoupledNotificationManager(IMessageService messageService)
        {
            _messageService = messageService;  // loosely coupled
        }

        public void SendNotification(string message)
        {
            _messageService.SendMessage(message);
        }
    }


    public class DependencyInjectionExample2Test
    {
        public static void Example()
        {
            NotificationManager notificationManager = new NotificationManager();
            notificationManager.SendNotification("Hello world");

            LooselyCoupledNotificationManager looselyNotificationManager = new LooselyCoupledNotificationManager(new EmailService2());
            notificationManager.SendNotification("Hello world");
        }


    }

    #endregion
}
