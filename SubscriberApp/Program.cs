using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SubscriberApp
{
    class Program
    {
        static string exchange_1 = "ex_1";
        static string exchange_2 = "ex_2";
        static int messageCounter = 0;
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var chanel = connection.CreateModel())
            {
                // declare default queue, which have random name
                var queueName = chanel.QueueDeclare().QueueName;

                // binding queue to exchanges
                chanel.QueueBind(queueName, exchange_1, "");
                chanel.QueueBind(queueName, exchange_2, "");

                // fair
                chanel.BasicQos(0, 1, false);

                // consuming messages
                var consumer = new EventingBasicConsumer(chanel);
                consumer.Received += (sender, ea) =>
                {
                    var message = Encoding.ASCII.GetString(ea.Body.ToArray());

                    System.Console.WriteLine($"Queue name is: {queueName}");
                    ReadMessage(message);

                    chanel.BasicAck(ea.DeliveryTag, false);
                };

                chanel.BasicConsume(queueName, false, consumer);

                System.Console.WriteLine("Enter to exit...");
                Console.ReadKey();
            } 
        }

        static void ReadMessage(string message)
        {
            var currentMessageCount = ++messageCounter;

            System.Console.WriteLine($"Reading new message {currentMessageCount}...");
            Thread.Sleep(10000);
            System.Console.WriteLine($"Message {currentMessageCount} is {message}");
        }
    }
}
