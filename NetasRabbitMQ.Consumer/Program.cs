using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace NetasRabbitMQ.Consumer
{
    public enum LogNames
    {
        Critical,
        Error
    };

    internal class Program
    {
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);
                    channel.QueueDeclare("header_queue1", false, false, false, null);

                    Dictionary<string, object> headers = new Dictionary<string, object>();

                    headers.Add("format", "pdf");
                    headers.Add("shape", "a4");
                    headers.Add("x-match", "all");

                    channel.QueueBind("header_queue1", "header-exchange", routingKey: "", headers);
                    
                    var consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume("header_queue1", false, consumer);

                    consumer.Received += (model, ea) =>
                      {
                          var message = Encoding.UTF8.GetString(ea.Body);
                          Console.WriteLine($"Gelen mesaj:{message}");

                          channel.BasicAck(ea.DeliveryTag, multiple: false);
                      };
                    Console.WriteLine("Çıkış yapmak için tıklayınız.");
                    Console.ReadLine();
                }
            }
        }

        private static string GetMessage(string[] args)
        {
            return args[0].ToString();
        }
    }
}
