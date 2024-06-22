﻿using RabbitMQ.Client;
using System.Text;

public class Program
{
    private static readonly List<string> cars = new List<string> { "BMW", "Audi", "Tesla", "Mercedes" };
    private static readonly List<string> colors = new List<string> { "red", "white", "black" };
    private static readonly Random random = new Random();

    public static void Main(string[] args)
    {
        var counter = 0;
        do
        {
            int timeToSleep = random.Next(1000, 2000);
            Thread.Sleep(timeToSleep);
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "topic_logs", type: ExchangeType.Topic);

                    string routingKey = counter % 4 == 0
                        ? "Tesla.red.fast.ecological"
                        : counter % 5 == 0
                         ? "Mersedes.exclusive.expensive.ecological"
                         : GenerateRoutingKey();

                    string message = $"Message type [{routingKey}] sent into Topic Exchange [N {counter}]";

                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "topic_logs", routingKey: routingKey, basicProperties: null, body: body);
                    Console.WriteLine($"Message type [{routingKey}] sent into Topic Exchange [N {counter++}]");
                }
            }

        } while (true);
        Console.ReadKey();
    }

    private static string GenerateRoutingKey()
    {
        return $"{cars[random.Next(0, 3)]}.{colors[random.Next(0, 2)]}";
    }
}