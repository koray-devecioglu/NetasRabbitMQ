using RabbitMQ.Client;
using System;
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
                    channel.ExchangeDeclare("topic-exchange", durable: true, type: ExchangeType.Topic);

                    Array log_name_array = Enum.GetValues(typeof(LogNames));

                    int counter = int.Parse(args[0].ToString());

                    for (int i = 0; i < counter; i++)
                    {
                        Random rnd = new Random();

                        LogNames log1 = (LogNames)log_name_array.GetValue(rnd.Next(log_name_array.Length)); /* 1-4 arası Random Değer */
                        LogNames log2 = (LogNames)log_name_array.GetValue(rnd.Next(log_name_array.Length)); /* 1-4 arası Random Değer */
                        LogNames log3 = (LogNames)log_name_array.GetValue(rnd.Next(log_name_array.Length)); /* 1-4 arası Random Değer */

                        string RoutingKey = $"{log1}.{log2}.{log3}";

                        var bodyByte = Encoding.UTF8.GetBytes($"log={log1.ToString()}-{log2.ToString()}-{log3.ToString()}");
                        var properties = channel.CreateBasicProperties();

                        properties.Persistent = true;

                        channel.BasicPublish("topic-exchange", routingKey:RoutingKey, properties, body: bodyByte);

                        Console.WriteLine($"Log mesajı gönderilmiştir: {RoutingKey}");
                    }

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
