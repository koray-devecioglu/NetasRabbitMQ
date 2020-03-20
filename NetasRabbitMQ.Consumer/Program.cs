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
                    channel.QueueDeclare("task_queue", durable:true, exclusive:false, autoDelete:false, null);

                    channel.BasicQos(prefetchSize:0, prefetchCount:1, false);

                    Console.WriteLine("Mesajlar bekleniyor...");

                    var consumer = new EventingBasicConsumer(channel);


                    channel.BasicConsume("task_queue", autoAck:false , consumer);

                    consumer.Received += (model, ea) =>
                      {
                          var bodyByte = ea.Body;
                          var message = Encoding.UTF8.GetString(bodyByte);
                          Console.WriteLine("Mesaj alındı: " + message);

                          int time = int.Parse(GetMessage(args));
                          Thread.Sleep(time);
                          Console.WriteLine("Mesaj işlendi...");

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
