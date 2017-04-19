using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Chat
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            Console.WriteLine("Username:");
            var chatName = Console.ReadLine();
            Console.Clear();

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var receivedBody = ea.Body;
                        var receivedMessage = Encoding.UTF8.GetString(receivedBody);
                        Console.WriteLine(receivedMessage);
                    };
                    channel.BasicConsume(queue: "broker",
                                         noAck: true,
                                         consumer: consumer);

                    while (true)
                    {
                        var message = $"{chatName}: {Console.ReadLine()}";
                        
                        channel.QueueDeclare(queue: "broker",
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: "broker",
                                             basicProperties: null,
                                             body: body);
                    }
                }
            }
        }
    }
}