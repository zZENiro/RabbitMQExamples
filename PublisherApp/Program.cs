using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PublisherApp
{
    class Program
    {
        static string exchange_1 = "ex_1";
        static string exchange_2 = "ex_2";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using(var connection = factory.CreateConnection())
            using(var chanel = connection.CreateModel())
            {
                // declaring 2 exchanges  
                chanel.ExchangeDeclare(exchange_1, ExchangeType.Fanout);
                chanel.ExchangeDeclare(exchange_2, ExchangeType.Fanout);

                var messageBody = Encoding.ASCII.GetBytes(GetMessage(args));

                var publishProps = chanel.CreateBasicProperties();
                publishProps.Persistent = true;

                // publish message to exchanges
                chanel.BasicPublish(exchange_1, "", publishProps, messageBody);
                chanel.BasicPublish(exchange_2, "", publishProps, messageBody);
            }

        }

        static string GetMessage(string[] args) =>string.Join(" ", args);

    }
}
