using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
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
                    channel.ExchangeDeclare("topic-exchange", durable: true, type: ExchangeType.Topic);

                    var queueName = channel.QueueDeclare().QueueName; /* Random Queue ismi üretiyor. */
                    
                    for (int i=0; i < args.Length; i++)
                    {
                        var item = args[i].ToString();
                        channel.QueueBind(queue: queueName, exchange: "topic-exchange", routingKey: item);
                        Console.WriteLine($"{args[i].ToString()} Loglar bekleniyor...");
                    }         
                    
                    channel.BasicQos(prefetchSize:0, prefetchCount:1, false);
                    
                    var consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume(queue:queueName, autoAck:false , consumer);

                    consumer.Received += (model, ea) =>
                      {
                          var bodyByte = ea.Body;
                          var log = Encoding.UTF8.GetString(bodyByte);
                          Console.WriteLine("Log alındı: " + log);

                          int time = 100;
                          Thread.Sleep(time);

                          Console.WriteLine("Loglama işlemi tamamlandı.");

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
