using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace NetasRabbitMQ.Consumer
{
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
                    channel.ExchangeDeclare("logs", durable: true, type: ExchangeType.Fanout);

                    var queueName = channel.QueueDeclare().QueueName; /* Random Queue ismi üretiyor. */

                    channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "");

                    channel.BasicQos(prefetchSize:0, prefetchCount:1, false);

                    Console.WriteLine("Loglar bekleniyor...");

                    var consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume(queue:queueName, autoAck:false , consumer);

                    consumer.Received += (model, ea) =>
                      {
                          var bodyByte = ea.Body;
                          var log = Encoding.UTF8.GetString(bodyByte);
                          Console.WriteLine("Log alındı: " + log);

                          int time = int.Parse(GetMessage(args));
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
