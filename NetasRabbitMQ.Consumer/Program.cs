using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace NetasRabbitMQ.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("hello", false, false, false, null);

                    var consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume("hello", true, consumer);

                    consumer.Received += (model, ea) =>
                      {
                          var bodyByte = ea.Body;
                          var message = Encoding.UTF8.GetString(bodyByte);

                          Console.WriteLine("Mesaj alındı: " + message);
                      };

                    Console.WriteLine("Çıkış yapmak için tıklayınız.");
                    Console.ReadLine();
                }


            }
        }
    }
}
