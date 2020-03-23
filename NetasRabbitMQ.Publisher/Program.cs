using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetasRabbitMQ.Publisher
{
    /*
    Critical.Error.Info
    Info.Warning.Critical
    */

    public enum LogNames
    {
        Critical=1,
        Error=2,
        Info=3,
        Warning=4
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

                    var properties = channel.CreateBasicProperties();

                    Dictionary<string, object> headers = new Dictionary<string, object>();

                    headers.Add("format", "pdf");
                    headers.Add("shape", "a4");

                    properties.Headers = headers;

                    channel.BasicPublish("header-exchange", routingKey:"", properties, Encoding.UTF8.GetBytes("Header Exchange Tipi ile gönderilen mesaj "));

                }

                Console.WriteLine("Çıkış yapmak için tıklayınız.");
                Console.ReadLine();
            }
        }

        private static string GetMessage(string[] args)
        {
            return args[0].ToString();
        }
    }
}
